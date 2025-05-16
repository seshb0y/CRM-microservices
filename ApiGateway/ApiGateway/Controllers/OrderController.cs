using ApiGateway.DTO.Requests;
using ApiGateway.DTO.Requests.Order;
using ApiGateway.DTO.Requests.Orders;
using ApiGateway.DTO.Responses;
using ApiGateway.Hubs;
using AutoMapper;
using CRMSolution.DTO.Requests;
using CRMSolution.Grpc.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using HttpChangeOrderDataRequest = CRMSolution.Grpc.Orders.ChangeOrderDataRequest;
using HttpDeleteOrderRequest = CRMSolution.Grpc.Orders.DeleteOrderRequest;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/v1/orders/")]
public class OrderController : ControllerBase
{
    private readonly OrderGrpcService.OrderGrpcServiceClient _orderGrpcService;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificationHub> _notificationHubContext;

    public OrderController(OrderGrpcService.OrderGrpcServiceClient orderGrpcService,  IMapper mapper,  IHubContext<NotificationHub> notificationHubContext)
    {
        _orderGrpcService = orderGrpcService;
        _mapper = mapper;
        _notificationHubContext = notificationHubContext;
    }


    [HttpPost]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> AddOrder([FromBody] HttpCreateOrderRequest request)
    {
        var grpcRequest = new CreateOrderRequest
        {
            UserEmail = request.userEmail,
            ClientEmail = request.clientEmail,
            TotalAmount = (double)request.totalAmount,
        };
        var grpcResponse = await _orderGrpcService.CreateOrderAsync(grpcRequest);
        
        await _notificationHubContext.Clients.All.SendAsync("OrderCreated", new
        {
            grpcResponse.Id,
            grpcResponse.CreatedAt,
            grpcResponse.TotalAmount,
            grpcResponse.Status,
        });
        return Ok("order created");
    }
    
    [HttpPut]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> ChangeOrder([FromBody] HttpChangeOrderDataRequest request)
    {
        var grpcRequest = new ChangeOrderDataRequest
        {
            OrderId = request.OrderId,
            TotalAmount = (double)request.TotalAmount,
            Status = (OrderStatus)request.Status,
        };
        var grpcResponse = await _orderGrpcService.ChangeOrderDataAsync(grpcRequest);
        
        await _notificationHubContext.Clients.All.SendAsync("OrderUpdated", new
        {
            grpcResponse.Id,
            grpcResponse.CreatedAt,
            grpcResponse.TotalAmount,
            grpcResponse.Status,
        });
        return Ok("order updated");
    }
    
    [HttpDelete]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> DeleteOrder([FromBody] HttpDeleteOrderRequest request)
    {
        var grpcRequest = new DeleteOrderRequest
        {
            OrderId = request.OrderId
        };
        var grpcResponse = await _orderGrpcService.DeleteOrderAsync(grpcRequest);
        
        await _notificationHubContext.Clients.All.SendAsync("OrderDeleted", new
        {
            request.OrderId,
        });
        return Ok("Order deleted");
    }
    
    // [HttpGet("find/order")]
    // // [Authorize(Policy = "ManagerPolicy")]
    // public async Task<IActionResult> FindOrder([FromQuery] FindOrderRequest request)
    // {
    //     return Ok(await _orderService.FindOrder(request));
    // }
    
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        var grpcRequest = new GetOrderFullInfoRequest
        {
            OrderId = orderId
        };
        var grpcResponse = _orderGrpcService.GetOrderFullInfo(grpcRequest);
        var response = _mapper.Map<OrderDetailsResponse>(grpcResponse);

        return Ok(response);
    }
    
    [HttpPut("{orderId}/user")]
    public async Task<IActionResult> ChangeResponsible(int orderId, HttpChangeResponsibleRequest request)
    {
        var grpcRequest = new ChangeResponsibleRequest
        {
            OrderId = orderId,
            UserId = request.userId,
        };
        var grpcResponse = await _orderGrpcService.ChangeResponsibleAsync(grpcRequest);
        
        await _notificationHubContext.Clients.All.SendAsync("ResponsibleUpdated", new
        {
            userId =  grpcResponse.UserId,
            orderId = grpcResponse.OrderId,
        });
        
        return Ok("Responsible changed");
    }
    
    
    
    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] HttpSortOrdersRequest sortOrdersRequest)
    {
        var grpcRequest = new GetLowInfoOrdersListRequest
        {
            Sort = new SortOrdersRequest
            {
                SortBy = sortOrdersRequest.sortBy,
                Descending = sortOrdersRequest.Descending,
            }
        };
        
        var grpcResponse = await _orderGrpcService.GetLowInfoOrdersListAsync(grpcRequest);

        var response = new GetAllOrdersResponse
        {
            Orders = _mapper.Map<List<OrderDTO>>(grpcResponse.Orders)
        };
        return Ok(response);
    }
    
    // [HttpGet("load/data")]
    // // [Authorize(Policy = "ManagerPolicy")]
    // public async Task<IActionResult> LoadClientData([FromQuery] FindOrderRequest request)
    // {
    //     return Ok(await _orderService.FindOrder(request));
    // }
}