using AutoMapper;
using ControllerFirst.DTO.Responses.User;
using CRMSolution.Data.Models;
using CRMSolution.Data.Repository.UserRep;
using CRMSolution.DTO.Requests;
using CRMSolution.Grpc.Client;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Grpc.Users;
using CRMSolution.Hubs;
using CRMSolution.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR;
using GrpcTaskStatus = CRMSolution.Grpc.Users.GrpcTaskStatus;
using TaskInfo = CRMSolution.Grpc.Users.TaskInfo;
using OrderStatus = CRMSolution.Grpc.Users.OrderStatus;
using UserRole = CRMSolution.Grpc.Users.UserRole;

namespace CRMSolution.Services.Classes;

public class UserService : IUserService
{
    private readonly IUserRep _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly OrderGrpcService.OrderGrpcServiceClient _orderGrpcService;
    private readonly TaskGrpcService.TaskGrpcServiceClient _taskGrpcService;
    private readonly ClientGrpcService.ClientGrpcServiceClient _clientGrpcService;
    
    public UserService(IUserRep userRepository, IMapper mapper, ILogger<UserService> logger, IHubContext<NotificationHub> notificationHub
    , OrderGrpcService.OrderGrpcServiceClient orderGrpcService,  TaskGrpcService.TaskGrpcServiceClient taskGrpcService,
    ClientGrpcService.ClientGrpcServiceClient clientGrpcService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _notificationHub = notificationHub;
        _orderGrpcService = orderGrpcService;
        _taskGrpcService = taskGrpcService;
        _clientGrpcService = clientGrpcService;
    }
    
    // public async Task<User> CreateUser(CreateUserRequest request)
    // {
    //     _logger.LogInformation("Создаем нового юзера: {@Request}", request);
    //     User user = _mapper.Map<User>(request);
    //     await _userRepository.UserRep.AddAsync(user);
    //     await _userRepository.SaveChangesAsync();
    //     return await _userRepository.UserRep.FindByEmailAsync(request.email);
    // }

    public async Task<ChangeUserDataResponse> ChangeUserData(ChangeUserDataRequest request)
    {
        _logger.LogInformation("Изменяем данные юзера: {@Request}", request);
        User user = await _userRepository.FindByEmailAsync(request.OldEmail);
        user = _mapper.Map(request, user);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        return _mapper.Map<ChangeUserDataResponse>(user);
    }
    
