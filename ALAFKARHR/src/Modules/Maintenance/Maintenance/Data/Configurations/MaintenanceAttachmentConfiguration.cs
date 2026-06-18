namespace Maintenance.Data.Configurations;

public class MaintenanceAttachmentConfiguration : IEntityTypeConfiguration<MaintenanceAttachment>
{
    public void Configure(EntityTypeBuilder<MaintenanceAttachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(120).IsRequired();
        builder.Property(x => x.FilePath).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => x.WorkOrderId);
    }
}
