namespace TaskManagement.Tasks.Features.DailyCheck;

public class RunMyTaskDailyCheckEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/taskmanagement/tasks/my/daily-check", async (ISender sender) =>
        {
            var result = await sender.Send(new RunMyTaskDailyCheckCommand());
            return Results.Ok(result);
        })
        .WithName("RunMyTaskDailyCheck")
        .Produces<RunMyTaskDailyCheckResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);
    }
}
