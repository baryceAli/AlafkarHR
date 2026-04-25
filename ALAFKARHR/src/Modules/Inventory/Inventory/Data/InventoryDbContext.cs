using System.Reflection;

namespace Inventory.Data;
//add-migration InventoryInitial -Project Inventory -StartupProject Api -OutputDir Data/Migrations -Context InventoryDbContext
public class InventoryDbContext:DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options):base(options)
    {
        
    }
    public DbSet<Warehouse> Warehouses=> Set<Warehouse>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<BatchStock> BatchStocks => Set<BatchStock>();
    public DbSet<InventoryAggregate> Inventories=> Set<InventoryAggregate>();
    public DbSet<InventorySnapshot> InventorySnapshots => Set<InventorySnapshot>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<WarehouseTransfer> WarehouseTransfers => Set<WarehouseTransfer>();
    public DbSet<TransferItem> TransferItems => Set<TransferItem>();
    //public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    //public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // 🔥 Schema
        modelBuilder.HasDefaultSchema("Inventory");

        // 🔥 Apply all IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 🔥 Global Conventions (optional but recommended)
        //ApplyGlobalConfigurations(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
