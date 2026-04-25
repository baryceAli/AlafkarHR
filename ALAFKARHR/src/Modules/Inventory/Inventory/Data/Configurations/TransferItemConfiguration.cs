using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

public class TransferItemConfiguration : IEntityTypeConfiguration<TransferItem>
{
    public void Configure(EntityTypeBuilder<TransferItem> builder)
    {
        // Table name
        builder.ToTable("TransferItems");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.ProductId)
               .IsRequired();

        builder.Property(x => x.ProductSkuId)
               .IsRequired();

        builder.Property(x => x.BatchId)
               .IsRequired();

        builder.Property(x => x.WarehouseId)
               .IsRequired();

        builder.Property(x => x.Quantity)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.ReceivedQuantity)
               .HasPrecision(18, 2);

        // Indexes (optional but useful)
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.ProductSkuId);
        builder.HasIndex(x => x.BatchId);
        builder.HasIndex(x => x.WarehouseId);

        // If you later add navigation properties, define relationships here
        // Example:
        // builder.HasOne<Product>()
        //        .WithMany()
        //        .HasForeignKey(x => x.ProductId)
        //        .OnDelete(DeleteBehavior.Restrict);
    }
}
