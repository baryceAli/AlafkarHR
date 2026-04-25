using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements", "Inventory");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.ProductSkuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.BatchId).IsRequired();
        builder.Property(x => x.MovementType).IsRequired();
        builder.Property(x => x.MovementDirection).IsRequired();
        builder.Property(x => x.MovementCategory).IsRequired();
        builder.Property(x => x.ReferenceId).IsRequired();
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.MovementDate).IsRequired();
        builder.Property(x => x.Notes).IsRequired(false);

        // Audit fields
        builder.Property<DateTime?>("CreatedAt");
        builder.Property<string?>("CreatedBy");
        builder.Property<DateTime?>("LastModified");
        builder.Property<string?>("LastModifiedBy");
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<string?>("DeletedBy");
    }
}
