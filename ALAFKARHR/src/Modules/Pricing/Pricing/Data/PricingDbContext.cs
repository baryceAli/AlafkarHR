using System.Reflection;

namespace Pricing.Data;

// add-migration PricingInitial -Project Pricing -StartupProject Api -OutputDir Data/Migrations -Context PricingDbContext
// update-database -Project Pricing -StartupProject Api -Context PricingDbContext

public class PricingDbContext : DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options) { }

    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Pricing");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
