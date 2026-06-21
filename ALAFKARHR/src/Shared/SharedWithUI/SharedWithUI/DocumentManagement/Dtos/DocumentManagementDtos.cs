using SharedWithUI.DocumentManagement.Enums;

namespace SharedWithUI.DocumentManagement.Dtos;

public class DocumentItemDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SourceModule { get; set; }
    public string? SourceEntity { get; set; }
    public Guid? SourceRecordId { get; set; }
    public int LatestVersionNumber { get; set; }
    public string? LatestFileName { get; set; }
    public string? LatestContentType { get; set; }
    public long? LatestFileSize { get; set; }
    public DateTime? LatestUploadedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DocumentAccessLevel? CollaboratorAccessLevel { get; set; }
    public bool CanView { get; set; }
    public bool CanWrite { get; set; }
    public bool CanShare { get; set; }
    public bool CanDelete { get; set; }
    public bool CanDeleteStorage { get; set; }
}

public class DocumentDetailDto : DocumentItemDto
{
    public List<DocumentVersionDto> Versions { get; set; } = [];
    public List<DocumentCollaboratorDto> Collaborators { get; set; } = [];
}

public class DocumentVersionDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public int VersionNumber { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid UploadedByUserId { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class DocumentCollaboratorDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public DocumentAccessLevel AccessLevel { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CreateDocumentDto
{
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SourceModule { get; set; }
    public string? SourceEntity { get; set; }
    public Guid? SourceRecordId { get; set; }
}

public class UpdateDocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SourceModule { get; set; }
    public string? SourceEntity { get; set; }
    public Guid? SourceRecordId { get; set; }
}

public class InviteDocumentCollaboratorDto
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public DocumentAccessLevel AccessLevel { get; set; } = DocumentAccessLevel.Read;
}

public class DocumentUploadOptionsDto
{
    public long MaxFileSizeBytes { get; set; }
    public List<string> AllowedContentTypes { get; set; } = [];
    public List<string> AllowedExtensions { get; set; } = [];
}

public class DocumentUploadPolicyDto : DocumentUploadOptionsDto
{
    public Guid CompanyId { get; set; }
    public string StorageProvider { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class UpdateDocumentUploadPolicyDto
{
    public long MaxFileSizeBytes { get; set; }
    public List<string> AllowedContentTypes { get; set; } = [];
    public List<string> AllowedExtensions { get; set; } = [];
}
