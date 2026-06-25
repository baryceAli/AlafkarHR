namespace StoreFront.Data;

public class StoreFrontDbContext(DbContextOptions<StoreFrontDbContext> options) : DbContext(options)
{
    public DbSet<StoreFrontType> StoreFrontTypes => Set<StoreFrontType>();
    public DbSet<StoreFrontStore> StoreFronts => Set<StoreFrontStore>();
    public DbSet<StoreFrontDepartment> StoreFrontDepartments => Set<StoreFrontDepartment>();
    public DbSet<StoreFrontSellableItem> StoreFrontSellableItems => Set<StoreFrontSellableItem>();
    public DbSet<PosCashierSession> PosCashierSessions => Set<PosCashierSession>();
    public DbSet<PosCashierSessionTransfer> PosCashierSessionTransfers => Set<PosCashierSessionTransfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("StoreFront");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<StoreFrontType>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StoreFrontStore>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StoreFrontDepartment>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StoreFrontSellableItem>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<PosCashierSession>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<PosCashierSessionTransfer>().HasQueryFilter(x => !x.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }
}
