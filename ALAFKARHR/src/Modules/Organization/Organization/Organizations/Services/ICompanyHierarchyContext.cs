namespace Organization.Organizations.Services;

public interface ICompanyHierarchyContext
{
    Task<Guid> GetCurrentParentCompanyIdAsync(CancellationToken cancellationToken);
    Task<Guid> GetParentCompanyIdForCompanyAsync(Guid companyId, CancellationToken cancellationToken);
    Task<List<Guid>> GetCompanyHierarchyIdsAsync(Guid parentCompanyId, CancellationToken cancellationToken);
}
