namespace TaskManagement.Tasks.Models;

public class TaskHistory : Entity<Guid>
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public DateTime ActionDate { get; private set; }

    private TaskHistory()
    {
    }

    public static TaskHistory Create(Guid taskId, Guid userId, string action, string? oldValue, string? newValue)
    {
        return new TaskHistory
        {
            //Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            Action = action,
            OldValue = oldValue,
            NewValue = newValue,
            ActionDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };
    }
}
