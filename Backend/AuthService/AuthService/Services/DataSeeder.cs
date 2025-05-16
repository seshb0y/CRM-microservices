using AuthService.Data;
using Bogus;
using CRMSolution.Data.Models;

namespace CRMSolution.Services;

public class DataSeeder
{
    private readonly AuthDbContext _context;

    public DataSeeder(AuthDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (_context.Users.Any()) return; 

        int userId = 1;
        var userFaker = new Faker<User>()
            // .RuleFor(u => u.Id, f => userId++) 
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.IsEmailConfirmed, f => f.Random.Bool())
            .RuleFor(u => u.Role, f => f.PickRandom<UserRole>())
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(1).ToUniversalTime());

        var users = userFaker.Generate(10);

        _context.Users.AddRange(users);
        _context.SaveChanges();
    }
}
