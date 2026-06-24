
using Organization.Organizations.Models;

namespace Organization.Data;

public class OrganizationDbContext:DbContext
{
    //add-migration OrganizationInitial -Project Organization -StartupProject Api -OutputDir Data/Migrations -Context OrganizationDbContext
    //update-database -Project Organization -StartupProject Api -Context OrganizationDbContext

    public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options):base(options)
    {
        
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<BusinessLine> BusinessLines => Set<BusinessLine>();
    public DbSet<BusinessLineActivation> BusinessLineActivations => Set<BusinessLineActivation>();
    public DbSet<CompanyLicense> CompanyLicenses => Set<CompanyLicense>();
    public DbSet<CompanyLicenseBusinessLine> CompanyLicenseBusinessLines => Set<CompanyLicenseBusinessLine>();
    public DbSet<LicenseCategory> LicenseCategories => Set<LicenseCategory>();
    public DbSet<LicenseCategoryBusinessLine> LicenseCategoryBusinessLines => Set<LicenseCategoryBusinessLine>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<UserBranchAssignment> UserBranchAssignments => Set<UserBranchAssignment>();
    public DbSet<UserBranchRoleAssignment> UserBranchRoleAssignments => Set<UserBranchRoleAssignment>();
    public DbSet<Administration> Administrations => Set<Administration>();
    public DbSet<Department> Departments => Set<Department>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //builder.HasDefaultSchema
        builder.HasDefaultSchema("Organization");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 🔴 Soft Delete Filter
        builder.Entity<Company>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<BusinessLine>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<CompanyLicense>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<LicenseCategory>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Branch>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<UserBranchAssignment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<UserBranchRoleAssignment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Administration>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);

        // 🔴 Multi-Tenant Filter (example)
        // modelBuilder.Entity<Branch>().HasQueryFilter(x => x.TenantId == _tenantId);


        base.OnModelCreating(builder);
    }
}
