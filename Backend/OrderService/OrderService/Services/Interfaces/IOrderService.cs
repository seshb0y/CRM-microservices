using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Twilio;
using OrderService.Data.Models;
using OrderService.DTO.Requests;
using OrderService.DTO.Requests.Order;
using OrderService.DTO.Requests.Orders;
using OrderService.DTO.Responses;

namespace OrderService.Services.Interfaces;

public interface IOrderService
{
    public Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request);
    public Task<ChangeOrderDataResponse> ChangeDataOrder(ChangeOrderDataRequest request);
    public Task DeleteOrder(DeleteOrderRequest request);
    // public Task<OrderResponse> FindOrder(FindOrderRequest request);
    public Task<GetLowInfoOrdersListResponse> GetLowInfoOrdersAsync(SortOrdersRequest sortRequest);
    Task<Order> GetByIdAsync(int orderId);
    Task<GetOrderFullInfoResponse> GetOrderInfo(GetOrderFullInfoRequest request);
    Task<GetOrdersByUserIdsResponse>  GetOrdersByUserIds(GetOrdersByUserIdsRequest request);
    Task<ChangeResponsibleResponse> ChangeResponsible(ChangeResponsibleRequest request);
    Task SaveCallRecord(SaveCallRecordRequest request);
}