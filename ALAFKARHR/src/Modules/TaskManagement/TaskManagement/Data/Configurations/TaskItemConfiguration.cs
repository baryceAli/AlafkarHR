namespace TaskManagement.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaskNumber).HasMaxLength(40).IsRequired();
        builder.HasIndex(x => x.TaskNumber).IsUnique();
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.Priority).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.RecurrenceFrequency).HasConversion<string>().HasMaxLength(30).HasDefaultValue(TaskRecurrenceFrequency.None);
        builder.Property(x => x.RecurrenceEndType).HasConversion<string>().HasMaxLength(30).HasDefaultValue(TaskRecurrenceEndType.Never);
        builder.Property(x => x.RecurrenceInterval).HasDefaultValue(1);
        builder.Property(x => x.ProgressPercentage).HasPrecision(5, 2);
        builder.HasIndex(x => x.NextOccurrenceDate);
        builder.HasIndex(x => x.ParentTaskId);
        builder.HasMany(x => x.Comments).WithOne().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Attachments).WithOne().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.History).WithOne().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Actions).WithOne().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
    }
}
