using Orders.Orders.Models;
using System.Reflection;

namespace Orders.Data;

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<OrderIntake> OrderIntakes => Set<OrderIntake>();
    public DbSet<OrderIntakeLine> OrderIntakeLines => Set<OrderIntakeLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Orders");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
