namespace Catalog.Data.Configurations;

public class ProductSkuPackageConfiguration : IEntityTypeConfiguration<ProductSkuPackage>
{
    public void Configure(EntityTypeBuilder<ProductSkuPackage> builder)
    {
        builder.ToTable("ProductSkuPackages");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ProductSku)
            .WithMany(x => x.Packages)
            .HasForeignKey(x => x.ProductSkuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductPackage)
            .WithMany()
            .HasForeignKey(x => x.ProductPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ProductSkuId, x.ProductPackageId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
