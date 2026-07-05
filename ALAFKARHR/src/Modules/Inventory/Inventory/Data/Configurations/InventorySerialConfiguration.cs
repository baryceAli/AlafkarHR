namespace Inventory.Data.Configurations;

public class InventorySerialNumberConfiguration : IEntityTypeConfiguration<InventorySerialNumber>
{
    public void Configure(EntityTypeBuilder<InventorySerialNumber> builder)
    {
        builder.ToTable("InventorySerialNumbers", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SerialNumber).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.ProductSkuId).IsRequired();
        builder.Property(x => x.BatchId).IsRequired(false);
        builder.Property(x => x.WarehouseId).IsRequired(false);
        builder.Property(x => x.WarehouseLocationId).IsRequired(false);
        builder.Property(x => x.SourceDocumentId).IsRequired(false);
        builder.Property(x => x.SourceDocumentLineId).IsRequired(false);
        builder.Property(x => x.LastStockMovementId).IsRequired(false);
        builder.Property(x => x.LastMovementAt).IsRequired(false);
        builder.HasIndex(x => new { x.CompanyId, x.ProductSkuId, x.SerialNumber }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.ProductSkuId, x.WarehouseId, x.WarehouseLocationId, x.BatchId, x.Status });
        builder.HasIndex(x => x.LastStockMovementId);
    }
}

public class StockMovementSerialConfiguration : IEntityTypeConfiguration<StockMovementSerial>
{
    public void Configure(EntityTypeBuilder<StockMovementSerial> builder)
    {
        builder.ToTable("StockMovementSerials", "Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StockMovementId).IsRequired();
        builder.Property(x => x.InventorySerialNumberId).IsRequired();
        builder.Property(x => x.SerialNumber).HasMaxLength(120).IsRequired();
        builder.Property(x => x.StatusAfterMovement).IsRequired();
        builder.HasIndex(x => x.StockMovementId);
        builder.HasIndex(x => x.InventorySerialNumberId);
        builder.HasIndex(x => new { x.StockMovementId, x.InventorySerialNumberId }).IsUnique();
    }
}
