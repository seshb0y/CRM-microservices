

using CRMSolution.Grpc.Orders;
using OrderService.Data.Models;
using OrderService.Data.Repository.Interface;

namespace OrderService.Data.Repository.OrderResp;

public interface IOrderRep : IRepository<Order>
{
    // public Task AddOrderToClientAndUser(Client client, Order order, User user);
    // public Task<Order> GetOrderInclude(int orderId);
    // public Task<Order> GetOrderWithClientAndTasks(int orderId);
    // public Task<List<OrderDTO>> GetLowInfoOrdersList(SortOrdersRequest sortOrdersRequest);
    public Task<Order> GetByIdAsync(int orderId);
    public Task<List<Order>> GetOrdersByUserIds(List<int>  userIds);
}