    public async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request)
    {
        _logger.LogInformation("Удаляем юзера: {@Request}", request);
        User user = await _userRepository.FindByEmailAsync(request.Email);
        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
        return new DeleteUserResponse
        {
            UserId = user.Id
        };
    }

    public async Task<GetUserResponse> GetUserByUsername(GetUserByEmailRequest request)
    {
        User? user;
        
        if (request.Email.Contains("@"))
        {
            user = await _userRepository.FindByEmailAsync(request.Email);
        }
        else
        {
            user = await _userRepository.FindByNameAsync(request.Email);
        }
        return new GetUserResponse
        {
            Email = user.Email,
            Id = user.Id,
            Username = user.Username,
            IsEmailConfirmed = user.IsEmailConfirmed,
            Role = (UserRole)user.Role,
        };
    }
    public async Task<FindUserResponse> FindUser(GetUserByEmailRequest request)
    {
        _logger.LogInformation("Сбор расширенной информации о пользователе: {Email}", request.Email);

        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null) return null;

        var orderResponse = await _orderGrpcService.GetOrdersByUserIdsAsync(
            new GetOrdersByUserIdsRequest { UserIds = { user.Id } });

        var orders = orderResponse.Orders;
    
        var taskResponse = await _taskGrpcService.GetTasksByUserIdsAsync(
            new GetTasksByUserIdsRequest { UserIds = { user.Id } });

        var tasks = taskResponse.Tasks;

        var clientIds = orders
            .Select(o => o.ClientId)
            .Distinct()
            .ToList();

        var clientResponse = await _clientGrpcService.GetClientsByIdsAsync(
            new GetClientsByIdsRequest { Ids = { clientIds } });

        var clientNames = clientResponse.ClientName;

        var response = new FindUserResponse();

        response.Orders.AddRange(orders.Select(o => new FindUserOrdersResponse
        {
            OrderId = o.Id.ToString(),
            TotalAmount = o.TotalAmount,
            Status = o.Status.ToString()
        }));

        response.Tasks.AddRange(tasks.Select(t => new FindUserTasksResponse
        {
            TaskId = t.Id.ToString(),
            Title = t.Title,
            Status = t.Status.ToString()
        }));

        response.Clients.AddRange(clientNames.Select(name => new FindUserClientsResponse
        {
            ClientName = name
        }));

        return response;
    }

    public async Task<GetAllUsersResponse> GetAllUsers(SortUsersRequest sortUsersRequest)
    {
        var users = await _userRepository.GetAllAsync();
        users = sortUsersRequest.SortBy?.ToLower() switch
        {
            "userid" => sortUsersRequest.Descending
                ? users.OrderByDescending(u => u.Id).ToList() 
                : users.OrderBy(u => u.Id).ToList(),
            
            "username" => sortUsersRequest.Descending 
                ? users.OrderByDescending(u => u.Username).ToList() 
                : users.OrderBy(u => u.Username).ToList(),
            
            "email" => sortUsersRequest.Descending 
                ? users.OrderByDescending(u => u.Email).ToList() 
                : users.OrderBy(u => u.Email).ToList(),
            
            "role" => sortUsersRequest.Descending 
                ? users.OrderByDescending(u => u.Role).ToList() 
                : users.OrderBy(u => u.Role).ToList(),
            
            "isemailconfirmed" => sortUsersRequest.Descending 
                ? users.OrderByDescending(u => u.IsEmailConfirmed).ToList() 
                : users.OrderBy(u => u.IsEmailConfirmed).ToList(),
            
            "createdat" => sortUsersRequest.Descending 
                ? users.OrderByDescending(u => u.CreatedAt).ToList() 
                : users.OrderBy(u => u.CreatedAt).ToList(),
            _ => users
        };

        var userIds = users.Select(u => u.Id).ToList();

        var taskResponse = await _taskGrpcService.GetTasksByUserIdsAsync(new GetTasksByUserIdsRequest { UserIds = { userIds } });
        var orderResponse = await _orderGrpcService.GetOrdersByUserIdsAsync(new GetOrdersByUserIdsRequest { UserIds = { userIds } });
        
        var tasksByUser = taskResponse.Tasks.GroupBy(t => t.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());
        var ordersByUser = orderResponse.Orders.GroupBy(o => o.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        var response = new GetAllUsersResponse();

        foreach (var user in users)
        {
            var userInfo = new UserInfo
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                UserRole = (CRMSolution.Grpc.Users.UserRole)user.Role,
                IsEmailConfirmed = user.IsEmailConfirmed,
                CreatedAt = Timestamp.FromDateTime(user.CreatedAt),
            };

            if (tasksByUser.TryGetValue(user.Id, out var userTasks))
            {
                userInfo.Tasks.AddRange(userTasks.Select<TaskWithUserId, TaskInfo>(t => new TaskInfo
                {
                    TaskId = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    TaskStatus = (GrpcTaskStatus)t.Status,
                    DueDate = t.DueDate
                }));

            }

            // добавь заказы
            if (ordersByUser.TryGetValue(user.Id, out var userOrders))
            {
                userInfo.Orders.AddRange(userOrders.Select(o => new OrderInfo
                {
                    OrderId = o.Id,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = (OrderStatus)o.Status
                }));
            }

            response.Users.Add(userInfo);
        }
        

        return response;
    }

    public async Task<User> GetByIdAsync(int userId)
    {
        return await _userRepository.GetById(userId);
    }

    public async Task<GetUsersByIdsResponse> GetUsersByIds(GetUsersByIdsRequest request)
    {
        var ids = request.Ids.ToList();
        
        var users = await _userRepository.GetUsersByIdsAsync(ids);
        var usernames = users.Select(u => u.Username).ToList();

        return new GetUsersByIdsResponse
        {
            Usernames = { usernames } 
        };
    }

    // public async Task<List<ClientWithOrdersAndTasksResponse>> GetClientsWithOrdersAndTasks(HttpContext httpContext)
    // {
    //     var username = await _tokenService.GetNameFromCookies(httpContext);
    //
    //     var orders = await _clientRepository.ClientRep.GetOrdersByUsername(username);
    //
    //     var clients = await _clientRepository.ClientRep.GetClientsByOrdersAsync(orders);
    //
    //     return _mapper.Map<List<ClientWithOrdersAndTasksResponse>>(clients);
    // }
}