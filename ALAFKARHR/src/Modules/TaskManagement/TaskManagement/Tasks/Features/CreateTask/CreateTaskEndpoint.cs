namespace TaskManagement.Tasks.Features.CreateTask;

public record CreateTaskRequest(CreateTaskItemDto Task);
public record CreateTaskResponse(Guid Id);

public class CreateTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/taskmanagement/tasks", async (CreateTaskRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateTaskCommand>());
            return Results.Created($"/api/v1/taskmanagement/tasks/{result.Id}", result.Adapt<CreateTaskResponse>());
        })
        .WithName("CreateTask")
        .Produces<CreateTaskResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Task")
        .WithDescription("Create task management item")
        .RequireAuthorization(PermissionList.TaskManagementPermissions.Create);
    }
}
