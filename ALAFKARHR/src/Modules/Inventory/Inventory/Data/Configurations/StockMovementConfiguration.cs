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
        //builder.Property(x => x.MovementCategory).IsRequired();
        builder.Property(x => x.ReferenceNumber).IsRequired();
        builder.Property(x => x.SourceDocumentType).IsRequired();
        builder.Property(x => x.QuantityBefore).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.QuantityAfter).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.ProductPackageId).IsRequired(false);
        builder.Property(x => x.EnteredQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.PackageMultiplier).HasColumnType("decimal(18,4)").HasDefaultValue(1m).IsRequired();
        builder.Property(x => x.NormalizedQuantity).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.CurrencyId).IsRequired();
        builder.HasIndex(x => x.CurrencyId);
        //builder.Property(x => x.MovementDate).IsRequired();
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
