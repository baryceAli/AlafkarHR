namespace TaskManagement.Tasks.Features.TaskActions;

public record CreateTaskActionRequest(CreateTaskActionDto Action);
public record UpdateTaskActionRequest(UpdateTaskActionDto Action);
public record ToggleTaskActionCompletionRequest(ToggleTaskActionCompletionDto Action);

public class TaskActionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/taskmanagement/tasks/{taskId:guid}/actions", async (Guid taskId, CreateTaskActionRequest request, ISender sender) =>
        {
            request.Action.TaskId = taskId;
            var result = await sender.Send(new CreateTaskActionCommand(request.Action));
            return Results.Created($"/api/v1/taskmanagement/tasks/{taskId}", result);
        })
        .WithName("CreateTaskAction")
        .Produces<CreateTaskActionResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);

        app.MapPut("/api/v1/taskmanagement/tasks/{taskId:guid}/actions/{actionId:guid}", async (Guid taskId, Guid actionId, UpdateTaskActionRequest request, ISender sender) =>
        {
            request.Action.TaskId = taskId;
            request.Action.Id = actionId;
            var result = await sender.Send(new UpdateTaskActionCommand(request.Action));
            return Results.Ok(result);
        })
        .WithName("UpdateTaskAction")
        .Produces<UpdateTaskActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);

        app.MapPut("/api/v1/taskmanagement/tasks/{taskId:guid}/actions/{actionId:guid}/completion", async (Guid taskId, Guid actionId, ToggleTaskActionCompletionRequest request, ISender sender) =>
        {
            request.Action.TaskId = taskId;
            request.Action.Id = actionId;
            var result = await sender.Send(new ToggleTaskActionCompletionCommand(request.Action));
            return Results.Ok(result);
        })
        .WithName("ToggleTaskActionCompletion")
        .Produces<ToggleTaskActionCompletionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);

        app.MapDelete("/api/v1/taskmanagement/tasks/{taskId:guid}/actions/{actionId:guid}", async (Guid taskId, Guid actionId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteTaskActionCommand(taskId, actionId));
            return Results.Ok(result);
        })
        .WithName("DeleteTaskAction")
        .Produces<DeleteTaskActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);
    }
}
