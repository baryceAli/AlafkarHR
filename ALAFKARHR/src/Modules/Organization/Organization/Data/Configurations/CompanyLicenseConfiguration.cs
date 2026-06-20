using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;

namespace Organization.Data.Configurations;

public class CompanyLicenseConfiguration : IEntityTypeConfiguration<CompanyLicense>
{
    public void Configure(EntityTypeBuilder<CompanyLicense> builder)
    {
        builder.ToTable("CompanyLicenses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanKey).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PlanName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasOne(x => x.Company)
            .WithOne()
            .HasForeignKey<CompanyLicense>(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LicenseCategory)
            .WithMany()
            .HasForeignKey(x => x.LicenseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CompanyId).IsUnique();
        builder.HasIndex(x => x.LicenseCategoryId);
        builder.HasIndex(x => new { x.Status, x.EndDate });
    }
}
