namespace Pricing.Data.Configurations;

public class DiscountCouponConfiguration : IEntityTypeConfiguration<DiscountCoupon>
{
    public void Configure(EntityTypeBuilder<DiscountCoupon> builder)
    {
        builder.ToTable("DiscountCoupons");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinimumOrderAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.EffectiveFrom).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.IsActive, x.EffectiveFrom });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
