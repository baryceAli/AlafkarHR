namespace Pricing.Data.Configurations;

public class PromotionPriceItemConfiguration : IEntityTypeConfiguration<PromotionPriceItem>
{
    public void Configure(EntityTypeBuilder<PromotionPriceItem> builder)
    {
        builder.ToTable("PromotionPriceItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.PromotionPriceId).IsRequired();
        builder.Property(x => x.ProductSkuId).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.PromotionPriceId, x.ProductSkuId, x.UnitId, x.MinQuantity });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
