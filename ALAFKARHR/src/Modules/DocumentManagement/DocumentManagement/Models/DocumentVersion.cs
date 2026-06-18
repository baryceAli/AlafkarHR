namespace DocumentManagement.Documents.Models;

public class DocumentVersion : Entity<Guid>
{
    public Guid DocumentId { get; private set; }
    public int VersionNumber { get; private set; }
    public string OriginalFileName { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private DocumentVersion()
    {
    }

    public static DocumentVersion Create(Guid documentId, int versionNumber, string originalFileName, string storagePath, string contentType, long fileSize, Guid uploadedByUserId)
    {
        return new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            VersionNumber = versionNumber,
            OriginalFileName = Path.GetFileName(originalFileName),
            StoragePath = storagePath,
            ContentType = contentType,
            FileSize = fileSize,
            UploadedByUserId = uploadedByUserId,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = uploadedByUserId.ToString()
        };
    }
}
