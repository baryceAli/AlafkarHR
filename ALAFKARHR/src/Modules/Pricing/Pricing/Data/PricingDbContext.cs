using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Pricing.Data;

public class PricingDbContext:DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options):base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 🔥 Schema
        modelBuilder.HasDefaultSchema("Pricing");

        // 🔥 Apply all IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 🔥 Global Conventions (optional but recommended)
        //ApplyGlobalConfigurations(modelBuilder);

        base.OnModelCreating(modelBuilder);

    }
}
