namespace TaskManagement.Tasks.Models;

public class TaskComment : Entity<Guid>
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public DateTime CreatedDate { get; private set; }

    private TaskComment()
    {
    }

    public static TaskComment Create(Guid taskId, Guid userId, string comment)
    {
        return new TaskComment
        {
            //Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            Comment = comment,
            CreatedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };
    }
}
