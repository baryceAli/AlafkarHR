
using Organization.Organizations.Models;

namespace Organization.Data;

public class OrganizationDbContext:DbContext
{
    //add-migration OrganizationInitial -Project Organization -StartupProject Api -OutputDir Data/Migrations -Context OrganizationDbContext

    public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options):base(options)
    {
        
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Administration> Administrations => Set<Administration>();
    public DbSet<Department> Departments => Set<Department>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //builder.HasDefaultSchema
        builder.HasDefaultSchema("Organization");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 🔴 Soft Delete Filter
        builder.Entity<Company>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Branch>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Administration>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);

        // 🔴 Multi-Tenant Filter (example)
        // modelBuilder.Entity<Branch>().HasQueryFilter(x => x.TenantId == _tenantId);


        base.OnModelCreating(builder);
    }
}
