namespace Catalog.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameEng)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        //builder.HasOne(x=> x.Category)
        builder.HasOne< Category>()
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        //builder.HasOne(x=> x.Brand)
        builder.HasOne< Brand>()
            .WithMany()
            .HasForeignKey(x => x.BrandId);

        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(x => x.UnitId);
    }
}