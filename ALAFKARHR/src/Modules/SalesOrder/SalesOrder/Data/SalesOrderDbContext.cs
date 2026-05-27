using SalesOrder.Orders.Models;
using System.Reflection;

namespace SalesOrder.Data;
// add-migration SalesOrderInitial -Project SalesOrder -StartupProject Api -OutputDir Data/Migrations -Context SalesOrderDbContext
// update-database -Project SalesOrder -StartupProject Api -Context SalesOrderDbContext

public class SalesOrderDbContext:DbContext
{
    public SalesOrderDbContext(DbContextOptions<SalesOrderDbContext> options):base(options)
    {
        
    }

    public DbSet<Orders.Models.SalesOrder> SalesOrders => Set<Orders.Models.SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines=> Set<SalesOrderLine>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("SalesOrder");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
