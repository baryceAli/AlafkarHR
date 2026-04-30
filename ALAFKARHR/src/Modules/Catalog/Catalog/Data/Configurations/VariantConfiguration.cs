namespace Catalog.Data.Configurations;

public class VariantConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.ToTable("Variants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameEng)
            .IsRequired()
            .HasMaxLength(200);

        // 🔥 Values relationship
        //builder.HasMany(typeof(VariantValue), "_values")
        //    .WithOne()
        //    .HasForeignKey("VariantId")
        //    .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.NameEng }).IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
