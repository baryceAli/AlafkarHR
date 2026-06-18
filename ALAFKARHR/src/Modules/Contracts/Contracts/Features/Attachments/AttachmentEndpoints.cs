namespace Contracts.Contracts.Features.Attachments;

public class AttachmentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var baseRoute = "/api/v1/contracts/contracts/{id:guid}/attachments";

        app.MapPost(baseRoute, async (Guid id, ContractAttachmentKind? kind, IFormFile file, ISender sender) =>
        {
            var result = await sender.Send(new UploadContractAttachmentCommand(id, kind ?? ContractAttachmentKind.SupportingDocument, file));
            return Results.Created($"/api/v1/contracts/contracts/{id}/attachments/{result.Id}", result);
        })
            .WithName("UploadContractAttachment")
            .Produces<UploadContractAttachmentResult>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.ContractPermissions.Edit)
            .DisableAntiforgery();

        app.MapDelete($"{baseRoute}/{{attachmentId:guid}}", async (Guid id, Guid attachmentId, ISender sender) =>
        {
            await sender.Send(new DeleteContractAttachmentCommand(id, attachmentId));
            return Results.Ok("OK");
        })
            .WithName("DeleteContractAttachment")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractPermissions.Edit);
    }
}
