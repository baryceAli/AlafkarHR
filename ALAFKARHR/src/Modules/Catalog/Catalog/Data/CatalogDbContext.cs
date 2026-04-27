using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

namespace Catalog.Data;
// add-migration CatalogInitial -Project Catalog -StartupProject Api -OutputDir Data/Migrations -Context CatalogDbContext
public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }
    // 🔹 Core Tables
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductPackage> ProductPackages => Set<ProductPackage>();
    public DbSet<Variant> Variants => Set<Variant>();
    public DbSet<ProductSku> ProductSkus => Set<ProductSku>();
    //public DbSet<ProductPricing> ProductPricings => Set<ProductPricing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // 🔥 Schema
        modelBuilder.HasDefaultSchema("Catalog");

        // 🔥 Apply all IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 🔥 Global Conventions (optional but recommended)
        //ApplyGlobalConfigurations(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void ApplyGlobalConfigurations(ModelBuilder modelBuilder)
    {
        // 🔒 Decimal precision globally
        foreach (var property in modelBuilder.Model
                     .GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        // 🔒 Concurrency token (if using RowVersion)
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var rowVersion = entity.FindProperty("RowVersion");
            if (rowVersion != null)
            {
                rowVersion.IsConcurrencyToken = true;
            }
        }
    }
}
