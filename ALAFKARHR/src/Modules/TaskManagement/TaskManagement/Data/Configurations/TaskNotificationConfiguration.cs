namespace TaskManagement.Data.Configurations;

public class TaskNotificationConfiguration : IEntityTypeConfiguration<TaskNotification>
{
    public void Configure(EntityTypeBuilder<TaskNotification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.NotificationType).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => new { x.UserCode, x.IsRead });
    }
}
