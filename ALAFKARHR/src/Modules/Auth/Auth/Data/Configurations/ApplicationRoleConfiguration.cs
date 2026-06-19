using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.Property(r => r.DisplayName)
            .HasMaxLength(256);

        builder.Property(r => r.TemplateKey)
            .HasMaxLength(100);

        builder.HasIndex(r => new { r.CompanyId, r.DisplayName })
            .IsUnique()
            .HasDatabaseName("IX_AspNetRoles_CompanyId_DisplayName")
            .HasFilter("[CompanyId] IS NOT NULL AND [DisplayName] IS NOT NULL");

        builder.HasIndex(r => r.DisplayName)
            .IsUnique()
            .HasDatabaseName("IX_AspNetRoles_Platform_DisplayName")
            .HasFilter("[CompanyId] IS NULL AND [DisplayName] IS NOT NULL");

        builder.HasIndex(r => new { r.CompanyId, r.TemplateKey })
            .IsUnique()
            .HasDatabaseName("IX_AspNetRoles_CompanyId_TemplateKey")
            .HasFilter("[CompanyId] IS NOT NULL AND [TemplateKey] IS NOT NULL");

        builder.HasIndex(r => r.TemplateKey)
            .IsUnique()
            .HasDatabaseName("IX_AspNetRoles_Platform_TemplateKey")
            .HasFilter("[CompanyId] IS NULL AND [TemplateKey] IS NOT NULL");
    }
}
