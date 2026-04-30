namespace Catalog.Data.Configurations;

public class VariantValueConfiguration : IEntityTypeConfiguration<VariantValue>
{
    public void Configure(EntityTypeBuilder<VariantValue> builder)
    {
        builder.ToTable("VariantValues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ValueEng)
            .IsRequired()
            .HasMaxLength(100);

        // 🔥 Prevent duplicate values per variant
        builder.HasIndex(x => new { x.VariantId, x.Value }).IsUnique();
        builder.HasIndex(x => new { x.VariantId, x.ValueEng }).IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
