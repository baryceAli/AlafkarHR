namespace DocumentManagement.Documents.Models;

public class DocumentVersion : Entity<Guid>
{
    public Guid DocumentId { get; private set; }
    public int VersionNumber { get; private set; }
    public string OriginalFileName { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public string StorageProvider { get; private set; } = DocumentStorageProviders.LocalFileSystem;
    public string? StorageKey { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private DocumentVersion()
    {
    }

    public static DocumentVersion Create(
        Guid documentId,
        int versionNumber,
        string originalFileName,
        string storagePath,
        string storageProvider,
        string storageKey,
        string contentType,
        long fileSize,
        Guid uploadedByUserId)
    {
        return new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            VersionNumber = versionNumber,
            OriginalFileName = Path.GetFileName(originalFileName),
            StoragePath = storagePath,
            StorageProvider = storageProvider,
            StorageKey = storageKey,
            ContentType = contentType,
            FileSize = fileSize,
            UploadedByUserId = uploadedByUserId,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = uploadedByUserId.ToString()
        };
    }
}
