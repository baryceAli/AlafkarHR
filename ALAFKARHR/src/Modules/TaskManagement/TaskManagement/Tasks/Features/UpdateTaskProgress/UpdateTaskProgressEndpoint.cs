namespace TaskManagement.Tasks.Features.UpdateTaskProgress;

public record UpdateTaskProgressRequest(UpdateTaskProgressDto Progress);

public class UpdateTaskProgressEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/taskmanagement/tasks/{id:guid}/progress", async (Guid id, UpdateTaskProgressRequest request, ISender sender) =>
        {
            request.Progress.Id = id;
            var result = await sender.Send(request.Adapt<UpdateTaskProgressCommand>());
            return Results.Ok(result);
        })
        .WithName("UpdateTaskProgress")
        .Produces<UpdateTaskProgressResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.Comment);
    }
}
