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

        // 🔥 Multi-tenant uniqueness
        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.NameEng }).IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}