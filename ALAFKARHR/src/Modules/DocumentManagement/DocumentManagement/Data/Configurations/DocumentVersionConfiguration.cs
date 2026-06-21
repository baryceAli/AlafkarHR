namespace DocumentManagement.Data.Configurations;

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("DocumentVersions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.StoragePath).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.StorageProvider)
            .HasMaxLength(100)
            .HasDefaultValue(DocumentStorageProviders.LocalFileSystem)
            .IsRequired();
        builder.Property(x => x.StorageKey).HasMaxLength(1000);
        builder.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.DocumentId, x.VersionNumber }).IsUnique();
    }
}
