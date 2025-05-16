using AutoMapper;
using CRMSolution.Grpc.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTO.Requests;
using OrderService.DTO.Requests.Order;
using OrderService.Services.Interfaces;
using OrderService.DTO.Requests.Orders;

namespace OrderService.Controllers;

[ApiController]
[Route("api/v1/orders/")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }


    [HttpPost]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> AddOrder([FromBody] CreateOrderRequest request)
    {
        await _orderService.CreateOrder(request);
        return Ok("Order created");
    }
    
    [HttpPut]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> ChangeOrder([FromBody] HttpChangeOrderDataRequest request)
    {
        // await _orderService.ChangeDataOrder(request);
        return Ok("Order changed");
    }
    
    [HttpDelete]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> DeleteOrder([FromBody] HttpDeleteOrderRequest request)
    {
        // await _orderService.DeleteOrder(request);
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
        // var orderDetails = await _orderService.GetOrderAsync(orderId);

        return Ok("orderDetails");
    }
    
    [HttpPut("{orderId}/user")]
    public async Task<IActionResult> ChangeResponsible(int orderId, HttpChangeResponsibleRequest request)
    {
        // await _orderService.ChangeResponsible(orderId, request);
        return Ok("Responsible changed");
    }
    
    
    
    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] SortOrdersRequest sortOrdersRequest)
    {
        // var orders = await _orderService.GetAllOrders(sortOrdersRequest);
        return Ok("orders");
    }
    
    // [HttpGet("load/data")]
    // // [Authorize(Policy = "ManagerPolicy")]
    // public async Task<IActionResult> LoadClientData([FromQuery] FindOrderRequest request)
    // {
    //     return Ok(await _orderService.FindOrder(request));
    // }
}