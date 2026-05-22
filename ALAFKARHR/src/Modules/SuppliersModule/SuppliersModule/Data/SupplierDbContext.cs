using System.Reflection;
using SuppliersModule.Suppliers.Models;

namespace SuppliersModule.Data;

public class SupplierDbContext : DbContext
{
    public SupplierDbContext(DbContextOptions<SupplierDbContext> options) : base(options)
    {
    }

    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierGroup> SupplierGroups => Set<SupplierGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Supplier");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
