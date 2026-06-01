namespace TaskManagement.Tasks.Models;

public class TaskNotification : Entity<Guid>
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public string NotificationType { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime CreatedDate { get; private set; }

    private TaskNotification()
    {
    }

    public static TaskNotification Create(Guid taskId, Guid userId, string notificationType, string message)
    {
        return new TaskNotification
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            NotificationType = notificationType,
            Message = message,
            CreatedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };
    }
}
