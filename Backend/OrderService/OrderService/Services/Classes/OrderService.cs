using AutoMapper;
using CRMSolution.Grpc.Client;
using Microsoft.AspNetCore.SignalR;
using OrderService.DTO.Requests;
using OrderService.DTO.Requests.Order;
using OrderService.DTO.Requests.Orders;
using OrderService.DTO.Responses;
using CRMSolution.Grpc.Users;
using CRMSolution.Grpc.Client;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Grpc.Twilio;
using Google.Protobuf.WellKnownTypes;
using OrderService.Data.Models;
using OrderService.Data.Repository.OrderResp;
using OrderService.Hubs;
using OrderService.Services.Interfaces;
using Org.BouncyCastle.Asn1.X509;
using ClientDto = CRMSolution.Grpc.Orders.ClientDto;
using OrderStatus = CRMSolution.Grpc.Orders.OrderStatus;
using TaskDto = CRMSolution.Grpc.Orders.TaskDto;
using UserRole = CRMSolution.Grpc.Orders.UserRole;


namespace OrderService.Services.Classes;

public class OrderService : IOrderService
{
    private readonly IOrderRep _orderRep;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly UserService.UserServiceClient _userGrpcClient;
    private readonly ClientGrpcService.ClientGrpcServiceClient _clientGrpcClient;
    private readonly TaskGrpcService.TaskGrpcServiceClient _taskGrpcService;
    private readonly TwilioGrpcService.TwilioGrpcServiceClient _twilioGrpcService;
    
    public OrderService(IOrderRep orderRep, IMapper mapper, ILogger<OrderService> logger, 
        IHubContext<NotificationHub> notificationHub, UserService.UserServiceClient userGrpcClient,
        ClientGrpcService.ClientGrpcServiceClient clientGrpcClient, TaskGrpcService.TaskGrpcServiceClient taskGrpcService,
        TwilioGrpcService.TwilioGrpcServiceClient twilioGrpcService)
    {
        _orderRep = orderRep;
        _mapper = mapper;
        _logger = logger;
        _notificationHub = notificationHub;
        _userGrpcClient = userGrpcClient;
        _clientGrpcClient = clientGrpcClient;
        _taskGrpcService = taskGrpcService;
        _twilioGrpcService = twilioGrpcService;
    }
    public async Task<Order> GetByIdAsync(int orderId)
    {
        return await _orderRep.GetByIdAsync(orderId);
    }
    

    public async Task<GetOrderFullInfoResponse> GetOrderInfo(GetOrderFullInfoRequest request)
    {
        Order order =  await _orderRep.GetByIdAsync(request.OrderId);
        var grpcTaskRequest = new GetTaskByIdRequest
        {
            Id = order.Id
        };
        var grpcClientRequest = new GetClientByIdRequest
        {
            ClientId = order.ClientId.Value
        };
        var grpcUserRequest = new GetUserByIdRequest
        {
            Id = order.UserId.Value
        };
        
        var grpcTaskResponse = await _taskGrpcService.GetTaskByOrderIdAsync(grpcTaskRequest);
        var grpcClientResponse = await _clientGrpcClient.GetClientByIdAsync(grpcClientRequest);
        var grpcUserResponse = await _userGrpcClient.GetUserByIdAsync(grpcUserRequest);

        var response = new GetOrderFullInfoResponse
        {
            OrderId = order.Id,
            OrderTotalAmount = (double)order.TotalAmount,
            OrderStatus = (OrderStatus)order.Status,

            Client = new ClientDto
            {
                Id = grpcClientResponse.Id,
                Name = grpcClientResponse.Name,
                Email = grpcClientResponse.Email,
                Phone = grpcClientResponse.Phone,
                Address = grpcClientResponse.Address,
                ClientCreatedAt = grpcClientResponse.CreatedAt
            }
        };

        response.Users.Add(new UserDto
        {
            Id = grpcUserResponse.Id,
            Username = grpcUserResponse.Username,
            Email = grpcUserResponse.Email,
            Role =  (UserRole)grpcUserResponse.Role,
            IsEmailConfirmed = grpcUserResponse.IsEmailConfirmed
        });
        
        response.Tasks.AddRange(_mapper.Map<List<TaskDto>>(grpcTaskResponse.Tasks));
        response.CallRecordingUrl.AddRange(order.CallRecord);


        return response;
    }


