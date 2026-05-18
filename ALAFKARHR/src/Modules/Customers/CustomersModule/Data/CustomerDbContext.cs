using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CustomersModule.Data;

public class CustomerDbContext:DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options):base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 🔥 Schema
        modelBuilder.HasDefaultSchema("Customer");

        // 🔥 Apply all IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 🔥 Global Conventions (optional but recommended)
        //ApplyGlobalConfigurations(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
