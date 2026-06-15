namespace Organization.Organizations.Services;

public interface ICompanyHierarchyContext
{
    Task<Guid> GetCurrentParentCompanyIdAsync(CancellationToken cancellationToken);
}
