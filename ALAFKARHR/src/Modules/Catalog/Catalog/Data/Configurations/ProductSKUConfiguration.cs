
namespace Catalog.Data.Configurations;

public class ProductSKUConfiguration : IEntityTypeConfiguration<ProductSku>
{
    public void Configure(EntityTypeBuilder<ProductSku> builder)
    {
        builder.ToTable("ProductSKUs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.SkuCode)
            .HasMaxLength(100);

        builder.Property(x => x.SkuCodeEng)
            .HasMaxLength(100);

        builder.Property(x => x.SkuKey)
            .HasMaxLength(200);

        // 🔥 Relationships
        //builder.HasOne<Product>()
        //    .WithMany("_skus")
        //    .HasForeignKey(x => x.ProductId);

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(x => x.BrandId);

        builder.HasOne<ProductPackage>()
            .WithMany()
            .HasForeignKey(x => x.PackageId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔥 Variants relation
        builder.HasMany(s => s.Variants)
    .WithOne()
    .HasForeignKey(v => v.ProductSkuId)
    .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne<ProductPackage>()
    .WithMany()
    .HasForeignKey(s => s.PackageId)
    .OnDelete(DeleteBehavior.Restrict);


        // 🔥 Multi-tenant uniqueness
        builder.HasIndex(x => new { x.CompanyId, x.Barcode })
            .IsUnique()
            .HasFilter("[Barcode] IS NOT NULL");

        builder.HasIndex(x => new { x.CompanyId, x.SkuKey })
            .IsUnique();

        // 🔥 Soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
