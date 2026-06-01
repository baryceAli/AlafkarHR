namespace TaskManagement.Tasks.Features.MarkOverdueTasks;

public class MarkOverdueTasksEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/taskmanagement/tasks/mark-overdue", async (ISender sender) =>
        {
            var result = await sender.Send(new MarkOverdueTasksCommand());
            return Results.Ok(result);
        })
        .WithName("MarkOverdueTasks")
        .Produces<MarkOverdueTasksResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.ManageAllTasks);
    }
}
