namespace MediaCenter.Data.Configurations;

public class MediaActivityTypeConfiguration : IEntityTypeConfiguration<MediaActivityType>
{
    public void Configure(EntityTypeBuilder<MediaActivityType> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(180);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.Name });
    }
}

public class MediaActivityConfiguration : IEntityTypeConfiguration<MediaActivity>
{
    public void Configure(EntityTypeBuilder<MediaActivity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.TitleEng).HasMaxLength(250);
        builder.Property(x => x.LocationText).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.RelatedRecords).WithOne().HasForeignKey(x => x.MediaActivityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Media).WithOne().HasForeignKey(x => x.MediaActivityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.CompanyId, x.ActivityDate });
        builder.HasIndex(x => new { x.CompanyId, x.ActivityTypeId });
    }
}

public class MediaActivityRelatedRecordConfiguration : IEntityTypeConfiguration<MediaActivityRelatedRecord>
{
    public void Configure(EntityTypeBuilder<MediaActivityRelatedRecord> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RelatedType).HasMaxLength(120).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => x.RelatedType);
        builder.HasIndex(x => x.RelatedRecordId);
    }
}

public class MediaActivityMediaConfiguration : IEntityTypeConfiguration<MediaActivityMedia>
{
    public void Configure(EntityTypeBuilder<MediaActivityMedia> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Caption).HasMaxLength(500);
        builder.HasIndex(x => x.DocumentId);
        builder.HasIndex(x => x.MediaKind);
    }
}
