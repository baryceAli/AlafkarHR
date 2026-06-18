namespace DocumentManagement.Data.Configurations;

public class DocumentItemConfiguration : IEntityTypeConfiguration<DocumentItem>
{
    public void Configure(EntityTypeBuilder<DocumentItem> builder)
    {
        builder.ToTable("Documents");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.SourceModule).HasMaxLength(100);
        builder.Property(x => x.SourceEntity).HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasMany(x => x.Versions)
            .WithOne()
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Collaborators)
            .WithOne()
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Versions).HasField("_versions").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Collaborators).HasField("_collaborators").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => new { x.CompanyId, x.Title });
        builder.HasIndex(x => new { x.CompanyId, x.SourceModule, x.SourceEntity, x.SourceRecordId });
    }
}
