using Catering.Models;

namespace Catering;

public class CateringDbContext(DbContextOptions<CateringDbContext> options) : DbContext(options)
{
    public DbSet<MealDefinition> MealDefinitions => Set<MealDefinition>();
    public DbSet<MealComponent> MealComponents => Set<MealComponent>();
    public DbSet<CateringContract> CateringContracts => Set<CateringContract>();
    public DbSet<CateringContractAddendum> CateringContractAddendums => Set<CateringContractAddendum>();
    public DbSet<CateringArea> CateringAreas => Set<CateringArea>();
    public DbSet<CateringSquare> CateringSquares => Set<CateringSquare>();
    public DbSet<CateringDailySchedule> CateringDailySchedules => Set<CateringDailySchedule>();
    public DbSet<CateringSquareAllocation> CateringSquareAllocations => Set<CateringSquareAllocation>();
    public DbSet<CateringVehicleDelivery> CateringVehicleDeliveries => Set<CateringVehicleDelivery>();
    public DbSet<CateringAssignment> CateringAssignments => Set<CateringAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Catering");
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
