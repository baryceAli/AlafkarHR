namespace Catalog.Data.Configurations;

public class ProductSkuVariantConfiguration : IEntityTypeConfiguration<ProductSkuVariant>
{
    public void Configure(EntityTypeBuilder<ProductSkuVariant> builder)
    {
        builder.ToTable("ProductSkuVariants");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ProductSkuId, x.VariantId })
            .IsUnique(); // 🔥 prevents duplicate variant per SKU
    }
}