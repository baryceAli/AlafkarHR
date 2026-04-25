using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("Batches", "Inventory");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        //builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.ProductSkuId).IsRequired();
        builder.Property(x => x.BatchNumber).IsRequired();
        builder.Property(x => x.ManufacturingDate).IsRequired();
        builder.Property(x => x.ExpiryDate).IsRequired();

        // Unique index as per domain comment
        builder.HasIndex(b => new { b.ProductId,  b.BatchNumber })
               .IsUnique()
               .HasDatabaseName("UX_Batch_Product_BatchNumber");

        // Audit fields
        builder.Property<DateTime?>("CreatedAt");
        builder.Property<string?>("CreatedBy");
        builder.Property<DateTime?>("LastModified");
        builder.Property<string?>("LastModifiedBy");
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<string?>("DeletedBy");
    }
}
