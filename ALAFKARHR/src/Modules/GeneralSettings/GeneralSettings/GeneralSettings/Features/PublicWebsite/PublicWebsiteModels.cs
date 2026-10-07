using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralSettings.GeneralSettings.Features.PublicWebsite;

public sealed class PublicWebsiteSite
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid Token { get; set; }
    public Guid? PublishedRevisionId { get; set; }
    public string DraftJson { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";
}

public sealed class PublicWebsiteRevision
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string ContentJson { get; set; } = "";
    public DateTime PublishedAt { get; set; }
    public string PublishedBy { get; set; } = "";
}

public sealed class PublicWebsiteMedia
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = "";
    public string ContentType { get; set; } = "";
    public string StorageKey { get; set; } = "";
    public long Size { get; set; }
    public bool WasPublished { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = "";
}

public sealed class PublicWebsiteAudit
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Action { get; set; } = "";
    public Guid Token { get; set; }
    public DateTime At { get; set; }
    public string By { get; set; } = "";
}

public sealed class PublicWebsiteSiteConfiguration : IEntityTypeConfiguration<PublicWebsiteSite>
{
    public void Configure(EntityTypeBuilder<PublicWebsiteSite> b)
    {
        b.ToTable("PublicWebsiteSites"); b.HasKey(x => x.Id);
        b.HasIndex(x => x.CompanyId).IsUnique();
        b.Property(x => x.Token).IsConcurrencyToken();
        b.Property(x => x.UpdatedBy).HasMaxLength(300);
    }
}
public sealed class PublicWebsiteRevisionConfiguration : IEntityTypeConfiguration<PublicWebsiteRevision>
{
    public void Configure(EntityTypeBuilder<PublicWebsiteRevision> b)
    {
        b.ToTable("PublicWebsiteRevisions"); b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.CompanyId, x.PublishedAt }); b.Property(x => x.PublishedBy).HasMaxLength(300);
    }
}
public sealed class PublicWebsiteMediaConfiguration : IEntityTypeConfiguration<PublicWebsiteMedia>
{
    public void Configure(EntityTypeBuilder<PublicWebsiteMedia> b)
    {
        b.ToTable("PublicWebsiteMedia"); b.HasKey(x => x.Id); b.HasIndex(x => x.CompanyId);
        b.Property(x => x.Name).HasMaxLength(255); b.Property(x => x.ContentType).HasMaxLength(100);
        b.Property(x => x.StorageKey).HasMaxLength(100); b.Property(x => x.UploadedBy).HasMaxLength(300);
    }
}
public sealed class PublicWebsiteAuditConfiguration : IEntityTypeConfiguration<PublicWebsiteAudit>
{
    public void Configure(EntityTypeBuilder<PublicWebsiteAudit> b)
    {
        b.ToTable("PublicWebsiteAudit"); b.HasKey(x => x.Id); b.HasIndex(x => new { x.CompanyId, x.At });
        b.Property(x => x.Action).HasMaxLength(100); b.Property(x => x.By).HasMaxLength(300);
    }
}

public sealed class PublicWebsiteOptions
{
    public Guid OwnerCompanyId { get; set; } 
    public string StorageRoot { get; set; } = "App_Data/PublicWebsite";
    public long ImageMaxBytes { get; set; } = 10 * 1024 * 1024;
    public long PdfMaxBytes { get; set; } = 25 * 1024 * 1024;
    public long MediaMaxBytes { get; set; } = 100 * 1024 * 1024;
}
