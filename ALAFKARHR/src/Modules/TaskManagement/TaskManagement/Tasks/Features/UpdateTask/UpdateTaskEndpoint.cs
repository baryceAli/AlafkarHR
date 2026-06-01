namespace TaskManagement.Tasks.Features.UpdateTask;

public record UpdateTaskRequest(UpdateTaskItemDto Task);

public class UpdateTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/taskmanagement/tasks", async (UpdateTaskRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateTaskCommand>());
            return Results.Ok(result);
        })
        .WithName("UpdateTask")
        .Produces<UpdateTaskResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.Edit);
    }
}
