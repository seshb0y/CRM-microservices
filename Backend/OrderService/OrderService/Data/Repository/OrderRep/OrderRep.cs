
using Microsoft.EntityFrameworkCore;
using OrderService.Data.Models;
using OrderService.Data.Repository.Interface;
using OrderService.Data;

namespace OrderService.Data.Repository.OrderResp;

public class OrderRep : Repository<Order>, IOrderRep
{
    public OrderRep(OrderDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        return order;
    }

    public async Task<List<Order>> GetOrdersByUserIds(List<int> userIds)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId.HasValue && userIds.Contains(o.UserId.Value))
            .ToListAsync();

        return orders;
    }

    // public async Task AddOrderToClientAndUser(Client client, Order order, User user)
    // {
    //     order.ClientOrders.Add(
    //         new ClientOrder
    //         {
    //             OrderId = order.Id,
    //             ClientId = client.Id,
    //             Order = order,
    //             Client = client,
    //         });
    //     order.UserOrders.Add(
    //         new UserOrders
    //         {
    //             OrderId = order.Id,
    //             UserId = user.Id,
    //             Order = order,
    //             User = user,
    //         });
    //     
    //     await _context.SaveChangesAsync();
    // }

    // public async Task<Order> GetOrderInclude(int orderId)
    // {
    //     return await _dbSet
    //         .Include(o => o.ClientOrders)
    //         .ThenInclude(o => o.Client)
    //         .Include(o => o.Tasks)
    //         .Include(o => o.UserOrders)
    //         .ThenInclude(o => o.User)
    //         .FirstOrDefaultAsync(o => o.Id == orderId);
    // }
    
    // public async Task<Order> GetOrderWithClientAndTasks(int orderId)
    // {
    //     return await _context.Orders
    //         .Include(o => o.ClientOrders)
    //         .ThenInclude(co => co.Client)
    //         .Include(o => o.Tasks)
    //         .Include(o => o.UserOrders)
    //         .ThenInclude(uo => uo.User)
    //         .Include(o => o.CallRecordings)
    //         .FirstOrDefaultAsync(o => o.Id == orderId);
    // }
    //
    //
    //
    // public async Task<List<OrderDTO>> GetLowInfoOrdersList(SortOrdersRequest sortOrdersRequest)
    // {
    //     var query = _dbSet
    //         .Include(o => o.UserOrders)
    //         .ThenInclude(uo => uo.User)
    //         .Include(co => co.ClientOrders)
    //         .ThenInclude(co => co.Client)
    //         .Select(o => new OrderDTO()
    //         {
    //             Id = o.Id,
    //             TotalAmount = o.TotalAmount,
    //             Status = o.Status,
    //             CreatedAt = o.CreatedAt,
    //             Username = o.UserOrders.Any() ? o.UserOrders.First().User.Username : "No user",
    //             ClientName = o.ClientOrders.Any() ? o.ClientOrders.First().Client.Name : "No client",
    //         });
    //
    //     query = sortOrdersRequest.sortBy?.ToLower() switch
    //     {
    //         "id" => sortOrdersRequest.Descending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
    //         "totalamount" => sortOrdersRequest.Descending ? query.OrderByDescending(c => c.TotalAmount) : query.OrderBy(c => c.TotalAmount),
    //         "status" => sortOrdersRequest.Descending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),
    //         "createdat" => sortOrdersRequest.Descending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
    //         "username" => sortOrdersRequest.Descending ? query.OrderByDescending(c => c.Username) : query.OrderBy(c => c.Username),
    //         "clientname" => sortOrdersRequest.Descending ? query.OrderByDescending(c => c.ClientName) : query.OrderBy(c => c.ClientName),
    //         _ => query
    //     };
    //
    //     return await query.ToListAsync();
    // }

    
    
    
}