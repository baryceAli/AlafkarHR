using TaskManagement.Tasks.Features.GetTasks;

namespace TaskManagement.Tasks.Features.GetMyTasks;

public class GetMyTasksEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/taskmanagement/tasks/my", async (int PageIndex, int PageSize, string? searchText, ISender sender, IHttpContextAccessor accessor) =>
        {
            var userCode = TaskManagement.Tasks.Features.TaskFeatureHelpers.GetCurrentUserName(accessor);
            var result = await sender.Send(new GetTasksQuery(
                new PaginationRequest(PageIndex, PageSize, searchText),
                new TaskFilterDto { AssignedToUser = userCode }));

            return Results.Ok(result);
        })
        .WithName("GetMyTasks")
        .Produces<GetTasksResult>(StatusCodes.Status200OK)
        .WithSummary("Get My Tasks")
        .WithDescription("Get tasks assigned to current user")
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);
    }
}
