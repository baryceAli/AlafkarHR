namespace DocumentManagement.Data.Configurations;

public class DocumentCollaboratorConfiguration : IEntityTypeConfiguration<DocumentCollaborator>
{
    public void Configure(EntityTypeBuilder<DocumentCollaborator> builder)
    {
        builder.ToTable("DocumentCollaborators");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName).HasMaxLength(256);
        builder.Property(x => x.AccessLevel).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => new { x.DocumentId, x.UserId }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
