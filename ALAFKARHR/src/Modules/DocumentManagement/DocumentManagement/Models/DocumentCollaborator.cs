namespace DocumentManagement.Documents.Models;

public class DocumentCollaborator : Entity<Guid>
{
    public Guid DocumentId { get; private set; }
    public Guid UserId { get; private set; }
    public string? UserName { get; private set; }
    public DocumentAccessLevel AccessLevel { get; private set; }

    private DocumentCollaborator()
    {
    }

    public static DocumentCollaborator Create(Guid documentId, Guid userId, string? userName, DocumentAccessLevel accessLevel, Guid actorUserId)
    {
        return new DocumentCollaborator
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            UserId = userId,
            UserName = Normalize(userName),
            AccessLevel = accessLevel,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = actorUserId.ToString()
        };
    }

    public void Update(string? userName, DocumentAccessLevel accessLevel, Guid actorUserId)
    {
        UserName = Normalize(userName);
        AccessLevel = accessLevel;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = actorUserId.ToString();
    }

    public void Remove(Guid actorUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = actorUserId.ToString();
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
