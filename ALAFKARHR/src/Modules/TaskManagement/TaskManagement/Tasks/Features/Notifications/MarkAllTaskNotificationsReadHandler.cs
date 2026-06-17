namespace TaskManagement.Tasks.Features.Notifications;

public record MarkAllTaskNotificationsReadCommand : ICommand<MarkAllTaskNotificationsReadResult>;
public record MarkAllTaskNotificationsReadResult(int UpdatedCount);

public class MarkAllTaskNotificationsReadHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<MarkAllTaskNotificationsReadCommand, MarkAllTaskNotificationsReadResult>
{
    public async Task<MarkAllTaskNotificationsReadResult> Handle(MarkAllTaskNotificationsReadCommand command, CancellationToken cancellationToken)
    {
        var userCode = TaskFeatureHelpers.GetCurrentUserName(httpContextAccessor);
        var notifications = await dbContext.TaskNotifications
            .Where(x => x.UserCode == userCode && !x.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.MarkRead(userCode);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new MarkAllTaskNotificationsReadResult(notifications.Count);
    }
}

