using Microsoft.Extensions.Options;

namespace DocumentManagement.Storage;

public class LocalDocumentStorageProvider(IWebHostEnvironment environment, IOptions<DocumentStorageOptions> options) : IDocumentStorageProvider
{
    public string ProviderName => DocumentStorageProviders.LocalFileSystem;

    public async Task<DocumentStorageSaveResult> SaveAsync(DocumentStorageSaveRequest request, CancellationToken cancellationToken)
    {
        var storageKey = BuildStorageKey(request.CompanyId, request.DocumentId, request.VersionId, request.File.FileName);
        var physicalPath = ResolveStorageKey(storageKey);
        var directory = Path.GetDirectoryName(physicalPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using var stream = File.Create(physicalPath);
        await request.File.CopyToAsync(stream, cancellationToken);

        return new DocumentStorageSaveResult(ProviderName, storageKey, physicalPath, request.File.Length);
    }

    public Task<DocumentStorageReadResult> OpenReadAsync(DocumentVersion version, CancellationToken cancellationToken)
    {
        var physicalPath = ResolveVersionPath(version);
        if (!File.Exists(physicalPath))
            throw new NotFoundException("Document file not found in storage.");

        Stream stream = File.OpenRead(physicalPath);
        return Task.FromResult(new DocumentStorageReadResult(stream, physicalPath));
    }

    public Task DeleteAsync(DocumentVersion version, CancellationToken cancellationToken)
    {
        var physicalPath = ResolveVersionPath(version);
        if (File.Exists(physicalPath))
            File.Delete(physicalPath);

        return Task.CompletedTask;
    }

    private string ResolveVersionPath(DocumentVersion version)
    {
        if (!string.IsNullOrWhiteSpace(version.StorageKey))
            return ResolveStorageKey(version.StorageKey);

        if (!string.IsNullOrWhiteSpace(version.StoragePath))
            return version.StoragePath;

        throw new NotFoundException("Document storage location is missing.");
    }

    private string ResolveStorageKey(string storageKey)
    {
        var root = GetRootPath();
        var safeKey = storageKey.Replace('/', Path.DirectorySeparatorChar);
        var rootPath = Path.GetFullPath(root);
        var physicalPath = Path.GetFullPath(Path.Combine(rootPath, safeKey));
        if (!physicalPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Document storage key is invalid.");

        return physicalPath;
    }

    private string GetRootPath()
    {
        var configuredPath = options.Value.LocalRootPath;
        if (string.IsNullOrWhiteSpace(configuredPath))
            return Path.Combine(environment.ContentRootPath, "App_Data", "DocumentManagement");

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(environment.ContentRootPath, configuredPath);
    }

    private static string BuildStorageKey(Guid companyId, Guid documentId, Guid versionId, string originalFileName)
    {
        var fileName = Path.GetFileName(originalFileName);
        return string.Join('/', companyId.ToString(), documentId.ToString(), versionId.ToString(), fileName);
    }
}
