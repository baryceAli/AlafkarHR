namespace TaskManagement.Tasks.Features.Dashboard;

public class GetTaskDashboardEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/taskmanagement/dashboard", async (string? scope, Guid? departmentId, ISender sender) =>
        {
            var result = await sender.Send(new GetTaskDashboardQuery(scope ?? "mine", departmentId));
            return Results.Ok(result);
        })
        .WithName("GetTaskDashboard")
        .Produces<GetTaskDashboardResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);
    }
}
