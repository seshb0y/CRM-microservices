using Bogus;
using ClientService.Data;
using ClientService.Data.Models;

namespace ClientService.Services;

public class DataSeeder
{
    private readonly ClientDbContext _context;

    public DataSeeder(ClientDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (_context.Clients.Any()) return;

        var clientFaker = new Faker<Client>()
            // .RuleFor(c => c.Id, f => clientId++)
            .RuleFor(c => c.Name, f => f.Company.CompanyName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("+# (###) ###-##-##"))
            .RuleFor(c => c.Address, f => f.Address.FullAddress())
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
            .RuleFor(c => c.OrderId, f => f.Random.Int(1, 50));

        var clients = clientFaker.Generate(50);
        _context.AddRange(clients);
        _context.SaveChanges();
    }
}
