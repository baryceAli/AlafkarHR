namespace DocumentManagement.Documents.Features;

public class GetDocumentsHandler(DocumentManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetDocumentsQuery, GetDocumentsResult>
{
    public async Task<GetDocumentsResult> Handle(GetDocumentsQuery query, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var manageAll = DocumentFeatureHelpers.HasManageAll(httpContextAccessor);

        var documents = dbContext.Documents.AsNoTracking()
            .IncludeDetails()
            .ApplyVisibility(companyId, currentUserId, manageAll);

        documents = query.Scope switch
        {
            DocumentListScope.OwnedByMe => documents.Where(x => x.OwnerUserId == currentUserId),
            DocumentListScope.SharedWithMe => documents.Where(x => x.OwnerUserId != currentUserId && x.Collaborators.Any(c => c.UserId == currentUserId && !c.IsDeleted)),
            _ => documents
        };

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var searchText = query.SearchText.Trim();
            documents = documents.Where(x =>
                x.Title.Contains(searchText)
                || (x.Description != null && x.Description.Contains(searchText))
                || x.Versions.Any(v => v.OriginalFileName.Contains(searchText)));
        }

        if (!string.IsNullOrWhiteSpace(query.SourceModule))
            documents = documents.Where(x => x.SourceModule == query.SourceModule.Trim());
        if (!string.IsNullOrWhiteSpace(query.SourceEntity))
            documents = documents.Where(x => x.SourceEntity == query.SourceEntity.Trim());
        if (query.SourceRecordId.HasValue)
            documents = documents.Where(x => x.SourceRecordId == query.SourceRecordId.Value);

        var totalCount = await documents.CountAsync(cancellationToken);
        var items = await documents
            .OrderByDescending(x => x.ModifiedAt ?? x.CreatedAt)
            .Skip(query.Pagination.PageIndex * query.Pagination.PageSize)
            .Take(query.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(x => DocumentFeatureHelpers.ToListDto(x, currentUserId, manageAll)).ToList();
        return new GetDocumentsResult(new PaginatedResult<DocumentItemDto>(query.Pagination.PageIndex, query.Pagination.PageSize, totalCount, dtos));
    }
}

public class GetDocumentByIdHandler(DocumentManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetDocumentByIdQuery, GetDocumentByIdResult>
{
    public async Task<GetDocumentByIdResult> Handle(GetDocumentByIdQuery query, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var manageAll = DocumentFeatureHelpers.HasManageAll(httpContextAccessor);

        var document = await dbContext.Documents.AsNoTracking()
            .IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == query.Id && x.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException($"Document not found: {query.Id}");

        DocumentFeatureHelpers.EnsureCanView(document, currentUserId, manageAll);
        return new GetDocumentByIdResult(DocumentFeatureHelpers.ToDetailDto(document, currentUserId, manageAll));
    }
}

public class CreateDocumentHandler(
    DocumentManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IWebHostEnvironment environment)
    : ICommandHandler<CreateDocumentCommand, CreateDocumentResult>
{
    public async Task<CreateDocumentResult> Handle(CreateDocumentCommand command, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        if (command.Document.CompanyId != companyId)
            throw new BadRequestException("Document company does not match the signed-in company.");

        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var document = DocumentItem.Create(companyId, currentUserId, command.Document);
        var provisionalVersionId = Guid.NewGuid();
        var savedFile = await DocumentFeatureHelpers.SaveVersionFileAsync(command.File, environment, companyId, document.Id, provisionalVersionId, cancellationToken);
        document.AddVersion(command.File.FileName, savedFile.StoragePath, command.File.ContentType, savedFile.FileSize, currentUserId);

        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateDocumentResult(document.Id);
    }
}

public class UpdateDocumentHandler(DocumentManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateDocumentCommand, UpdateDocumentResult>
{
    public async Task<UpdateDocumentResult> Handle(UpdateDocumentCommand command, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var manageAll = DocumentFeatureHelpers.HasManageAll(httpContextAccessor);

        var document = await dbContext.Documents.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException($"Document not found: {command.Id}");

        DocumentFeatureHelpers.EnsureCanWrite(document, currentUserId, manageAll);
        command.Document.Id = command.Id;
        document.Update(command.Document, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateDocumentResult(true);
    }
}

public class UploadDocumentVersionHandler(
    DocumentManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IWebHostEnvironment environment)
    : ICommandHandler<UploadDocumentVersionCommand, UploadDocumentVersionResult>
{
    public async Task<UploadDocumentVersionResult> Handle(UploadDocumentVersionCommand command, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var manageAll = DocumentFeatureHelpers.HasManageAll(httpContextAccessor);

        var document = await dbContext.Documents.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException($"Document not found: {command.Id}");

        DocumentFeatureHelpers.EnsureCanWrite(document, currentUserId, manageAll);
        var provisionalVersionId = Guid.NewGuid();
        var savedFile = await DocumentFeatureHelpers.SaveVersionFileAsync(command.File, environment, companyId, document.Id, provisionalVersionId, cancellationToken);
        var version = document.AddVersion(command.File.FileName, savedFile.StoragePath, command.File.ContentType, savedFile.FileSize, currentUserId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UploadDocumentVersionResult(version.Id, version.VersionNumber);
    }
}

public class InviteDocumentCollaboratorHandler(DocumentManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<InviteDocumentCollaboratorCommand, InviteDocumentCollaboratorResult>
{
    public async Task<InviteDocumentCollaboratorResult> Handle(InviteDocumentCollaboratorCommand command, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var manageAll = DocumentFeatureHelpers.HasManageAll(httpContextAccessor);

        var document = await dbContext.Documents.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == command.DocumentId && x.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException($"Document not found: {command.DocumentId}");

        DocumentFeatureHelpers.EnsureCanShare(document, currentUserId, manageAll);
        var collaborator = document.AddOrUpdateCollaborator(command.Collaborator.UserId, command.Collaborator.UserName, command.Collaborator.AccessLevel, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new InviteDocumentCollaboratorResult(collaborator.Id);
    }
}

public class RemoveDocumentCollaboratorHandler(DocumentManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RemoveDocumentCollaboratorCommand, RemoveDocumentCollaboratorResult>
{
    public async Task<RemoveDocumentCollaboratorResult> Handle(RemoveDocumentCollaboratorCommand command, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var manageAll = DocumentFeatureHelpers.HasManageAll(httpContextAccessor);

        var document = await dbContext.Documents.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == command.DocumentId && x.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException($"Document not found: {command.DocumentId}");

        DocumentFeatureHelpers.EnsureCanShare(document, currentUserId, manageAll);
        document.RemoveCollaborator(command.CollaboratorId, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RemoveDocumentCollaboratorResult(true);
    }
}

public class DeleteDocumentHandler(DocumentManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteDocumentCommand, DeleteDocumentResult>
{
    public async Task<DeleteDocumentResult> Handle(DeleteDocumentCommand command, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var manageAll = DocumentFeatureHelpers.HasManageAll(httpContextAccessor);

        var document = await dbContext.Documents.IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException($"Document not found: {command.Id}");

        DocumentFeatureHelpers.EnsureCanShare(document, currentUserId, manageAll);
        document.Remove(currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteDocumentResult(true);
    }
}

public class DownloadDocumentVersionHandler(DocumentManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<DownloadDocumentVersionQuery, DownloadDocumentVersionResult>
{
    public async Task<DownloadDocumentVersionResult> Handle(DownloadDocumentVersionQuery query, CancellationToken cancellationToken)
    {
        var companyId = DocumentFeatureHelpers.CurrentCompanyId(httpContextAccessor);
        var currentUserId = DocumentFeatureHelpers.CurrentUserId(httpContextAccessor);
        var manageAll = DocumentFeatureHelpers.HasManageAll(httpContextAccessor);

        var document = await dbContext.Documents.AsNoTracking()
            .IncludeDetails()
            .FirstOrDefaultAsync(x => x.Id == query.Id && x.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException($"Document not found: {query.Id}");

        DocumentFeatureHelpers.EnsureCanView(document, currentUserId, manageAll);

        var version = query.VersionId.HasValue
            ? document.Versions.FirstOrDefault(x => x.Id == query.VersionId.Value)
            : document.Versions.OrderByDescending(x => x.VersionNumber).FirstOrDefault();

        if (version is null)
            throw new NotFoundException("Document version not found.");
        if (!File.Exists(version.StoragePath))
            throw new NotFoundException("Document file not found in storage.");

        return new DownloadDocumentVersionResult(version.StoragePath, version.OriginalFileName, version.ContentType);
    }
}
