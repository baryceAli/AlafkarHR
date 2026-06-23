namespace Shared.Contracts.Organization;

public interface ICompanyHierarchyReader
{
    Task<Guid> GetParentCompanyIdForCompanyAsync(Guid companyId, CancellationToken cancellationToken);
    Task<List<Guid>> GetCompanyHierarchyIdsAsync(Guid parentCompanyId, CancellationToken cancellationToken);
}
