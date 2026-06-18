namespace DocumentManagement.Documents.Models;

public class DocumentItem : Aggregate<Guid>
{
    private readonly List<DocumentVersion> _versions = [];
    private readonly List<DocumentCollaborator> _collaborators = [];

    public Guid CompanyId { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? SourceModule { get; private set; }
    public string? SourceEntity { get; private set; }
    public Guid? SourceRecordId { get; private set; }
    public IReadOnlyCollection<DocumentVersion> Versions => _versions;
    public IReadOnlyCollection<DocumentCollaborator> Collaborators => _collaborators;

    private DocumentItem()
    {
    }

    public static DocumentItem Create(Guid companyId, Guid ownerUserId, CreateDocumentDto dto)
    {
        return new DocumentItem
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            OwnerUserId = ownerUserId,
            Title = dto.Title.Trim(),
            Description = Normalize(dto.Description),
            SourceModule = Normalize(dto.SourceModule),
            SourceEntity = Normalize(dto.SourceEntity),
            SourceRecordId = dto.SourceRecordId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = ownerUserId.ToString()
        };
    }

    public void Update(UpdateDocumentDto dto, Guid modifiedByUserId)
    {
        Title = dto.Title.Trim();
        Description = Normalize(dto.Description);
        SourceModule = Normalize(dto.SourceModule);
        SourceEntity = Normalize(dto.SourceEntity);
        SourceRecordId = dto.SourceRecordId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public DocumentVersion AddVersion(string originalFileName, string storagePath, string contentType, long fileSize, Guid uploadedByUserId)
    {
        var version = DocumentVersion.Create(
            Id,
            _versions.Count == 0 ? 1 : _versions.Max(x => x.VersionNumber) + 1,
            originalFileName,
            storagePath,
            contentType,
            fileSize,
            uploadedByUserId);

        _versions.Add(version);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = uploadedByUserId.ToString();
        return version;
    }

    public DocumentCollaborator AddOrUpdateCollaborator(Guid userId, string? userName, DocumentAccessLevel accessLevel, Guid actorUserId)
    {
        if (userId == OwnerUserId)
            throw new BadRequestException("Owner already has full document access.");

        var collaborator = _collaborators.FirstOrDefault(x => x.UserId == userId && !x.IsDeleted);
        if (collaborator is null)
        {
            collaborator = DocumentCollaborator.Create(Id, userId, userName, accessLevel, actorUserId);
            _collaborators.Add(collaborator);
        }
        else
        {
            collaborator.Update(userName, accessLevel, actorUserId);
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = actorUserId.ToString();
        return collaborator;
    }

    public void RemoveCollaborator(Guid collaboratorId, Guid actorUserId)
    {
        var collaborator = _collaborators.FirstOrDefault(x => x.Id == collaboratorId && !x.IsDeleted)
            ?? throw new NotFoundException($"Document collaborator not found: {collaboratorId}");

        collaborator.Remove(actorUserId);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = actorUserId.ToString();
    }

    public void Remove(Guid deletedByUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedByUserId.ToString();
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
