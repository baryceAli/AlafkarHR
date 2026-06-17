namespace TaskManagement.Tasks.Features.Notifications;

public record GetTaskNotificationUnreadCountQuery : IQuery<GetTaskNotificationUnreadCountResult>;
public record GetTaskNotificationUnreadCountResult(TaskNotificationUnreadCountDto Unread);

public class GetTaskNotificationUnreadCountHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetTaskNotificationUnreadCountQuery, GetTaskNotificationUnreadCountResult>
{
    public async Task<GetTaskNotificationUnreadCountResult> Handle(GetTaskNotificationUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var userCode = TaskFeatureHelpers.GetCurrentUserName(httpContextAccessor);
        var count = await dbContext.TaskNotifications.CountAsync(x => x.UserCode == userCode && !x.IsRead, cancellationToken);
        return new GetTaskNotificationUnreadCountResult(new TaskNotificationUnreadCountDto { Count = count });
    }
}

