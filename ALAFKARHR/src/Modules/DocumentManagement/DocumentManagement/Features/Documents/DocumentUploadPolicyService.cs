namespace DocumentManagement.Documents.Features;

public interface IDocumentUploadPolicyService
{
    Task<DocumentUploadPolicyDto> GetEffectivePolicyAsync(Guid companyId, CancellationToken cancellationToken);
    DocumentUploadPolicyDto GetDefaultPolicy(Guid companyId);
    Task<DocumentUploadPolicyDto> UpsertPolicyAsync(Guid companyId, Guid userId, UpdateDocumentUploadPolicyDto dto, CancellationToken cancellationToken);
}

public class DocumentUploadPolicyService(
    DocumentManagementDbContext dbContext,
    IOptions<DocumentStorageOptions> storageOptions) : IDocumentUploadPolicyService
{
    public async Task<DocumentUploadPolicyDto> GetEffectivePolicyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var policy = await dbContext.DocumentUploadPolicies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);

        return policy?.ToDto(storageOptions.Value.Provider, false) ?? GetDefaultPolicy(companyId);
    }

    public DocumentUploadPolicyDto GetDefaultPolicy(Guid companyId) => DefaultPolicy(companyId);

    public async Task<DocumentUploadPolicyDto> UpsertPolicyAsync(Guid companyId, Guid userId, UpdateDocumentUploadPolicyDto dto, CancellationToken cancellationToken)
    {
        Validate(dto);
        var normalized = DocumentUploadPolicy.Normalize(dto);
        var policy = await dbContext.DocumentUploadPolicies
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);

        if (policy is null)
        {
            policy = DocumentUploadPolicy.Create(companyId, normalized, userId);
            dbContext.DocumentUploadPolicies.Add(policy);
        }
        else
        {
            policy.Update(normalized, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return policy.ToDto(storageOptions.Value.Provider, false);
    }

    private DocumentUploadPolicyDto DefaultPolicy(Guid companyId)
    {
        var options = storageOptions.Value;
        return new DocumentUploadPolicyDto
        {
            CompanyId = companyId,
            StorageProvider = options.Provider,
            IsDefault = true,
            MaxFileSizeBytes = options.MaxFileSizeBytes,
            AllowedExtensions = DocumentUploadPolicy.NormalizeExtensions(options.AllowedExtensions),
            AllowedContentTypes = DocumentUploadPolicy.NormalizeContentTypes(options.AllowedContentTypes)
        };
    }

    private static void Validate(UpdateDocumentUploadPolicyDto dto)
    {
        if (dto.MaxFileSizeBytes <= 0)
            throw new BadRequestException("Maximum file size must be greater than zero.");

        var extensions = DocumentUploadPolicy.NormalizeExtensions(dto.AllowedExtensions);
        if (extensions.Count == 0)
            throw new BadRequestException("At least one allowed extension is required.");

        if (extensions.Any(x => !x.StartsWith('.') || x.Contains('/') || x.Contains('\\')))
            throw new BadRequestException("Extensions must start with a dot and cannot contain path separators.");

        var contentTypes = DocumentUploadPolicy.NormalizeContentTypes(dto.AllowedContentTypes);
        if (contentTypes.Count == 0)
            throw new BadRequestException("At least one allowed content type is required.");

        if (contentTypes.Any(x => x != "application/octet-stream" && !x.Contains('/')))
            throw new BadRequestException("Content types must be valid MIME values.");
    }
}
