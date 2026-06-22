namespace MediaCenter.Data;

public class MediaCenterDbContext(DbContextOptions<MediaCenterDbContext> options) : DbContext(options)
{
    public DbSet<MediaActivityType> MediaActivityTypes => Set<MediaActivityType>();
    public DbSet<MediaActivity> MediaActivities => Set<MediaActivity>();
    public DbSet<MediaActivityRelatedRecord> MediaActivityRelatedRecords => Set<MediaActivityRelatedRecord>();
    public DbSet<MediaActivityMedia> MediaActivityMedia => Set<MediaActivityMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("MediaCenter");
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
