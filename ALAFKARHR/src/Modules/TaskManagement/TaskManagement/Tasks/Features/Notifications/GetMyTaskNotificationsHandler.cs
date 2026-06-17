namespace TaskManagement.Tasks.Features.Notifications;

public record GetMyTaskNotificationsQuery(bool UnreadOnly, int PageIndex, int PageSize) : IQuery<GetMyTaskNotificationsResult>;
public record GetMyTaskNotificationsResult(PaginatedResult<TaskNotificationDto> Notifications);

public class GetMyTaskNotificationsHandler(TaskManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetMyTaskNotificationsQuery, GetMyTaskNotificationsResult>
{
    public async Task<GetMyTaskNotificationsResult> Handle(GetMyTaskNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userCode = TaskFeatureHelpers.GetCurrentUserName(httpContextAccessor);
        var query = dbContext.TaskNotifications
            .AsNoTracking()
            .Where(x => x.UserCode == userCode);

        if (request.UnreadOnly)
            query = query.Where(x => !x.IsRead);

        var count = await query.LongCountAsync(cancellationToken);
        var notifications = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new GetMyTaskNotificationsResult(new PaginatedResult<TaskNotificationDto>(
            request.PageIndex,
            request.PageSize,
            count,
            notifications.Adapt<List<TaskNotificationDto>>()));
    }
}

