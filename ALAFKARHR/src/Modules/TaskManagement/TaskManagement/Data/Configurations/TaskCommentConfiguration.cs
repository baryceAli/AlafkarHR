namespace TaskManagement.Data.Configurations;

public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Comment).HasMaxLength(2000).IsRequired();
        builder.HasIndex(x => x.TaskId);
    }
}
