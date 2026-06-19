using Organization.Contracts.Companies.Features.GetCompanyAccessStatus;

namespace Organization.Organizations.Features.Companies.GetCompanyAccessStatus;

public class GetCompanyAccessStatusHandler(OrganizationDbContext dbContext, ICompanyHierarchyContext companyHierarchyContext)
    : IQueryHandler<GetCompanyAccessStatusQuery, GetCompanyAccessStatusResult>
{
    public async Task<GetCompanyAccessStatusResult> Handle(GetCompanyAccessStatusQuery request, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CompanyId, cancellationToken);

        if (company is null || !company.IsActive)
            return new GetCompanyAccessStatusResult(false);

        var parentCompanyId = await companyHierarchyContext.GetParentCompanyIdForCompanyAsync(request.CompanyId, cancellationToken);
        var parentCompany = parentCompanyId == company.Id
            ? company
            : await dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == parentCompanyId, cancellationToken);

        if (parentCompany is null || !parentCompany.IsActive)
            return new GetCompanyAccessStatusResult(false);

        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken);

        var canLogin = license is null || license.AllowsAccess(DateTime.UtcNow);

        return new GetCompanyAccessStatusResult(canLogin);
    }
}
