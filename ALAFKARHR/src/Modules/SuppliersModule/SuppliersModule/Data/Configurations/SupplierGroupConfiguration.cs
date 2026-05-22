using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuppliersModule.Suppliers.Models;

namespace SuppliersModule.Data.Configurations;

public class SupplierGroupConfiguration : IEntityTypeConfiguration<SupplierGroup>
{
    public void Configure(EntityTypeBuilder<SupplierGroup> builder)
    {
        builder.ToTable("SupplierGroups");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.DefaultPaymentTerm).HasConversion<int>().IsRequired();
        builder.Property(x => x.CompanyId).IsRequired();

        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
