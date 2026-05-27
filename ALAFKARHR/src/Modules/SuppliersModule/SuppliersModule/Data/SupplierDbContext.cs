using System.Reflection;
using SuppliersModule.Suppliers.Models;

namespace SuppliersModule.Data;
// add-migration SuppliersModuleInitial -Project SuppliersModule -StartupProject Api -OutputDir Data/Migrations -Context SupplierDbContext
// update-database -Project SuppliersModule -StartupProject Api -Context SupplierDbContext

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
