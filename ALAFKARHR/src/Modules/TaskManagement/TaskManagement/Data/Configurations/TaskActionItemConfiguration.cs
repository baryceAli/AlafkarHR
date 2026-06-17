namespace TaskManagement.Data.Configurations;

public class TaskActionItemConfiguration : IEntityTypeConfiguration<TaskActionItem>
{
    public void Configure(EntityTypeBuilder<TaskActionItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
        builder.Property(x => x.CreatedByUserName).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.TaskId);
        builder.HasIndex(x => x.ExpectedCompletionAt);
        builder.HasIndex(x => new { x.CreatedByUserId, x.TaskId });
    }
}
