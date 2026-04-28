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
    }
}
