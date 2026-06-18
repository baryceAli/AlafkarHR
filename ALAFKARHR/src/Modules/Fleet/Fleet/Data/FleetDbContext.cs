namespace Fleet.Data;

public class FleetDbContext(DbContextOptions<FleetDbContext> options) : DbContext(options)
{
    public DbSet<FleetVehicle> Vehicles => Set<FleetVehicle>();
    public DbSet<FleetVehicleAssignment> VehicleAssignments => Set<FleetVehicleAssignment>();
    public DbSet<FleetVehicleDocument> VehicleDocuments => Set<FleetVehicleDocument>();
    public DbSet<FleetVehicleExpense> VehicleExpenses => Set<FleetVehicleExpense>();
    public DbSet<FleetVehicleServiceRule> VehicleServiceRules => Set<FleetVehicleServiceRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Fleet");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
