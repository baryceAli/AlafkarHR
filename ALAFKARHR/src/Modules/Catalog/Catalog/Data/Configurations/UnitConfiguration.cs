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

        builder.HasIndex(x => new { x.CompanyId, x.UnitName }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.UnitNameEng }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
