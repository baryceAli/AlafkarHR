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
        builder.Property(x => x.ProjectName).HasMaxLength(250);
        builder.Property(x => x.PlaceName).HasMaxLength(250);
        builder.Property(x => x.FreeTextLocation).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Customers).WithOne().HasForeignKey(x => x.MediaActivityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Allocations).WithOne().HasForeignKey(x => x.MediaActivityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Media).WithOne().HasForeignKey(x => x.MediaActivityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.CompanyId, x.ActivityDate });
        builder.HasIndex(x => new { x.CompanyId, x.ActivityTypeId });
        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.DistributionPlaceId);
    }
}

public class MediaActivityCustomerConfiguration : IEntityTypeConfiguration<MediaActivityCustomer>
{
    public void Configure(EntityTypeBuilder<MediaActivityCustomer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerName).HasMaxLength(250);
        builder.Property(x => x.CustomerNameEng).HasMaxLength(250);
        builder.Property(x => x.ProjectCustomerName).HasMaxLength(250);
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.ProjectCustomerId);
    }
}

public class MediaActivityAllocationConfiguration : IEntityTypeConfiguration<MediaActivityAllocation>
{
    public void Configure(EntityTypeBuilder<MediaActivityAllocation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerName).HasMaxLength(250);
        builder.Property(x => x.DeliverableName).HasMaxLength(250);
        builder.Property(x => x.PlaceName).HasMaxLength(250);
        builder.HasIndex(x => x.ProjectDistributionAllocationId);
        builder.HasIndex(x => new { x.DistributionDate, x.ProjectCustomerId, x.DistributionPlaceId });
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
