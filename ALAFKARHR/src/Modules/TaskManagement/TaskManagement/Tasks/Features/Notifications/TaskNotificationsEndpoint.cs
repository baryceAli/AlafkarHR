namespace TaskManagement.Tasks.Features.Notifications;

public class TaskNotificationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/taskmanagement/notifications", async (bool? unreadOnly, int? PageIndex, int? PageSize, ISender sender) =>
        {
            var result = await sender.Send(new GetMyTaskNotificationsQuery(unreadOnly == true, PageIndex ?? 0, PageSize ?? 20));
            return Results.Ok(result);
        })
        .WithName("GetMyTaskNotifications")
        .Produces<GetMyTaskNotificationsResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);

        app.MapGet("/api/v1/taskmanagement/notifications/unread-count", async (ISender sender) =>
        {
            var result = await sender.Send(new GetTaskNotificationUnreadCountQuery());
            return Results.Ok(result);
        })
        .WithName("GetTaskNotificationUnreadCount")
        .Produces<GetTaskNotificationUnreadCountResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);

        app.MapPut("/api/v1/taskmanagement/notifications/{id:guid}/read", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new MarkTaskNotificationReadCommand(id));
            return Results.Ok(result);
        })
        .WithName("MarkTaskNotificationRead")
        .Produces<MarkTaskNotificationReadResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);

        app.MapPut("/api/v1/taskmanagement/notifications/read-all", async (ISender sender) =>
        {
            var result = await sender.Send(new MarkAllTaskNotificationsReadCommand());
            return Results.Ok(result);
        })
        .WithName("MarkAllTaskNotificationsRead")
        .Produces<MarkAllTaskNotificationsReadResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);
    }
}

