namespace TaskManagement.Tasks.Features.AddTaskComment;

public record AddTaskCommentRequest(CreateTaskCommentDto Comment);

public class AddTaskCommentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/taskmanagement/tasks/{id:guid}/comments", async (Guid id, AddTaskCommentRequest request, ISender sender) =>
        {
            request.Comment.TaskId = id;
            var result = await sender.Send(request.Adapt<AddTaskCommentCommand>());
            return Results.Created($"/api/v1/taskmanagement/tasks/{id}", result);
        })
        .WithName("AddTaskComment")
        .Produces<AddTaskCommentResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.Comment);
    }
}
