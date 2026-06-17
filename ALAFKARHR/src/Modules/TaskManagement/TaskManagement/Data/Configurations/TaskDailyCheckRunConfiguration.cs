namespace TaskManagement.Data.Configurations;

public class TaskDailyCheckRunConfiguration : IEntityTypeConfiguration<TaskDailyCheckRun>
{
    public void Configure(EntityTypeBuilder<TaskDailyCheckRun> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.LastError).HasMaxLength(2000);
        builder.HasIndex(x => new { x.UserId, x.CheckDate }).IsUnique();
        builder.HasIndex(x => x.NextRetryAt);
    }
}
