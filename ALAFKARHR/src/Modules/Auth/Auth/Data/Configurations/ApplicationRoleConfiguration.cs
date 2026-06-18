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
            .HasFilter("[DisplayName] IS NOT NULL");

        builder.HasIndex(r => new { r.CompanyId, r.TemplateKey })
            .IsUnique()
            .HasDatabaseName("IX_AspNetRoles_CompanyId_TemplateKey")
            .HasFilter("[TemplateKey] IS NOT NULL");
    }
}
