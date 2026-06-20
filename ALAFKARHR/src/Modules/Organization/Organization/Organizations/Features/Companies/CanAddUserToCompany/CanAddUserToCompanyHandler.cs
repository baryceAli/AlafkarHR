using Auth.Contracts.Features.CountUsersByCompanyIds;
using Organization.Contracts.Companies.Features.CanAddUserToCompany;

namespace Organization.Organizations.Features.Companies.CanAddUserToCompany;

public class CanAddUserToCompanyHandler(OrganizationDbContext dbContext, ICompanyHierarchyContext companyHierarchyContext, ISender sender)
    : IQueryHandler<CanAddUserToCompanyQuery, CanAddUserToCompanyResult>
{
    public async Task<CanAddUserToCompanyResult> Handle(CanAddUserToCompanyQuery request, CancellationToken cancellationToken)
    {
        var parentCompanyId = await companyHierarchyContext.GetParentCompanyIdForCompanyAsync(request.CompanyId, cancellationToken);
        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .Include(x => x.LicenseCategory)
            .FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken);

        if (license is null)
            return new CanAddUserToCompanyResult(true, null);

        if (!license.AllowsAccess(DateTime.UtcNow))
            return new CanAddUserToCompanyResult(false, "Parent company license is not active");

        var hierarchyIds = await companyHierarchyContext.GetCompanyHierarchyIdsAsync(parentCompanyId, cancellationToken);
        var usersCount = await sender.Send(new CountUsersByCompanyIdsQuery(hierarchyIds), cancellationToken);
        if (usersCount.Count >= license.EffectiveMaxUsers)
            return new CanAddUserToCompanyResult(false, "Parent company user license limit has been reached");

        return new CanAddUserToCompanyResult(true, null);
    }
}
