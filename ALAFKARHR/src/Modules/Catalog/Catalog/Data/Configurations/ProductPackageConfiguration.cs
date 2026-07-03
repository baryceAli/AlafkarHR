namespace Catalog.Data.Configurations;

public class ProductPackageConfiguration : IEntityTypeConfiguration<ProductPackage>
{
    public void Configure(EntityTypeBuilder<ProductPackage> builder)
    {
        builder.ToTable("ProductPackages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.NameEng)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Barcode)
            .HasMaxLength(100);

        builder.Property(x => x.Weight)
            .HasColumnType("decimal(18,3)");

        builder.Property(x => x.Length)
            .HasColumnType("decimal(18,3)");

        builder.Property(x => x.Width)
            .HasColumnType("decimal(18,3)");

        builder.Property(x => x.Height)
            .HasColumnType("decimal(18,3)");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔥 Multi-tenant uniqueness
        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.NameEng }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.Barcode })
            .IsUnique()
            .HasFilter("[Barcode] IS NOT NULL");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
