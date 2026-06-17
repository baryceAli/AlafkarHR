namespace TaskManagement.Tasks.Models;

public class TaskNotification : Entity<Guid>
{
    public Guid TaskId { get; private set; }
    public string UserCode { get; private set; } = string.Empty;
    public string NotificationType { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime CreatedDate { get; private set; }

    private TaskNotification()
    {
    }

    public static TaskNotification Create(Guid taskId, string userCode, string notificationType, string message,string createdBy)
    {
        return new TaskNotification
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserCode = userCode,
            NotificationType = notificationType,
            Message = message,
            CreatedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void MarkRead(string userCode)
    {
        if (!string.Equals(UserCode, userCode, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Notification is not visible to the current user.");

        IsRead = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userCode;
    }
}
