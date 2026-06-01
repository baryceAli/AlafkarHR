using TaskManagement.Contracts.Features.GetTaskById;

namespace TaskManagement.Tasks.Features.GetTaskById;

public class GetTaskByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/taskmanagement/tasks/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetTaskByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetTaskById")
        .Produces<GetTaskByIdResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Task By Id")
        .WithDescription("Get task details with comments, attachments and history")
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);
    }
}
