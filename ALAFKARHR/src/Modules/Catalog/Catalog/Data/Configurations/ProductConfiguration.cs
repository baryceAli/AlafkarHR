namespace Catalog.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEng).IsRequired().HasMaxLength(200);

        //builder.HasMany(typeof(ProductSku), "_skus")
        //    .WithOne()
        //    .HasForeignKey("ProductId")
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}