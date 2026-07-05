using System.Reflection;

namespace Inventory.Data;
//add-migration InventoryInitial -Project Inventory -StartupProject Api -OutputDir Data/Migrations -Context InventoryDbContext
//update-database -Project Inventory -StartupProject Api -Context InventoryDbContext
public class InventoryDbContext:DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options):base(options)
    {
        
    }
    public DbSet<Warehouse> Warehouses=> Set<Warehouse>();
    public DbSet<AssetInstance> AssetInstances => Set<AssetInstance>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<BatchStock> BatchStocks => Set<BatchStock>();
    public DbSet<InventoryAggregate> Inventories=> Set<InventoryAggregate>();
    public DbSet<InventorySnapshot> InventorySnapshots => Set<InventorySnapshot>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<InventorySerialNumber> InventorySerialNumbers => Set<InventorySerialNumber>();
    public DbSet<StockMovementSerial> StockMovementSerials => Set<StockMovementSerial>();
    public DbSet<WarehouseTransfer> WarehouseTransfers => Set<WarehouseTransfer>();
    public DbSet<TransferItem> TransferItems => Set<TransferItem>();
    public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();
    public DbSet<InventoryLocationBalance> InventoryLocationBalances => Set<InventoryLocationBalance>();
    public DbSet<CycleCount> CycleCounts => Set<CycleCount>();
    public DbSet<CycleCountLine> CycleCountLines => Set<CycleCountLine>();
    public DbSet<BarcodeOperationSession> BarcodeOperationSessions => Set<BarcodeOperationSession>();
    public DbSet<BarcodeOperationLine> BarcodeOperationLines => Set<BarcodeOperationLine>();
    public DbSet<PutawayRule> PutawayRules => Set<PutawayRule>();
    public DbSet<QualityInspection> QualityInspections => Set<QualityInspection>();
    public DbSet<LandedCostVoucher> LandedCostVouchers => Set<LandedCostVoucher>();
    public DbSet<InventoryValuationLayer> InventoryValuationLayers => Set<InventoryValuationLayer>();
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
