using AutoMapper;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Grpc.Users;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR;
using TaskService.Data.Models;
using TaskService.Data.Repository.TasksRep;
using TaskService.DTO.Requests.Task;
using TaskService.DTO.Responses;
using TaskService.Hubs;
using TaskService.Services.Interfaces;
using TaskInfo = CRMSolution.Grpc.Tasks.TaskInfo;
using TasksStatus = CRMSolution.Grpc.Tasks.GrpcTaskStatus;
using UpdateTaskRequest = CRMSolution.Grpc.Tasks.UpdateTaskRequest;

namespace TaskService.Services.Classes;

public class TasksService : ITasksService
{
    private readonly ITasksRep _tasksRep;
    private readonly IMapper _mapper;
    private readonly ILogger<TasksService> _logger;
    private readonly IHubContext<NotificationHub>  _hubContext;
    private readonly UserService.UserServiceClient _userService;

    public TasksService(ITasksRep tasksRep, IMapper mapper, ILogger<TasksService> logger,  IHubContext<NotificationHub> hubContext,
        UserService.UserServiceClient userService)
    {
        _tasksRep = tasksRep;
        _mapper = mapper;
        _logger = logger;
        _hubContext = hubContext;
        _userService = userService;
    }
    
    // public async Task CreateTaskAsync(CreateTaskRequest request)
    // {
    //     _logger.LogInformation("Создаем новую задачу: {@Request}", request);
    //     
    //     Order order = await _unitOfWork.OrderRep.GetById(request.orderId);
    //     
    //     User user = await _unitOfWork.UserRep.FindByNameAsync(request.userName);
    //     
    //     Tasks task = _mapper.Map<Tasks>(request);
    //     Console.Write(order);
    //     Console.WriteLine(task);
    //     Console.WriteLine(user);
    //     await _unitOfWork.TasksRep.AddDependency(order, user, task);
    //     await _unitOfWork.SaveChangesAsync();
    //     await _hubContext.Clients.All.SendAsync("TaskCreated", new
    //     {
    //         task.Id,
    //         task.Title,
    //         task.Description,
    //         task.Status,
    //         task.DueDate,
    //         orderId = order.Id
    //     });
    // }
    //
    // public async Task UpdateTaskAsync(UpdateTaskRequest request)
    // {
    //     _logger.LogInformation("Обновляем задачу: {@Request}", request);
    //     Tasks task = await _unitOfWork.TasksRep.GetById(request.taskId);
    //     task = _mapper.Map(request, task);
    //     await _unitOfWork.SaveChangesAsync();
    //     await _hubContext.Clients.All.SendAsync("TaskUpdated", new
    //     {
    //         task.Id,
    //         task.Title,
    //         task.Description,
    //         task.Status,
    //         task.DueDate
    //     });
    // }
    //
    // public async Task DeleteTaskAsync(DeleteTaskRequest request)
    // {
    //     _logger.LogInformation("Удаляем задачу: {@Request}", request);
    //     Tasks task = await _unitOfWork.TasksRep.GetById(request.taskId);
    //     _unitOfWork.TasksRep.Delete(task);
    //     await _unitOfWork.SaveChangesAsync();
    //     await _hubContext.Clients.All.SendAsync("TaskDeleted", new
    //     {
    //         task.Id,
    //     });
    // }
    //
    // public async Task<TaskResponse> FindTaskByIdAsync(FindTaskRequest request)
    // {
    //     _logger.LogInformation("Находим задачу: {@Request}", request);
    //     Tasks task = await _unitOfWork.TasksRep.GetById(request.taskId);
    //     return _mapper.Map<TaskResponse>(task);
    // }
    //
    // public async Task<GetAllTasksResponse> GetAllTasks(SortTasksRequest sortTasksRequest)
    // {
    //     var tasks = await _tasksRep.TasksRep.GetLowInfoTasksList(sortTasksRequest);
    //     return new GetAllTasksResponse()
    //     {
    //         Tasks = _mapper.Map<List<TaskDto>>(tasks)
    //     };
    // }
    public async Task<CreateTaskResponse> CreateTaskAsync(int orderId, string description, DateTime dueDate, string title)
    {
        TaskEntity task = new TaskEntity
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            OrderId = orderId
        };
        await _tasksRep.AddAsync(task);
        await _tasksRep.SaveChangesAsync();
        return new CreateTaskResponse
        {
            OrderId = orderId,
            Description = description,
            DueDate = Timestamp.FromDateTime(dueDate.ToUniversalTime()),
            Title = title,
            Status = (TasksStatus)task.Status,
            Id = task.Id
        };

    }

    public async Task<TaskInfo> UpdateTaskAsync(UpdateTaskRequest request)
    {
        TaskEntity task = await _tasksRep.GetById(request.TaskId);
        _mapper.Map(request, task); 
        _tasksRep.Update(task);
        await _tasksRep.SaveChangesAsync();

        return new TaskInfo
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = Timestamp.FromDateTime(task.DueDate.ToUniversalTime()),
            Status = (TasksStatus)task.Status,
        };
    }

    public async Task<DeleteTaskResponse> DeleteTaskAsync(DeleteTaskRequest request)
    {
        TaskEntity task = await _tasksRep.GetById(request.Id);
        _tasksRep.Delete(task);
        await _tasksRep.SaveChangesAsync();

        return new DeleteTaskResponse
        {
            TaskId = request.Id,
        };
    }

    public async Task<GetAllTasksResponse> GetAllTasks(SortTasksRequest sortTasksRequest)
    {
        var tasks = (await _tasksRep.GetAllAsync()).ToList();
        
        var userIds = tasks
            .Where(t => t.UserId != 0)
            .Select(t => t.UserId)
            .Distinct()
            .ToList();
        
        var userResponse = await _userService.GetUsersByIdsAsync(new GetUsersByIdsRequest
        {
            Ids = { userIds }
        });
        
        var usernames = userResponse.Usernames.ToList();
        var usersDict = userIds.Zip(usernames).ToDictionary(x => x.First, x => x.Second);
        
        var grpcTasks = tasks.Select(t => new TaskInfo
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = (TasksStatus)t.Status,
            DueDate = Timestamp.FromDateTime(t.DueDate.ToUniversalTime()),
            OrderId =  t.OrderId,
            Username = usersDict.TryGetValue(t.UserId, out var username) ? username : string.Empty

        }).ToList();
        
        grpcTasks = sortTasksRequest.SortBy.ToLower() switch
        {
            "taskid" => sortTasksRequest.Descending
                ? grpcTasks.OrderByDescending(x => x.Id).ToList()
                : grpcTasks.OrderBy(x => x.Id).ToList(),

            "title" => sortTasksRequest.Descending
                ? grpcTasks.OrderByDescending(x => x.Title).ToList()
                : grpcTasks.OrderBy(x => x.Title).ToList(),

            "status" => sortTasksRequest.Descending
                ? grpcTasks.OrderByDescending(x => x.Status).ToList()
                : grpcTasks.OrderBy(x => x.Status).ToList(),
            
            "description" => sortTasksRequest.Descending
                ? grpcTasks.OrderByDescending(x => x.Description).ToList()
                : grpcTasks.OrderBy(x => x.Description).ToList(),
            
            "username" => sortTasksRequest.Descending
                ? grpcTasks.OrderByDescending(x => x.Username).ToList()
                : grpcTasks.OrderBy(x => x.Username).ToList(),
            
            "duedate" => sortTasksRequest.Descending
                ? grpcTasks.OrderByDescending(x => x.DueDate.Seconds).ToList()
                : grpcTasks.OrderBy(x => x.DueDate.Seconds).ToList(),

            _ => grpcTasks
        };
        
        return new GetAllTasksResponse
        {
            Tasks = { grpcTasks }
        };
    }

    
    public async Task<TaskEntity> GetByIdAsync(int id)
    {
        return await _tasksRep.GetById(id);
    }

    public async Task<GetTaskByOrderIdResponse> GetTasksByOrderIdAsync(int orderId)
    {
        var tasks = await _tasksRep.GetTasksByOrderId(orderId);

        var response = new GetTaskByOrderIdResponse();
        response.Tasks.AddRange(tasks.Select(t => new GetTaskByIdResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = (int)t.Status,
            DueDate = Timestamp.FromDateTime(t.DueDate),
            OrderId = t.OrderId
        }));

        return response;
    }

    public async Task<GetTasksByUserIdsResponse> GetTasksByUserIdsAsync(GetTasksByUserIdsRequest request)
    {
        var tasks = await _tasksRep.GetTasksByUserId(request.UserIds.ToList());
        var response = new GetTasksByUserIdsResponse();
        foreach (var task in tasks)
        {
            response.Tasks.Add(new TaskWithUserId
            {
                UserId = task.UserId,
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = (TasksStatus)task.Status,
                DueDate = Timestamp.FromDateTime(task.DueDate.ToUniversalTime())
            });
        }
        return response;
    }
    
    public async Task<GetTasksByOrderIdsResponse> GetTasksByOrderIdsAsync(GetTasksByOrderIdsRequest request)
    {
        var tasks = await _tasksRep.GetTasksByOrderIds(request.OrderIds.ToList());
        var response = new GetTasksByOrderIdsResponse();
        foreach (var task in tasks)
        {
            response.Tasks.Add(new GetTaskByIdResponse()
            {
                OrderId = task.OrderId,
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = (int)task.Status,
                DueDate = Timestamp.FromDateTime(task.DueDate.ToUniversalTime())
            });
        }
        return response;
    }
}