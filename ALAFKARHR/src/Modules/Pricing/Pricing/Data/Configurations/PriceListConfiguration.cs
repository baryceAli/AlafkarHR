namespace Pricing.Data.Configurations;

public class PriceListConfiguration : IEntityTypeConfiguration<PriceList>
{
    public void Configure(EntityTypeBuilder<PriceList> builder)
    {
        builder.ToTable("PriceLists");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CurrencyId);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.CompanyId).IsRequired();
        builder.Property(x => x.EffectiveFrom).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.PriceListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.IsDefault });
        builder.HasIndex(x => x.CurrencyId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
