
namespace Catalog.Data.Configurations;

public class ProductSKUConfiguration : IEntityTypeConfiguration<ProductSku>
{
    public void Configure(EntityTypeBuilder<ProductSku> builder)
    {
        builder.ToTable("ProductSKUs");

        builder.HasKey(x => x.Id);

        //builder.Property(x => x.Sku)
        //    .IsRequired()
        //    .HasMaxLength(100);

        //builder.HasIndex(x => x.Sku)
        //    .IsUnique();

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        //builder
        //    .HasOne<Variant>()
        //    .WithMany()
        //    .HasForeignKey(x => x.VariantId);
        //// 🔥 Configure Value Object collection
        
        //builder.HasOne<Product>()
        //    .WithMany(p => p.ProductSkus)
        //    .HasForeignKey(x => x.ProductId)
        //    .OnDelete(DeleteBehavior.Cascade);


    }
}
