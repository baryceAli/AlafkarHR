using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

public class BatchStockConfiguration : IEntityTypeConfiguration<BatchStock>
{
    public void Configure(EntityTypeBuilder<BatchStock> builder)
    {
        builder.ToTable("BatchStocks", "Inventory");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.BatchId).IsRequired();
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.ReservedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex("InventoryAggregateId", nameof(BatchStock.BatchId), nameof(BatchStock.WarehouseId))
               .IsUnique()
               .HasDatabaseName("UX_BatchStock_Inventory_Batch_Warehouse")
               .HasFilter("[InventoryAggregateId] IS NOT NULL");

        // Owned collection is modeled as relationship in Aggregate; leave as-is
        // Audit fields
        builder.Property<DateTime?>("CreatedAt");
        builder.Property<string?>("CreatedBy");
        builder.Property<DateTime?>("LastModified");
        builder.Property<string?>("LastModifiedBy");
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<string?>("DeletedBy");
    }
}
