namespace TaskManagement.Tasks.Features.AssignTask;

public record AssignTaskRequest(AssignTaskDto Assignment);

public class AssignTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/taskmanagement/tasks/{id:guid}/assign", async (Guid id, AssignTaskRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AssignTaskCommand(id, request.Assignment));
            return Results.Ok(result);
        })
        .WithName("AssignTask")
        .Produces<AssignTaskResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.Assign);
    }
}
