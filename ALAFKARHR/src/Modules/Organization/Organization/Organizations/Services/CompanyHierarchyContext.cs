namespace Organization.Organizations.Services;

public class CompanyHierarchyContext(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICompanyHierarchyContext, Shared.Contracts.Organization.ICompanyHierarchyReader, Shared.Contracts.Organization.IWebsiteCompanyReader
{
    public Task<List<Shared.Contracts.Organization.WebsiteCompanyInfo>> GetActiveCompaniesAsync(Guid parentCompanyId, CancellationToken cancellationToken)
        => dbContext.Companies.AsNoTracking().Where(x => x.IsActive && (x.Id == parentCompanyId || x.ParentCompanyId == parentCompanyId))
            .Select(x => new Shared.Contracts.Organization.WebsiteCompanyInfo(x.Id, x.Name, x.NameEng)).ToListAsync(cancellationToken);
    public async Task<Guid> GetCurrentParentCompanyIdAsync(CancellationToken cancellationToken)
    {
        var companyIdValue = httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(companyIdValue, out var companyId))
            throw new UnauthorizedAccessException("Current user is not linked to a company");

        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Current user's company was not found");

        if (company.ParentCompanyId.HasValue)
            throw new UnauthorizedAccessException("Child companies cannot manage child companies");

        return companyId;
    }

    public async Task<Guid> GetParentCompanyIdForCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new NotFoundException($"Company not found: {companyId}");

        return company.ParentCompanyId ?? company.Id;
    }

    public async Task<List<Guid>> GetCompanyHierarchyIdsAsync(Guid parentCompanyId, CancellationToken cancellationToken)
    {
        var childCompanyIds = await dbContext.Companies
            .AsNoTracking()
            .Where(x => x.ParentCompanyId == parentCompanyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        childCompanyIds.Insert(0, parentCompanyId);
        return childCompanyIds;
    }
}
