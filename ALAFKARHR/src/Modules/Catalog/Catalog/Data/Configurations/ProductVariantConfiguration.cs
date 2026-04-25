
namespace Catalog.Data.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductSKU>
{
    public void Configure(EntityTypeBuilder<ProductSKU> builder)
    {
        builder.ToTable("ProductSKUs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Sku)
            .IsUnique();

        builder
            .HasOne<Variant>()
            .WithMany()
            .HasForeignKey(x => x.VariantId);
        // 🔥 Configure Value Object collection
        
        builder.HasOne<Product>()
            .WithMany(p => p.ProductSkus)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
