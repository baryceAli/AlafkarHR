namespace TaskManagement.Tasks.Features.GenerateRecurringTasks;

public class GenerateRecurringTasksEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/taskmanagement/tasks/generate-recurring", async (ISender sender) =>
        {
            var result = await sender.Send(new GenerateRecurringTasksCommand());
            return Results.Ok(result);
        })
        .WithName("GenerateRecurringTasks")
        .Produces<GenerateRecurringTasksResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.ManageAllTasks);
    }
}

