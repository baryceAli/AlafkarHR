namespace Contracts.Contracts.Features.Templates;

public record CreateContractTemplateRequest(ContractTemplateDto Template);
public record UpdateContractTemplateRequest(ContractTemplateDto Template);

public class TemplateEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var baseRoute = "/api/v1/contracts/templates";

        app.MapGet(baseRoute, async (Guid? companyId, string? contractType, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
        {
            var result = await sender.Send(new GetContractTemplatesQuery(companyId, contractType, pageIndex ?? 1, pageSize ?? 20, searchText));
            return Results.Ok(new { templates = result.Templates });
        })
            .WithName("GetContractTemplates")
            .Produces<PaginatedResult<ContractTemplateDto>>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractTemplatePermissions.View);

        app.MapGet($"{baseRoute}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetContractTemplateByIdQuery(id));
            return Results.Ok(new { template = result.Template });
        })
            .WithName("GetContractTemplateById")
            .Produces<ContractTemplateDto>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractTemplatePermissions.View);

        app.MapPost(baseRoute, async (CreateContractTemplateRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateContractTemplateCommand(request.Template));
            return Results.Created($"{baseRoute}/{result.Id}", result);
        })
            .WithName("CreateContractTemplate")
            .Produces<CreateContractTemplateResult>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.ContractTemplatePermissions.Create);

        app.MapPut($"{baseRoute}/{{id:guid}}", async (Guid id, UpdateContractTemplateRequest request, ISender sender) =>
        {
            await sender.Send(new UpdateContractTemplateCommand(id, request.Template));
            return Results.Ok("OK");
        })
            .WithName("UpdateContractTemplate")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractTemplatePermissions.Edit);

        app.MapDelete($"{baseRoute}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteContractTemplateCommand(id));
            return Results.Ok("OK");
        })
            .WithName("DeleteContractTemplate")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractTemplatePermissions.Delete);

        app.MapPost($"{baseRoute}/{{id:guid}}/file", async (Guid id, IFormFile file, ISender sender) =>
        {
            var result = await sender.Send(new UploadContractTemplateFileCommand(id, file));
            return Results.Ok(result);
        })
            .WithName("UploadContractTemplateFile")
            .Produces<UploadContractTemplateFileResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ContractTemplatePermissions.Edit)
            .DisableAntiforgery();
    }
}
