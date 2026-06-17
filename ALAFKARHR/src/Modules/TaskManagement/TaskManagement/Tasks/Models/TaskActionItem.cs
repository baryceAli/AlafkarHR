namespace TaskManagement.Tasks.Models;

public class TaskActionItem : Entity<Guid>
{
    public Guid TaskId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTime? ExpectedCompletionAt { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string CreatedByUserName { get; private set; } = string.Empty;

    private TaskActionItem()
    {
    }

    public static TaskActionItem Create(Guid taskId, string title, DateTime? expectedCompletionAt, Guid createdByUserId, string createdByUserName)
    {
        return new TaskActionItem
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            Title = title.Trim(),
            ExpectedCompletionAt = expectedCompletionAt,
            CreatedByUserId = createdByUserId,
            CreatedByUserName = createdByUserName,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        };
    }

    public TaskActionItem CopyAsOpen(Guid taskId, DateTime? expectedCompletionAt, Guid createdByUserId, string createdByUserName)
    {
        return Create(taskId, Title, expectedCompletionAt, createdByUserId, createdByUserName);
    }

    public void Update(string title, DateTime? expectedCompletionAt, Guid modifiedByUserId)
    {
        Title = title.Trim();
        ExpectedCompletionAt = expectedCompletionAt;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void SetCompletion(bool isCompleted, Guid modifiedByUserId)
    {
        IsCompleted = isCompleted;
        CompletedAt = isCompleted ? DateTime.UtcNow : null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public void Remove(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedByUserId.ToString();
    }
}
