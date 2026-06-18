namespace RealEstate.Data;

public class RealEstateDbContext(DbContextOptions<RealEstateDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyUnit> PropertyUnits => Set<PropertyUnit>();
    public DbSet<Lease> Leases => Set<Lease>();
    public DbSet<LeaseInstallment> LeaseInstallments => Set<LeaseInstallment>();
    public DbSet<LeasePaymentAllocation> LeasePaymentAllocations => Set<LeasePaymentAllocation>();
    public DbSet<PropertyExpense> PropertyExpenses => Set<PropertyExpense>();
    public DbSet<UtilityAccount> UtilityAccounts => Set<UtilityAccount>();
    public DbSet<UtilityBill> UtilityBills => Set<UtilityBill>();
    public DbSet<OccupancyHistory> OccupancyHistory => Set<OccupancyHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("RealEstate");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
