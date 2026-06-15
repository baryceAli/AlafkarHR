namespace Organization.Organizations.Services;

public class CompanyHierarchyContext(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICompanyHierarchyContext
{
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
}
