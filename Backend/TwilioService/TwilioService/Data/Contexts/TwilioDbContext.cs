using Microsoft.EntityFrameworkCore;
using TwilioService.Data.Model;

namespace TaskService.Data;

public class TwilioDbContext : DbContext
{
    public TwilioDbContext(DbContextOptions<TwilioDbContext> options) : base(options) {}
    public DbSet<TwilioService.Data.Model.Twilio> Twilio { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TwilioDbContext).Assembly); 
    }
}