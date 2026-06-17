namespace TaskManagement.Tasks.Features.Notifications;

public record MarkTaskNotificationReadCommand(Guid Id) : ICommand<MarkTaskNotificationReadResult>;
public record MarkTaskNotificationReadResult(bool IsSuccess);

public class MarkTaskNotificationReadHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<MarkTaskNotificationReadCommand, MarkTaskNotificationReadResult>
{
    public async Task<MarkTaskNotificationReadResult> Handle(MarkTaskNotificationReadCommand command, CancellationToken cancellationToken)
    {
        var userCode = TaskFeatureHelpers.GetCurrentUserName(httpContextAccessor);
        var notification = await dbContext.TaskNotifications
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.UserCode == userCode, cancellationToken)
            ?? throw new NotFoundException($"Notification not found: {command.Id}");

        notification.MarkRead(userCode);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new MarkTaskNotificationReadResult(true);
    }
}

