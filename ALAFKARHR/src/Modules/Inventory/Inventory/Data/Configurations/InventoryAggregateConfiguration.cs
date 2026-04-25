using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

public class InventoryAggregateConfiguration : IEntityTypeConfiguration<InventoryAggregate>
{
    public void Configure(EntityTypeBuilder<InventoryAggregate> builder)
    {
        builder.ToTable("Inventories", "Inventory");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.ProductSkuId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();

        // One-to-many for BatchStocks
        //builder.HasMany<BatchStock>("_batches")
        //       .WithOne()
        //       .HasForeignKey("InventoryAggregateId")
        //       .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Batches)
       .WithOne()
       .HasForeignKey("InventoryAggregateId")
       .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Batches)
               .HasField("_batches")
               .UsePropertyAccessMode(PropertyAccessMode.Field);
        // Audit fields
        builder.Property<DateTime?>("CreatedAt");
        builder.Property<string?>("CreatedBy");
        builder.Property<DateTime?>("LastModified");
        builder.Property<string?>("LastModifiedBy");
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<string?>("DeletedBy");
    }
}
