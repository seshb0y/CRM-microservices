using Bogus;
using TaskService.Data;
using TaskService.Data.Models;

namespace TaskService.Services;

public class DataSeeder
{
    private readonly TaskDbContext _context;

    public DataSeeder(TaskDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (_context.Tasks.Any()) return;

        var taskFaker = new Faker<TaskEntity>()
            .RuleFor(t => t.Title, f => f.Lorem.Sentence(3))
            .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
            .RuleFor(t => t.Status, f => f.Random.Enum<TasksStatus>())
            .RuleFor(t => t.DueDate, f => f.Date.Future(2).ToUniversalTime())
            .RuleFor(t => t.OrderId, f => f.Random.Int(1, 50))
            .RuleFor(t => t.UserId, f => f.Random.Int(1, 10));

        var tasks = taskFaker.Generate(100);
        _context.Tasks.AddRange(tasks);
        _context.SaveChanges();
    }
}
