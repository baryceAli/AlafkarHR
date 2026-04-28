
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

        builder.HasIndex(x => x.Barcode)
     .IsUnique();
     //.HasFilter("[Barcode] IS NOT NULL");

        //builder.HasMany(typeof(ProductSkuVariant), "_variants")
        //    .WithOne()
        //    .HasForeignKey("ProductSkuId")
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}
