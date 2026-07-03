namespace Catalog.Data.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UnitNameEng)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UnitCategory)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("General");

        builder.Property(x => x.ConversionFactor)
            .HasColumnType("decimal(18,6)")
            .HasDefaultValue(1m);

        builder.Property(x => x.IsReferenceUnit)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => new { x.CompanyId, x.UnitName }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.UnitNameEng }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.UnitCategory, x.IsReferenceUnit })
            .IsUnique()
            .HasFilter("[IsReferenceUnit] = 1");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
