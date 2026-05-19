using CustomersModule.Customers.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CustomersModule.Data;
// add-migration CustomersModuleInitial -Project CustomersModule -StartupProject Api -OutputDir Data/Migrations -Context CustomerDbContext
// update-database -Project CustomersModule -StartupProject Api -Context CustomerDbContext

public class CustomerDbContext:DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options):base(options){}

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Address => Set<Address>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<CustomerGroup> CustomerGroups => Set<CustomerGroup>();
    public DbSet<CustomerPricingProfile> CustomerPricingProfiles => Set<CustomerPricingProfile>();

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
