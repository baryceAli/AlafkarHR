namespace Maintenance.Data.Configurations;

public class MaintenanceCommentConfiguration : IEntityTypeConfiguration<MaintenanceComment>
{
    public void Configure(EntityTypeBuilder<MaintenanceComment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Comment).HasMaxLength(2000).IsRequired();
        builder.HasIndex(x => x.WorkOrderId);
    }
}
