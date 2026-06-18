namespace DocumentManagement.Documents.Features;

public record GetDocumentsQuery(
    PaginationRequest Pagination,
    string? SearchText,
    string? SourceModule,
    string? SourceEntity,
    Guid? SourceRecordId,
    DocumentListScope Scope) : IQuery<GetDocumentsResult>;

public record GetDocumentsResult(PaginatedResult<DocumentItemDto> Documents);

public record GetDocumentByIdQuery(Guid Id) : IQuery<GetDocumentByIdResult>;
public record GetDocumentByIdResult(DocumentDetailDto Document);

public record CreateDocumentCommand(CreateDocumentDto Document, IFormFile File) : ICommand<CreateDocumentResult>;
public record CreateDocumentResult(Guid Id);

public record UpdateDocumentCommand(Guid Id, UpdateDocumentDto Document) : ICommand<UpdateDocumentResult>;
public record UpdateDocumentResult(bool IsSuccess);

public record UploadDocumentVersionCommand(Guid Id, IFormFile File) : ICommand<UploadDocumentVersionResult>;
public record UploadDocumentVersionResult(Guid Id, int VersionNumber);

public record InviteDocumentCollaboratorCommand(Guid DocumentId, InviteDocumentCollaboratorDto Collaborator) : ICommand<InviteDocumentCollaboratorResult>;
public record InviteDocumentCollaboratorResult(Guid Id);

public record RemoveDocumentCollaboratorCommand(Guid DocumentId, Guid CollaboratorId) : ICommand<RemoveDocumentCollaboratorResult>;
public record RemoveDocumentCollaboratorResult(bool IsSuccess);

public record DeleteDocumentCommand(Guid Id) : ICommand<DeleteDocumentResult>;
public record DeleteDocumentResult(bool IsSuccess);

public record DownloadDocumentVersionQuery(Guid Id, Guid? VersionId) : IQuery<DownloadDocumentVersionResult>;
public record DownloadDocumentVersionResult(string StoragePath, string OriginalFileName, string ContentType);

public class CreateDocumentValidator : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentValidator()
    {
        RuleFor(x => x.Document.CompanyId).NotEmpty();
        RuleFor(x => x.Document.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Document.Description).MaximumLength(1000);
        RuleFor(x => x.Document.SourceModule).MaximumLength(100);
        RuleFor(x => x.Document.SourceEntity).MaximumLength(100);
        RuleFor(x => x.File).NotNull();
    }
}

public class UpdateDocumentValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Document.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Document.Description).MaximumLength(1000);
        RuleFor(x => x.Document.SourceModule).MaximumLength(100);
        RuleFor(x => x.Document.SourceEntity).MaximumLength(100);
    }
}

public class InviteDocumentCollaboratorValidator : AbstractValidator<InviteDocumentCollaboratorCommand>
{
    public InviteDocumentCollaboratorValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty();
        RuleFor(x => x.Collaborator.UserId).NotEmpty();
        RuleFor(x => x.Collaborator.UserName).MaximumLength(256);
        RuleFor(x => x.Collaborator.AccessLevel).IsInEnum();
    }
}
