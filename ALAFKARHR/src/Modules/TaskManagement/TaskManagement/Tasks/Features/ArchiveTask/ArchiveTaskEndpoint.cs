namespace TaskManagement.Tasks.Features.ArchiveTask;

public class ArchiveTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/taskmanagement/tasks/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ArchiveTaskCommand(id));
            return Results.Ok(result);
        })
        .WithName("ArchiveTask")
        .Produces<ArchiveTaskResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.Delete);
    }
}
