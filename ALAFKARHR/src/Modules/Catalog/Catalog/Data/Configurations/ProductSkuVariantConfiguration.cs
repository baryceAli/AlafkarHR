namespace Catalog.Data.Configurations;

public class ProductSkuVariantConfiguration : IEntityTypeConfiguration<ProductSkuVariant>
{
    public void Configure(EntityTypeBuilder<ProductSkuVariant> builder)
    {
        builder.ToTable("ProductSkuVariants");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ProductSkuId, x.VariantId })
            .IsUnique();

        builder.HasOne<Variant>()
            .WithMany()
            .HasForeignKey(x => x.VariantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<VariantValue>()
            .WithMany()
            .HasForeignKey(x => x.VariantValueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}