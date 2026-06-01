namespace TaskManagement.Tasks.Features.ChangeTaskStatus;

public record ChangeTaskStatusRequest(ChangeTaskStatusDto TaskWorkflowStatus);

public class ChangeTaskStatusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/taskmanagement/tasks/{id:guid}/status", async (Guid id, ChangeTaskStatusRequest request, ISender sender) =>
        {
            request.TaskWorkflowStatus.Id = id;
            var result = await sender.Send(request.Adapt<ChangeTaskStatusCommand>());
            return Results.Ok(result);
        })
        .WithName("ChangeTaskStatus")
        .Produces<ChangeTaskStatusResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.Edit);
    }
}
