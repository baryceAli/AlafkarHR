
namespace Catalog.Data.Configurations;

public class ProductSKUConfiguration : IEntityTypeConfiguration<ProductSku>
{
    public void Configure(EntityTypeBuilder<ProductSku> builder)
    {
        builder.ToTable("ProductSKUs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Calories)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ProductionType)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(SkuProductionType.PurchasedRawMaterial);

        builder.Property(x => x.TrackingMode)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(CatalogTrackingMode.Quantity);

        builder.Property(x => x.IsSellable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsPurchasable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsInventoryTracked)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsAssetTrackable)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

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

        // 🔥 Variants relation
        builder.HasMany(s => s.Variants)
    .WithOne()
    .HasForeignKey(v => v.ProductSkuId)
    .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Packages)
            .WithOne(x => x.ProductSku)
            .HasForeignKey(x => x.ProductSkuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Components)
            .WithOne(x => x.ParentProductSku)
            .HasForeignKey(x => x.ParentProductSkuId)
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

        builder.HasIndex(x => new { x.CompanyId, x.ProductId, x.BrandId, x.PackageId, x.SkuCode })
            .IsUnique();

        // 🔥 Soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