    public async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request)
    {
        _logger.LogInformation("Создаем заказ. Проверка пользователя через gRPC: {@Request}", request);

        // gRPC-запрос на получение пользователя по Email
        

        Order order = _mapper.Map<Order>(request);
        
        await _orderRep.AddAsync(order);
        await _orderRep.SaveChangesAsync();

        var grpcUserRequest = new GetUserByEmailRequest
        {
            OrderId = order.Id,
            Email = request.UserEmail
        };

        var grpcUserResponse = await _userGrpcClient.GetUserByUsernameAsync(grpcUserRequest);
        
        var lastOrder = _orderRep.GetAllAsync();
        var grpcClientRequest = new GetClientByEmailRequest
        {
            OrderId = order.Id,
            Email = request.ClientEmail
        }; 
        var grpcClientResponse = await _clientGrpcClient.GetClientByEmailAsync(grpcClientRequest);
        
        order.UserId = grpcUserResponse.Id;
        order.ClientId = grpcClientResponse.Id;
        
        await _orderRep.SaveChangesAsync();
        
        var grpcTaskRequest = new CreateTaskRequest
        {
            Title = "First contact",
            Description = "Connect the client",
            DueDate = Timestamp.FromDateTime(DateTime.UtcNow),
            OrderId = order.Id,
        };
        var grpcTaskReponse = await _taskGrpcService.CreateTaskAsync(grpcTaskRequest);
        _logger.LogInformation("Задача создана через gRPC: ");

        return new CreateOrderResponse
        {
            TotalAmount = (double)order.TotalAmount,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            Status = (OrderStatus)order.Status,
            Id = order.Id,
        };
    }

    public async Task SaveCallRecord(SaveCallRecordRequest request)
    {
        var order = await _orderRep.GetByIdAsync(request.OrderId);
        if (order.CallRecord == null)
        {
            order.CallRecord = new List<string>();
        }
        order.CallRecord.Add(request.RecordUrl);
        await _orderRep.SaveChangesAsync();
        
    }

    public async Task<ChangeOrderDataResponse> ChangeDataOrder(ChangeOrderDataRequest request)
    {
        _logger.LogInformation("Изменяем заказ: {@Request}", request);
        Order order = await _orderRep.GetById(request.OrderId);
        
        order = _mapper.Map(request, order);
        _orderRep.Update(order);
        await _orderRep.SaveChangesAsync();
        return new ChangeOrderDataResponse
        {
            Id = order.Id,
            CreatedAt = Timestamp.FromDateTime(order.CreatedAt.ToUniversalTime()),
            TotalAmount = (double)order.TotalAmount,
            Status = (OrderStatus)order.Status,
        };
    }

    public async Task DeleteOrder(DeleteOrderRequest request)
    {
        Order order = await _orderRep.GetByIdAsync(request.OrderId);
        order.IsDeleted = true;
        await _orderRep.SaveChangesAsync();
    }

    public async Task<ChangeResponsibleResponse> ChangeResponsible(ChangeResponsibleRequest request)
    {
        Order order = await _orderRep.GetByIdAsync(request.OrderId);
        order.UserId = request.UserId;
        await _orderRep.SaveChangesAsync();
        return new ChangeResponsibleResponse
        {
            UserId = order.UserId.Value,
            OrderId = order.Id,
        };
    }
    
    public async Task<GetLowInfoOrdersListResponse> GetLowInfoOrdersAsync(SortOrdersRequest sortRequest)
    {
        var orders = await _orderRep.GetAllAsync();
        
        var userIds = orders.Where(o => o.UserId.HasValue).Select(o => o.UserId.Value).Distinct().ToList();
        var grpcUserRequest = new GetUsersByIdsRequest
        {
            Ids = { userIds }
        };
        var grpcUserResponse = await _userGrpcClient.GetUsersByIdsAsync(grpcUserRequest);
        
        var clientIds = orders.Where(o => o.ClientId.HasValue).Select(o => o.ClientId.Value).Distinct().ToList();
        var grpcClientRequest = new GetClientsByIdsRequest
        {
            Ids = { clientIds }
        };
        var grpcClientResponse = await _clientGrpcClient.GetClientsByIdsAsync(grpcClientRequest);
        
        var userMap = userIds
            .Select((id, index) => new { id, username = grpcUserResponse.Usernames.ElementAtOrDefault(index) ?? "Unknown" })
            .ToDictionary(x => x.id, x => x.username);

        var clientMap = clientIds
            .Select((id, index) => new { id, name = grpcClientResponse.ClientName.ElementAtOrDefault(index) ?? "Unknown" })
            .ToDictionary(x => x.id, x => x.name);

        
        var orderList = orders.Select(o => new LowInfoOrder
        {
            Id = o.Id,
            TotalAmount = (double)o.TotalAmount,
            Status = (int)o.Status,
            CreatedAt = Timestamp.FromDateTime(o.CreatedAt.ToUniversalTime()),
            Username = o.UserId.HasValue && userMap.ContainsKey(o.UserId.Value)
                ? userMap[o.UserId.Value]
                : "Unknown",
            ClientName = o.ClientId.HasValue && clientMap.ContainsKey(o.ClientId.Value) 
                ? clientMap[o.ClientId.Value] 
                : "Unknown"
        }).ToList();
        
        orderList = sortRequest.SortBy.ToLower() switch
        {
            "id" => sortRequest.Descending ? orderList.OrderByDescending(x => x.Id).ToList() : orderList.OrderBy(x => x.Id).ToList(),
            "totalamount" => sortRequest.Descending ? orderList.OrderByDescending(x => x.TotalAmount).ToList() : orderList.OrderBy(x => x.TotalAmount).ToList(),
            "status" => sortRequest.Descending ? orderList.OrderByDescending(x => x.Status).ToList() : orderList.OrderBy(x => x.Status).ToList(),
            "createdat" => sortRequest.Descending ? orderList.OrderByDescending(x => x.CreatedAt.Seconds).ToList() : orderList.OrderBy(x => x.CreatedAt.Seconds).ToList(),
            "username" => sortRequest.Descending ? orderList.OrderByDescending(x => x.Username).ToList() : orderList.OrderBy(x => x.Username).ToList(),
            "clientname" => sortRequest.Descending ? orderList.OrderByDescending(x => x.ClientName).ToList() : orderList.OrderBy(x => x.ClientName).ToList(),
            _ => orderList
        };

        return new GetLowInfoOrdersListResponse
        {
            Orders = { orderList }
        };
    }

    public async Task<GetOrdersByUserIdsResponse> GetOrdersByUserIds(GetOrdersByUserIdsRequest request)
    {
        var orders = await _orderRep.GetOrdersByUserIds(request.UserIds.ToList());

        var response = new GetOrdersByUserIdsResponse();
        foreach (var order in orders)
        {
            response.Orders.Add(new OrderWithUserId
            {
                UserId = order.UserId.Value,
                Id = order.Id,
                TotalAmount = (double)order.TotalAmount,
                Status = (OrderStatus)order.Status,
                ClientId = order.ClientId.Value,
                CreatedAt = Timestamp.FromDateTime(order.CreatedAt.ToUniversalTime()),
            });
        }

        return response;
    }

}