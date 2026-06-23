using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Organizations.Models;

namespace Organization.Data.Configurations;

public class LicenseCategoryBusinessLineConfiguration : IEntityTypeConfiguration<LicenseCategoryBusinessLine>
{
    public void Configure(EntityTypeBuilder<LicenseCategoryBusinessLine> builder)
    {
        builder.ToTable("LicenseCategoryBusinessLines");
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.BusinessLine.IsDeleted);
        builder.Property(x => x.ActivationLimit).IsRequired();

        builder.HasOne(x => x.LicenseCategory)
            .WithMany()
            .HasForeignKey(x => x.LicenseCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.BusinessLine)
            .WithMany()
            .HasForeignKey(x => x.BusinessLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.LicenseCategoryId, x.BusinessLineId }).IsUnique();
        builder.HasIndex(x => x.BusinessLineId);
    }
}

public class CompanyLicenseBusinessLineConfiguration : IEntityTypeConfiguration<CompanyLicenseBusinessLine>
{
    public void Configure(EntityTypeBuilder<CompanyLicenseBusinessLine> builder)
    {
        builder.ToTable("CompanyLicenseBusinessLines");
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.BusinessLine.IsDeleted);
        builder.Property(x => x.ActivationLimit).IsRequired();

        builder.HasOne(x => x.CompanyLicense)
            .WithMany()
            .HasForeignKey(x => x.CompanyLicenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.BusinessLine)
            .WithMany()
            .HasForeignKey(x => x.BusinessLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CompanyLicenseId, x.BusinessLineId }).IsUnique();
        builder.HasIndex(x => x.BusinessLineId);
    }
}

public class BusinessLineActivationConfiguration : IEntityTypeConfiguration<BusinessLineActivation>
{
    public void Configure(EntityTypeBuilder<BusinessLineActivation> builder)
    {
        builder.ToTable("BusinessLineActivations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);

        builder.HasOne(x => x.BusinessLine)
            .WithMany()
            .HasForeignKey(x => x.BusinessLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ParentCompanyId, x.BusinessLineId, x.IsActive });
        builder.HasIndex(x => new { x.CompanyId, x.BusinessLineId, x.IsActive });
    }
}
