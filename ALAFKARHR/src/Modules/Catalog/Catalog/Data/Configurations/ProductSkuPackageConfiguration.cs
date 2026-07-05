namespace Catalog.Data.Configurations;

public class ProductSkuPackageConfiguration : IEntityTypeConfiguration<ProductSkuPackage>
{
    public void Configure(EntityTypeBuilder<ProductSkuPackage> builder)
    {
        builder.ToTable("ProductSkuPackages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,6)")
            .HasDefaultValue(1m);

        builder.Property(x => x.Barcode)
            .HasMaxLength(100);

        builder.Property(x => x.SalesEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.PurchaseEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.ProductSku)
            .WithMany(x => x.Packages)
            .HasForeignKey(x => x.ProductSkuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductPackage)
            .WithMany()
            .HasForeignKey(x => x.ProductPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ProductSkuId, x.ProductPackageId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(x => x.Barcode)
            .HasFilter("[Barcode] IS NOT NULL");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
