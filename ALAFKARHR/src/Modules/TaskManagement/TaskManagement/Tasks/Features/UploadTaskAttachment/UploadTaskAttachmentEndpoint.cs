namespace TaskManagement.Tasks.Features.UploadTaskAttachment;

public class UploadTaskAttachmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/taskmanagement/tasks/{id:guid}/attachments", async (Guid id, IFormFile file, ISender sender) =>
        {
            var result = await sender.Send(new UploadTaskAttachmentCommand(id, file));
            return Results.Created($"/api/v1/taskmanagement/tasks/{id}", result);
        })
        .DisableAntiforgery()
        .WithName("UploadTaskAttachment")
        .Produces<UploadTaskAttachmentResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.TaskManagementPermissions.Comment);
    }
}
