namespace Pricing.Data.Configurations;

public class PromotionPriceConfiguration : IEntityTypeConfiguration<PromotionPrice>
{
    public void Configure(EntityTypeBuilder<PromotionPrice> builder)
    {
        builder.ToTable("PromotionPrices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.EffectiveFrom).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.PromotionPriceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CompanyId, x.IsActive, x.EffectiveFrom });
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
