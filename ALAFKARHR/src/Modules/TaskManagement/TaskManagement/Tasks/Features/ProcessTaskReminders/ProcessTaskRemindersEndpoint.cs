namespace TaskManagement.Tasks.Features.ProcessTaskReminders;

public class ProcessTaskRemindersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/taskmanagement/tasks/process-reminders", async (ISender sender) =>
        {
            var result = await sender.Send(new ProcessTaskRemindersCommand());
            return Results.Ok(result);
        })
        .WithName("ProcessTaskReminders")
        .Produces<ProcessTaskRemindersResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.ManageAllTasks);
    }
}

