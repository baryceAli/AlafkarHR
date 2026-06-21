namespace DocumentManagement.Storage;

public record DocumentStorageSaveRequest(
    IFormFile File,
    Guid CompanyId,
    Guid DocumentId,
    Guid VersionId);

public record DocumentStorageSaveResult(
    string Provider,
    string StorageKey,
    string StoragePath,
    long FileSize);

public record DocumentStorageReadResult(
    Stream Stream,
    string StoragePath);

public interface IDocumentStorageProvider
{
    string ProviderName { get; }
    Task<DocumentStorageSaveResult> SaveAsync(DocumentStorageSaveRequest request, CancellationToken cancellationToken);
    Task<DocumentStorageReadResult> OpenReadAsync(DocumentVersion version, CancellationToken cancellationToken);
    Task DeleteAsync(DocumentVersion version, CancellationToken cancellationToken);
}
