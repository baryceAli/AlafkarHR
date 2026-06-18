namespace Maintenance.Data.Configurations;

public class MaintenanceHistoryConfiguration : IEntityTypeConfiguration<MaintenanceHistory>
{
    public void Configure(EntityTypeBuilder<MaintenanceHistory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Details).HasMaxLength(2000);
        builder.HasIndex(x => x.WorkOrderId);
    }
}
