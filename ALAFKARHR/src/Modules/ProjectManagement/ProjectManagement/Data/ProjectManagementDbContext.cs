using System.Reflection;

namespace ProjectManagement.Data;

public class ProjectManagementDbContext(DbContextOptions<ProjectManagementDbContext> options) : DbContext(options)
{
    //add-migration ProjectManagementInitial -Project ProjectManagement -StartupProject Api -OutputDir Data/Migrations -Context ProjectManagementDbContext
    //update-database -Project ProjectManagement -StartupProject Api -Context ProjectManagementDbContext

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectCustomer> ProjectCustomers => Set<ProjectCustomer>();
    public DbSet<ProjectCustomerProductPlan> ProjectCustomerProductPlans => Set<ProjectCustomerProductPlan>();
    public DbSet<ProjectDeliverable> ProjectDeliverables => Set<ProjectDeliverable>();
    public DbSet<DistributionPlace> DistributionPlaces => Set<DistributionPlace>();
    public DbSet<ProjectDistributionSchedule> ProjectDistributionSchedules => Set<ProjectDistributionSchedule>();
    public DbSet<ProjectDistributionAllocation> ProjectDistributionAllocations => Set<ProjectDistributionAllocation>();
    public DbSet<ProjectMaterialRequirement> ProjectMaterialRequirements => Set<ProjectMaterialRequirement>();
    public DbSet<ProjectResource> ProjectResources => Set<ProjectResource>();
    public DbSet<ProjectExpense> ProjectExpenses => Set<ProjectExpense>();
    public DbSet<ProjectHandoff> ProjectHandoffs => Set<ProjectHandoff>();
    public DbSet<ProjectHandoffLine> ProjectHandoffLines => Set<ProjectHandoffLine>();
    public DbSet<ProjectTaskLink> ProjectTaskLinks => Set<ProjectTaskLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ProjectManagement");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (typeof(Entity<Guid>).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType).HasQueryFilter(CreateSoftDeleteFilter(clrType));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private static System.Linq.Expressions.LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Entity<Guid>.IsDeleted));
        var body = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(body, parameter);
    }
}
