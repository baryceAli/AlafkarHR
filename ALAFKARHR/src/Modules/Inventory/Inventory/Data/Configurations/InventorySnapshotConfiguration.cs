using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

public class InventorySnapshotConfiguration : IEntityTypeConfiguration<InventorySnapshot>
{
    public void Configure(EntityTypeBuilder<InventorySnapshot> builder)
    {
        builder.ToTable("InventorySnapshots", "Inventory");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.ProductSkuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.BatchId).IsRequired();
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.ReservedQuantity).HasColumnType("decimal(18,4)").IsRequired();

        builder.Property(x => x.RowVersion)
               .IsRowVersion()
               .IsRequired();

        //builder.Property(x => x.LastUpdated).IsRequired(false);

        // Unique constraint
        builder.HasIndex(x => new { x.ProductId, x.WarehouseId, x.BatchId })
               .IsUnique()
               .HasDatabaseName("UX_InventorySnapshot_Product_Warehouse_Batch");
    }
}
