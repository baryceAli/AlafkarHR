namespace DocumentManagement.Documents.Models;

public class DocumentUploadPolicy : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public long MaxFileSizeBytes { get; private set; }
    public string AllowedExtensions { get; private set; } = string.Empty;
    public string AllowedContentTypes { get; private set; } = string.Empty;

    private DocumentUploadPolicy()
    {
    }

    public static DocumentUploadPolicy Create(Guid companyId, UpdateDocumentUploadPolicyDto dto, Guid createdByUserId)
    {
        var normalized = Normalize(dto);
        return new DocumentUploadPolicy
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            MaxFileSizeBytes = normalized.MaxFileSizeBytes,
            AllowedExtensions = string.Join(';', normalized.AllowedExtensions),
            AllowedContentTypes = string.Join(';', normalized.AllowedContentTypes),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        };
    }

    public void Update(UpdateDocumentUploadPolicyDto dto, Guid modifiedByUserId)
    {
        var normalized = Normalize(dto);
        MaxFileSizeBytes = normalized.MaxFileSizeBytes;
        AllowedExtensions = string.Join(';', normalized.AllowedExtensions);
        AllowedContentTypes = string.Join(';', normalized.AllowedContentTypes);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedByUserId.ToString();
    }

    public DocumentUploadPolicyDto ToDto(string provider, bool isDefault)
    {
        return new DocumentUploadPolicyDto
        {
            CompanyId = CompanyId,
            StorageProvider = provider,
            IsDefault = isDefault,
            MaxFileSizeBytes = MaxFileSizeBytes,
            AllowedExtensions = Split(AllowedExtensions),
            AllowedContentTypes = Split(AllowedContentTypes)
        };
    }

    public static UpdateDocumentUploadPolicyDto Normalize(UpdateDocumentUploadPolicyDto dto)
    {
        return new UpdateDocumentUploadPolicyDto
        {
            MaxFileSizeBytes = dto.MaxFileSizeBytes,
            AllowedExtensions = NormalizeExtensions(dto.AllowedExtensions),
            AllowedContentTypes = NormalizeContentTypes(dto.AllowedContentTypes)
        };
    }

    public static List<string> NormalizeExtensions(IEnumerable<string>? extensions) =>
        (extensions ?? [])
            .Select(x => x.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    public static List<string> NormalizeContentTypes(IEnumerable<string>? contentTypes) =>
        (contentTypes ?? [])
            .Select(x => x.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    private static List<string> Split(string value) =>
        value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}
