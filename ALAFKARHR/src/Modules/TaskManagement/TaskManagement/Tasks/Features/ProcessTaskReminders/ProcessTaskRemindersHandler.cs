namespace TaskManagement.Tasks.Features.ProcessTaskReminders;

public record ProcessTaskRemindersCommand : ICommand<ProcessTaskRemindersResult>;
public record ProcessTaskRemindersResult(int NotificationCount);

public class ProcessTaskRemindersHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ProcessTaskRemindersCommand, ProcessTaskRemindersResult>
{
    public async Task<ProcessTaskRemindersResult> Handle(ProcessTaskRemindersCommand command, CancellationToken cancellationToken)
    {
        var userId = TaskFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var now = DateTime.UtcNow;
        var tasks = await dbContext.TaskItems.Include(x => x.History)
            .Where(x => !x.IsDeleted
                && !x.IsArchived
                && x.ReminderDate.HasValue
                && x.ReminderDate.Value <= now
                && x.ReminderNotificationSentAt == null
                && x.Status != TaskWorkflowStatus.Completed
                && x.Status != TaskWorkflowStatus.Cancelled)
            .ToListAsync(cancellationToken);

        foreach (var task in tasks)
        {
            task.MarkReminderSent(userId);
            TaskFeatureHelpers.AddHistoryAndNotification(dbContext, task, userId, "TaskReminder", null, task.ReminderDate?.ToString("O"), task.AssignedToUser);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ProcessTaskRemindersResult(tasks.Count);
    }
}

