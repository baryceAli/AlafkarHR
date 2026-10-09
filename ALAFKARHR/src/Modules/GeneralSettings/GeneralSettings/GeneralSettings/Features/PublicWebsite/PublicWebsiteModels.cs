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
    public string Origin { get; set; } = "";
    public List<WebsiteStorageLocation> StorageLocations { get; set; } = [];
    public List<WebsiteStorageLocation> ManagedStorageRoots { get; set; } = [];
    public Guid OwnerCompanyId { get; set; } 
    public string StorageRoot { get; set; } = "App_Data/PublicWebsite";
    public long ImageMaxBytes { get; set; } = 10 * 1024 * 1024;
    public long PdfMaxBytes { get; set; } = 25 * 1024 * 1024;
    public long MediaMaxBytes { get; set; } = 100 * 1024 * 1024;
}

public sealed class WebsiteStorageLocation
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Path { get; set; } = "";
}
public sealed class PublicWebsiteManagedLocation
{
    public Guid Id { get; set; }
    public Guid ParentCompanyId { get; set; }
    public string RootKey { get; set; } = "";
    public string FolderName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
}
public sealed class PublicWebsiteManagedLocationMapping : IEntityTypeConfiguration<PublicWebsiteManagedLocation>
{
    public void Configure(EntityTypeBuilder<PublicWebsiteManagedLocation> b)
    {
        b.ToTable("PublicWebsiteManagedLocations"); b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.ParentCompanyId, x.RootKey, x.FolderName }).IsUnique();
        b.Property(x => x.RootKey).HasMaxLength(100);
        b.Property(x => x.FolderName).HasMaxLength(64);
        b.Property(x => x.DisplayName).HasMaxLength(150);
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.CreatedBy).HasMaxLength(300);
    }
}
public sealed class PublicWebsiteConfiguration
{
    public int Id { get; set; } = 1;
    public Guid OwnerCompanyId { get; set; }
    public Guid AdministratorParentCompanyId { get; set; }
    public string StorageLocationKey { get; set; } = "legacy";
    public string PublicOrigin { get; set; } = "";
    public int ImageLimitMiB { get; set; } = 10;
    public int PdfLimitMiB { get; set; } = 25;
    public int MediaLimitMiB { get; set; } = 100;
    public bool Activated { get; set; }
    public Guid Token { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";
}
public sealed class PublicWebsiteConfigurationMapping : IEntityTypeConfiguration<PublicWebsiteConfiguration>
{
    public void Configure(EntityTypeBuilder<PublicWebsiteConfiguration> b)
    {
        b.ToTable("PublicWebsiteConfiguration", t => t.HasCheckConstraint("CK_PublicWebsiteConfiguration_Singleton", "[Id] = 1"));
        b.HasKey(x => x.Id); b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Token).IsConcurrencyToken();
        b.Property(x => x.StorageLocationKey).HasMaxLength(100);
        b.Property(x => x.PublicOrigin).HasMaxLength(2048);
        b.Property(x => x.UpdatedBy).HasMaxLength(300);
    }
}
