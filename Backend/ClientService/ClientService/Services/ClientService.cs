using AutoMapper;
using ClientService.Data.Models;
using ClientService.Data.Repository.SpecialRepClass.ClientRep;
using ClientService.DTO.Requests.Client;
using ClientService.DTO.Responses;
using ClientService.Hubs;
using ClientService.Services.Interfaces;
using CRMSolution.DTO.Requests.Client;
using CRMSolution.Grpc.Client;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Grpc.Users;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR;
using TaskDto = CRMSolution.Grpc.Client.TaskDto;
using CRMSolution.Grpc.Tasks;


namespace ClientService.Services.Classes;

public class ClientService : IClientService
{
    private readonly IClientRep _clientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ClientService> _logger;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly UserService.UserServiceClient _userGrpcClient;
    private readonly OrderGrpcService.OrderGrpcServiceClient _orderGrpcClient;
    private readonly TaskGrpcService.TaskGrpcServiceClient _taskGrpcClient;
    
    public ClientService(IClientRep clientRepository, IMapper mapper, ILogger<ClientService> logger, IHubContext<NotificationHub> hubContext
    , UserService.UserServiceClient userGrpcClient, OrderGrpcService.OrderGrpcServiceClient orderGrpcClient, TaskGrpcService.TaskGrpcServiceClient taskGrpcClient)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
        _logger = logger;
        _hubContext = hubContext;
        _userGrpcClient = userGrpcClient;
        _orderGrpcClient = orderGrpcClient;
        _taskGrpcClient = taskGrpcClient;
        
    }
    
    public async Task<CreateClientResponse> CreateClient(CreateClientRequest request)
    {
        _logger.LogInformation("Создаем нового клиента: {@Request}", request);
        Client client = _mapper.Map<Client>(request);
        await _clientRepository.AddAsync(client);
        await _clientRepository.SaveChangesAsync();
        _logger.LogInformation("Отправка сигнала ClientCreated");
        return new CreateClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            Address = client.Address,
            Email = client.Email,
            Phone =  client.Phone,
            CreatedAt = Timestamp.FromDateTime(client.CreatedAt.ToUniversalTime())
        };
    }

    public async Task<ChangeDataClientResponse> ChangeDataClient(ChangeDataClientRequest request)
    {
        _logger.LogInformation("Изменяем данные клиента: {@Request}", request);
        Client client = await _clientRepository.GetClientByEmail(request.OldEmail);
        if (client == null)
        {
            throw new KeyNotFoundException($"Client with email {request.OldEmail} not found");
        }
        client = _mapper.Map(request, client);
        _clientRepository.Update(client);
        await _clientRepository.SaveChangesAsync();

        return new ChangeDataClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address,
        };
    }
    
    public async Task<DeleteClientResponse> DeleteClient(DeleteClientRequest request)
    {
        _logger.LogInformation("Удаляем клиента: {@Request}", request);
        Client client = await _clientRepository.GetClientByEmail(request.Email);
        if (client == null)
        {
            throw new KeyNotFoundException($"Client with email {request.Email} not found");
        }
        _clientRepository.Delete(client);
        await _hubContext.Clients.All.SendAsync("ClientDeleted", new
        {
            client.Id
        });
        await _clientRepository.SaveChangesAsync();
        return new DeleteClientResponse
        {
            Id = client.Id,
        };
    }

    public async Task<GetClientResponse> FindClient(GetClientByEmailRequest request)
    {
        _logger.LogInformation("Поиск клиента: {@Request}", request);
        HttpFindClientResponse? client = await _clientRepository.GetClientsOrdersAndUsersAsync(request.Email);
        if (client == null)
        {
            _logger.LogWarning("Клиент с email {Email} не найден",request.Email);
            throw new KeyNotFoundException($"Client with email {request.Email} not found");
        }
        _logger.LogInformation("Клиент найден: {ClientId}", request.Email);
        return _mapper.Map<GetClientResponse>(client);
    }

    public async Task<GetAllClientsResponse> GetAllClients(GetAllClientsRequest getAllClientsRequest)
    {
        var tasks = (await _clientRepository.GetAllAsync()).ToList();
        
        var grpcClient = tasks.Select(t => new ClientInfo
        {
            Id = t.Id,
            Name = t.Name,
            Email = t.Email,
            Phone = t.Phone,
            Address = t.Address,
            CreatedAt = Timestamp.FromDateTime(t.CreatedAt.ToUniversalTime()),
            OrderId = t.OrderId ?? 0
        }).ToList();
        
        grpcClient = getAllClientsRequest.Sort.SortBy.ToLower() switch
        {
            "id" => getAllClientsRequest.Sort.Descending
                ? grpcClient.OrderByDescending(x => x.Id).ToList()
                : grpcClient.OrderBy(x => x.Id).ToList(),

            "name" => getAllClientsRequest.Sort.Descending
                ? grpcClient.OrderByDescending(x => x.Name).ToList()
                : grpcClient.OrderBy(x => x.Name).ToList(),

            "email" => getAllClientsRequest.Sort.Descending
                ? grpcClient.OrderByDescending(x => x.Email).ToList()
                : grpcClient.OrderBy(x => x.Email).ToList(),
            
            "address" => getAllClientsRequest.Sort.Descending
                ? grpcClient.OrderByDescending(x => x.Address).ToList()
                : grpcClient.OrderBy(x => x.Address).ToList(),
            
            "phone" => getAllClientsRequest.Sort.Descending
                ? grpcClient.OrderByDescending(x => x.Phone).ToList()
                : grpcClient.OrderBy(x => x.Phone).ToList(), 

            "createdat" => getAllClientsRequest.Sort.Descending
                ? grpcClient.OrderByDescending(x => x.CreatedAt.Seconds).ToList()
                : grpcClient.OrderBy(x => x.CreatedAt.Seconds).ToList(),

            _ => grpcClient
        };
        
        return new GetAllClientsResponse
        {
            Clients = { grpcClient }
        };
    }
    
    // public async Task<List<ClientWithOrdersAndTasksResponse>> GetClientsWithOrdersAndTasks(HttpContext httpContext)
    // {
    //     var username = await _tokenService.GetNameFromCookies(httpContext);
    //
    //     var orders = await _clientRepository.ClientRep.GetOrdersByUsername(username);
    //     
    //     orders = orders.Select(o => new Order
    //     {
    //         Id = o.Id,
    //         TotalAmount = o.TotalAmount,
    //         Status = o.Status,
    //         CreatedAt = o.CreatedAt,
    //         Tasks = o.Tasks
    //     }).ToList();
    //
    //     var clients = await _clientRepository.ClientRep.GetClientsByOrdersAsync(orders);
    //
    //     return _mapper.Map<List<ClientWithOrdersAndTasksResponse>>(clients);
    // }

    public async Task<Client> GetByEmailAsync(GetClientByEmailRequest request)
    {
        Client client = await _clientRepository.GetClientByEmail(request.Email);
        if (request.OrderId != 0)
        {
            client.OrderId = request.OrderId;
        }
        await _clientRepository.SaveChangesAsync();
        return client;
    }

    public async Task<Client> GetByIdAsync(GetClientByIdRequest request)
    {
        return await _clientRepository.GetById(request.ClientId);
    }

    public async Task<GetClientsByIdsResponse> GetClientsByIds(GetClientsByIdsRequest request)
    {
        var ids = request.Ids.ToList();
        
        var clients = await _clientRepository.GetClientsByIdsAsync(ids);
        var names = clients.Select(u => u.Name).ToList();

        return new GetClientsByIdsResponse
        {
            ClientName = { names }
        };
    }

    public async Task<GetDashboardDataResponse> GetDashboardData(GetDashboardDataRequest request)
    {
        List<Client> clients = new List<Client>();
        clients.AddRange(await _clientRepository.GetAllAsync());
        var orders = await _orderGrpcClient.GetLowInfoOrdersListAsync(new GetLowInfoOrdersListRequest
            { Sort = new SortOrdersRequest { SortBy = "", Descending = true } });
        var tasks = await _taskGrpcClient.GetAllTasksAsync(new GetAllTasksRequest{Sort =  new SortTasksRequest() { SortBy = "", Descending = true }});

        var ordersTotalAmount = orders.Orders.Sum(o => o.TotalAmount);
        var ordersCreatedDates = orders.Orders.Select(o => o.CreatedAt).ToList();
        var taskStatuses = tasks.Tasks.Select(t => t.Status).ToList();
        var response =  new GetDashboardDataResponse
        {
            ClientsAmount = clients.Count,
            OrdersTotalAmount = ordersTotalAmount,
            OrdersCount = orders.Orders.Count,
            TasksCount = tasks.Tasks.Count
        };
        response.OrdersCreatedDates.AddRange(orders.Orders.Select(o => o.CreatedAt));

        response.TasksStatuses.AddRange(
            taskStatuses.Select(s => (CRMSolution.Grpc.Client.GrpcTaskStatus)(int)s)
        );
        
        return response;
    }
    public async Task<GetClientsWithOrdersAndTasksResponse> GetClientsWithOrdersAndTasksAsync(string httpContext)
    {
        var token = httpContext;
        var username = (await _userGrpcClient.GetNameFromTokenAsync(new GetNameFromTokenRequest { Token = token })).Username;
        var user = await _userGrpcClient.GetUserByUsernameAsync(new GetUserByEmailRequest { Email = username });

        var ordersResponse = await _orderGrpcClient.GetOrdersByUserIdsAsync(
            new GetOrdersByUserIdsRequest { UserIds = { user.Id } });

        var orders = ordersResponse.Orders;
        var orderIds = orders.Select(o => o.Id).ToList();
        var clientIds = orders.Select(o => o.ClientId).Distinct().ToList();

        var tasksResponse = await _taskGrpcClient.GetTasksByOrderIdsAsync(
            new GetTasksByOrderIdsRequest { OrderIds = { orderIds } });

        var tasks = tasksResponse.Tasks;
        var clients = await _clientRepository.GetClientsByIdsAsync(clientIds);

        var response = new GetClientsWithOrdersAndTasksResponse();

        response.Clients.AddRange(clients.Select(client =>
        {
            var clientOrders = orders.Where(o => o.ClientId == client.Id).ToList();

            return new ClientWithOrdersAndTasks
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address,
                CreatedAt = Timestamp.FromDateTime(client.CreatedAt.ToUniversalTime()),
                Orders = {
                    clientOrders.Select(order => new KanbanOrder
                    {
                        Id = order.Id,
                        TotalAmount = order.TotalAmount,
                        Status = order.Status.ToString(),
                        CreatedAt = Timestamp.FromDateTime(order.CreatedAt.ToDateTime()),
                        Tasks = {
                            tasks.Where(t => t.OrderId == order.Id).Select(t => new TaskDto
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                Status = t.Status.ToString(),
                                DueDate = t.DueDate
                            })
                        }
                    })
                }
            };
        }));

        return response;

    }
    
}