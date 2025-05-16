using Bogus;
using OrderService.Data;
using OrderService.Data.Models;

namespace OrderService.Services;

public class DataSeeder
{
    private readonly OrderDbContext _context;

    public DataSeeder(OrderDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (_context.Orders.Any()) return;
        
        int orderId = 1;
        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.TotalAmount, f => f.Finance.Amount(500, 10000))
            .RuleFor(o => o.Status, f => f.PickRandom<OrderStatus>())
            .RuleFor(o => o.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(o => o.IsDeleted, f =>  f.Equals(false))
            .RuleFor(o => o.ClientId, f => f.Random.Int(1, 50))
            .RuleFor(o => o.UserId, f => f.Random.Int(1, 10));

        var orders = orderFaker.Generate(50);
        
        _context.Orders.AddRange(orders);
        _context.SaveChanges(); 
    }
}
