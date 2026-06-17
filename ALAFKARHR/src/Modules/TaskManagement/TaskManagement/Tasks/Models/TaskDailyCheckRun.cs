namespace TaskManagement.Tasks.Models;

public class TaskDailyCheckRun : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public DateTime CheckDate { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public DateTime? NextRetryAt { get; private set; }
    public string? LastError { get; private set; }

    private TaskDailyCheckRun()
    {
    }

    public static TaskDailyCheckRun Create(Guid userId, string userName, DateTime checkDate)
    {
        return new TaskDailyCheckRun
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = userName,
            CheckDate = checkDate.Date,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };
    }

    public bool ShouldSkip(DateTime now)
    {
        return IsCompleted || (NextRetryAt.HasValue && NextRetryAt.Value > now);
    }

    public void MarkAttempt()
    {
        AttemptCount++;
        LastAttemptAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = UserId.ToString();
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        NextRetryAt = null;
        LastError = null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = UserId.ToString();
    }

    public void MarkFailed(string error, DateTime nextRetryAt)
    {
        IsCompleted = false;
        LastError = error.Length > 2000 ? error[..2000] : error;
        NextRetryAt = nextRetryAt;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = UserId.ToString();
    }
}
