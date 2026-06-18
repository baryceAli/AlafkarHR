namespace DocumentManagement.Documents.Features;

public record UpdateDocumentRequest(UpdateDocumentDto Document);
public record InviteDocumentCollaboratorRequest(InviteDocumentCollaboratorDto Collaborator);

public class DocumentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/documentmanagement/documents");

        group.MapGet("/", async (
            int? pageIndex,
            int? pageSize,
            string? searchText,
            string? sourceModule,
            string? sourceEntity,
            Guid? sourceRecordId,
            DocumentListScope? scope,
            ISender sender) =>
        {
            var pagination = new PaginationRequest(pageIndex ?? 0, pageSize ?? 20);
            var result = await sender.Send(new GetDocumentsQuery(pagination, searchText, sourceModule, sourceEntity, sourceRecordId, scope ?? DocumentListScope.All));
            return Results.Ok(new { documents = result.Documents });
        })
        .WithName("GetDocuments")
        .Produces<PaginatedResult<DocumentItemDto>>(StatusCodes.Status200OK)
        .RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetDocumentByIdQuery(id));
            return Results.Ok(new { document = result.Document });
        })
        .WithName("GetDocumentById")
        .Produces<DocumentDetailDto>(StatusCodes.Status200OK)
        .RequireAuthorization();

        group.MapPost("/", async ([AsParameters] CreateDocumentForm form, ISender sender) =>
        {
            var dto = new CreateDocumentDto
            {
                CompanyId = form.CompanyId,
                Title = form.Title,
                Description = form.Description,
                SourceModule = form.SourceModule,
                SourceEntity = form.SourceEntity,
                SourceRecordId = form.SourceRecordId
            };
            var result = await sender.Send(new CreateDocumentCommand(dto, form.File));
            return Results.Created($"/api/v1/documentmanagement/documents/{result.Id}", result);
        })
        .DisableAntiforgery()
        .WithName("CreateDocument")
        .Produces<CreateDocumentResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.DocumentManagementPermissions.Create);

        group.MapPut("/{id:guid}", async (Guid id, UpdateDocumentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateDocumentCommand(id, request.Document));
            return Results.Ok(result);
        })
        .WithName("UpdateDocument")
        .Produces<UpdateDocumentResult>(StatusCodes.Status200OK)
        .RequireAuthorization();

        group.MapPost("/{id:guid}/versions", async (Guid id, IFormFile file, ISender sender) =>
        {
            var result = await sender.Send(new UploadDocumentVersionCommand(id, file));
            return Results.Created($"/api/v1/documentmanagement/documents/{id}/versions/{result.Id}", result);
        })
        .DisableAntiforgery()
        .WithName("UploadDocumentVersion")
        .Produces<UploadDocumentVersionResult>(StatusCodes.Status201Created)
        .RequireAuthorization();

        group.MapPost("/{id:guid}/collaborators", async (Guid id, InviteDocumentCollaboratorRequest request, ISender sender) =>
        {
            var result = await sender.Send(new InviteDocumentCollaboratorCommand(id, request.Collaborator));
            return Results.Ok(result);
        })
        .WithName("InviteDocumentCollaborator")
        .Produces<InviteDocumentCollaboratorResult>(StatusCodes.Status200OK)
        .RequireAuthorization();

        group.MapDelete("/{id:guid}/collaborators/{collaboratorId:guid}", async (Guid id, Guid collaboratorId, ISender sender) =>
        {
            var result = await sender.Send(new RemoveDocumentCollaboratorCommand(id, collaboratorId));
            return Results.Ok(result);
        })
        .WithName("RemoveDocumentCollaborator")
        .Produces<RemoveDocumentCollaboratorResult>(StatusCodes.Status200OK)
        .RequireAuthorization();

        group.MapGet("/{id:guid}/download", async (Guid id, Guid? versionId, ISender sender) =>
        {
            var result = await sender.Send(new DownloadDocumentVersionQuery(id, versionId));
            return Results.File(result.StoragePath, result.ContentType, result.OriginalFileName);
        })
        .WithName("DownloadDocument")
        .RequireAuthorization();

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteDocumentCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteDocument")
        .Produces<DeleteDocumentResult>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}

public class CreateDocumentForm
{
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SourceModule { get; set; }
    public string? SourceEntity { get; set; }
    public Guid? SourceRecordId { get; set; }
    public IFormFile File { get; set; } = default!;
}
