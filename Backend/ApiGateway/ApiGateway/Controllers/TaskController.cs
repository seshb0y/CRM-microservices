using ApiGateway.DTO.Requests.Task;
using ApiGateway.DTO.Responses;
using ApiGateway.Hubs;
using AutoMapper;
using CRMSolution.DTO.Requests;
using CRMSolution.DTO.Requests.Task;
using CRMSolution.Grpc.Tasks;
// using CRMSolution.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DeleteTaskRequest = CRMSolution.Grpc.Tasks.DeleteTaskRequest;
using Enum = System.Enum;
using SortTasksRequest = CRMSolution.Grpc.Tasks.SortTasksRequest;
using UpdateTaskRequest = CRMSolution.Grpc.Tasks.UpdateTaskRequest;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/v1/tasks/")]
public class TaskController : ControllerBase
{
    private readonly TaskGrpcService.TaskGrpcServiceClient _tasksService;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificationHub> _hubContext;

    public TaskController(TaskGrpcService.TaskGrpcServiceClient tasksService, IMapper mapper,  IHubContext<NotificationHub> hubContext)
    {
        _tasksService = tasksService;
        _mapper = mapper;
        _hubContext = hubContext;
    }



    [HttpPost]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> AddTask([FromBody] HttpCreateTaskRequest request)
    {
        var grpcRequest = new CreateTaskRequest
        {
            Title = request.title,
            Description = request.description,
            DueDate = Timestamp.FromDateTime(request.endDate.ToUniversalTime()),
            OrderId = request.orderId,
        };
        var grpcResponse = await _tasksService.CreateTaskAsync(grpcRequest);
        
        await _hubContext.Clients.All.SendAsync("TaskCreated", new
        {
            grpcResponse.Id,
            grpcResponse.Title,
            grpcResponse.Description,
            Status = (TaskStatus)grpcResponse.Status,
            grpcResponse.DueDate,
            grpcResponse.OrderId
        });
        
        return Ok("Task created");
    }
    
    private static string NormalizeStatus(string input)
    {
        return input
            .Replace(" ", "", StringComparison.OrdinalIgnoreCase)
            .Replace("_", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
    }
    [HttpPut]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> ChangeTask([FromBody] HttpUpdateTaskRequest request)
    {
        string normalizedStatus = NormalizeStatus(request.status);

        if (!Enum.TryParse<GrpcTaskStatus>(normalizedStatus, ignoreCase: true, out var parsedStatus))
        {
            return BadRequest($"Invalid task status: {request.status}");
        }
        var grpcRequest = new UpdateTaskRequest
        {
            Description = request.description,
            Status = parsedStatus,
            TaskId = request.taskId,
        };
        var grpcResponse = await _tasksService.UpdateTaskAsync(grpcRequest);
        
        await _hubContext.Clients.All.SendAsync("TaskUpdated", new
        {
            grpcResponse.Id,
            grpcResponse.Title,
            grpcResponse.Description,
            Status = grpcResponse.Status.ToString(),
            grpcResponse.DueDate
        });
        
        return Ok("Task updated");
    }
    
    [HttpDelete]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> DeleteTask([FromBody] HttpDeleteTaskRequest request)
    {
        var grpcRequest = new DeleteTaskRequest
        {
            Id = request.taskId
        };
        var grpcResponse = await _tasksService.DeleteTaskAsync(grpcRequest);
        
        await _hubContext.Clients.All.SendAsync("TaskDeleted", new
        {
            grpcResponse.TaskId,
        });
        
        return Ok("Task deleted");
    }
    
    [HttpGet("search")]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> FindTask([FromQuery] HttpFindTaskRequest request)
    {
        var grpcRequest = new GetTaskByIdRequest
        {
            Id = request.taskId
        };
        var grpcResponse = await _tasksService.GetTaskByIdAsync(grpcRequest);
        return Ok(grpcResponse);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTasks([FromQuery] HttpSortTasksRequest sortTasksRequest)
    {
        var grpcRequest = new GetAllTasksRequest
        {
            Sort = new SortTasksRequest
            {
                Descending = sortTasksRequest.Descending,
                SortBy = sortTasksRequest.sortBy
            }
        };
        
        var grpcResponse = await _tasksService.GetAllTasksAsync(grpcRequest);
        var httpResponse = _mapper.Map<List<TaskDto>>(grpcResponse);
        return Ok(new HttpGetAllTasksResponse{Tasks = httpResponse});
    }
    
}