namespace Catalog.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameEng)
            .IsRequired()
            .HasMaxLength(200);

        

        builder.HasMany(p => p.Skus)
    .WithOne()
    .HasForeignKey(s => s.ProductId)
    .OnDelete(DeleteBehavior.Cascade);
        // 🔥 Multi-tenant index
        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

        // 🔥 Soft delete filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}