namespace StoreFront.Data;

public class StoreFrontDbContext(DbContextOptions<StoreFrontDbContext> options) : DbContext(options)
{
    public DbSet<StoreFrontType> StoreFrontTypes => Set<StoreFrontType>();
    public DbSet<StoreFrontStore> StoreFronts => Set<StoreFrontStore>();
    public DbSet<StoreFrontSellableItem> StoreFrontSellableItems => Set<StoreFrontSellableItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("StoreFront");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<StoreFrontType>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StoreFrontStore>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StoreFrontSellableItem>().HasQueryFilter(x => !x.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }
}
