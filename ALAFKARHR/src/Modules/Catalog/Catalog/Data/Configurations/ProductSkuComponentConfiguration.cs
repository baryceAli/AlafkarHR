namespace Catalog.Data.Configurations;

public class ProductSkuComponentConfiguration : IEntityTypeConfiguration<ProductSkuComponent>
{
    public void Configure(EntityTypeBuilder<ProductSkuComponent> builder)
    {
        builder.ToTable("ProductSkuComponents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.HasOne(x => x.ParentProductSku)
            .WithMany(x => x.Components)
            .HasForeignKey(x => x.ParentProductSkuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ComponentProductSku)
            .WithMany()
            .HasForeignKey(x => x.ComponentProductSkuId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ParentProductSkuId, x.ComponentProductSkuId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
