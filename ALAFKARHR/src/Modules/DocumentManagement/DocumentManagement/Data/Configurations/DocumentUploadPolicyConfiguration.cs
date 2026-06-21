namespace DocumentManagement.Data.Configurations;

public class DocumentUploadPolicyConfiguration : IEntityTypeConfiguration<DocumentUploadPolicy>
{
    public void Configure(EntityTypeBuilder<DocumentUploadPolicy> builder)
    {
        builder.ToTable("DocumentUploadPolicies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaxFileSizeBytes).IsRequired();
        builder.Property(x => x.AllowedExtensions).IsRequired();
        builder.Property(x => x.AllowedContentTypes).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => x.CompanyId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
