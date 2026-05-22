using Microsoft.EntityFrameworkCore;
using SalesOrder.Orders.Models;

//using SalesOrder.Orders.Models;
using System.Reflection;

namespace SalesOrder.Data;

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
