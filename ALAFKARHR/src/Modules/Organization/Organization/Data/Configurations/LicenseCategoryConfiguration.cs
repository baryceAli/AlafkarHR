using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;

namespace Organization.Data.Configurations;

public class LicenseCategoryConfiguration : IEntityTypeConfiguration<LicenseCategory>
{
    public void Configure(EntityTypeBuilder<LicenseCategory> builder)
    {
        builder.ToTable("LicenseCategories");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(x => x.MonthlyPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.YearlyPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasIndex(x => x.Key).IsUnique();
        builder.HasIndex(x => x.IsActive);
    }
}
