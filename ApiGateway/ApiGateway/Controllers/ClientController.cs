using ApiGateway.DTO.Requests.Client;
using ApiGateway.DTO.Responses;
using ApiGateway.Hubs;
using AutoMapper;
using CRMSolution.Grpc.Client;
using FluentValidation;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using KanbanOrderResponse = ApiGateway.DTO.Responses.KanbanOrderResponse;
using TaskResponse = ApiGateway.DTO.Responses.TaskResponse;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/v1/clients/")]
public class ClientController : ControllerBase
{
    private readonly ClientGrpcService.ClientGrpcServiceClient _clientService;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IMapper _mapper;

    public ClientController(ClientGrpcService.ClientGrpcServiceClient clientService, IHubContext<NotificationHub> hubContext,
        IMapper mapper)
    {
        _clientService = clientService;
        _hubContext = hubContext;
        _mapper = mapper;
    }


    [HttpPost]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> AddClient([FromBody] HttpCreateClientRequest request)
    {
        var grpcRequest = new CreateClientRequest
        {
            Name = request.name,
            Address = request.address,
            Email = request.email,
            Phone = request.phone,
        };
        var grpcResponse = await _clientService.CreateClientAsync(grpcRequest);
        
        _hubContext.Clients.All.SendAsync("ClientCreated", new
        {
            grpcResponse.Id,
            grpcResponse.Name,
            grpcResponse.Email,
            grpcResponse.Phone,
            grpcResponse.Address,
            CreatedAt = grpcResponse.CreatedAt.ToDateTime()
        });
        return Ok(grpcResponse);
    }
    
    [HttpPut]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> ChangeClient([FromBody] HttpChangeDataClientRequest request)
    {
        // var validationResult = await validator.ValidateAsync(request);
        // if (!validationResult.IsValid)
        // {
        //     return BadRequest(validationResult.Errors);
        // }
        var grpcRequest = new ChangeDataClientRequest
        {
            Name = request.name,
            Address = request.address,
            Email = request.newEmail,
            OldEmail = request.oldEmail,
            Phone = request.phone,
        };
        var grpcResponse = await _clientService.ChangeDataClientAsync(grpcRequest);
        await _hubContext.Clients.All.SendAsync("ClientUpdated", new
        {
            grpcResponse.Id,
            grpcResponse.Name,
            grpcResponse.Email,
            grpcResponse.Phone,
            grpcResponse.Address,
        });
        return Ok(grpcResponse);
    }
    
    [HttpDelete]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> DeleteClient([FromBody] HttpDeleteClientRequest request)
    {
        var grpcRequest = new DeleteClientRequest
        {
            Email = request.email,
        };
        var grpcResponse = await _clientService.DeleteClientAsync(grpcRequest);
        
        await _hubContext.Clients.All.SendAsync("ClientDeleted", new
        {
            grpcResponse.Id
        });
        return Ok("Client deleted");
    }
    
    [HttpGet("search")]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> LoadClientData([FromQuery] HttpFindClientRequest request)
    {
        var grpcRequest = new GetClientByEmailRequest
        {
            Email = request.email,
            OrderId = 0
        };
        var grpcResponse = await _clientService.GetClientByEmailAsync(grpcRequest);
        return Ok(grpcResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClients([FromQuery] HttpSortClientsRequest sortClientsRequest)
    {
        var grpcRequest = new GetAllClientsRequest
        {
            Sort = new SortClientRequest
            {
                SortBy = sortClientsRequest.sortBy,
                Descending = sortClientsRequest.Descending
            }
        };
        var grpcResponse = await _clientService.GetAllClientsAsync(grpcRequest);
        var httpResponse = _mapper.Map<HttpGetAllClientsResponse>(grpcResponse);
        return Ok(httpResponse);
    }
    
    [HttpGet("relations")]
    public async Task<IActionResult> GetClientsWithOrdersAndTasks()
    {
        string? accessToken = Request.Cookies["accessToken"];
        var metadata = new Metadata { { "authorization", accessToken } };

        var grpcRequest = new GetClientWithOrdersAndTasksRequest();
        var grpcResponse = await _clientService.GetClientsWithOrdersAndTasksAsync(grpcRequest, metadata);

        var result = grpcResponse.Clients.Select(c => new ClientWithOrdersAndTasksResponse()
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            CreatedAt = c.CreatedAt.ToDateTime(),
            Orders = c.Orders.Select(o => new KanbanOrderResponse
            {
                OrderId = o.Id,
                TotalAmount = (decimal)o.TotalAmount,
                OrderStatus = o.Status,
                CreatedAt = o.CreatedAt.ToDateTime(),
                Tasks = o.Tasks.Select(t => new KanbanTaskResponse()
                {
                    Taskid = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    DueDate = t.DueDate.ToDateTime()
                }).ToList()
            }).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var grpcResponse = _clientService.GetDashboardData(new GetDashboardDataRequest());
        var response = new DashboardResponse
        {
            ClientsAmount = grpcResponse.ClientsAmount,
            OrdersCount = grpcResponse.OrdersCount,
            OrdersCreatedDates = grpcResponse.OrdersCreatedDates.Select(g => g.ToDateTime()).ToList(),
            OrdersTotalAmount = (decimal)grpcResponse.OrdersTotalAmount,
            TasksCount = grpcResponse.TasksCount,
            TasksStatuses = grpcResponse.TasksStatuses.Select(s => (CRMSolution.Grpc.Tasks.GrpcTaskStatus)s).ToList()
        };
        return Ok(response);
    }


}