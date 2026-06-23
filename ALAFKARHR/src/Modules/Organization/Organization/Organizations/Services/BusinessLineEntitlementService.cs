using Shared.Contracts.Organization;

namespace Organization.Organizations.Services;

public class BusinessLineEntitlementService(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IBusinessLineEntitlementService
{
    public async Task<bool> IsBusinessLineLicensedAsync(string businessLineKey, CancellationToken cancellationToken)
    {
        var keys = await GetCurrentLicensedBusinessLineKeysAsync(cancellationToken);
        return keys.Contains(BusinessLine.NormalizeKey(businessLineKey));
    }

    public async Task<IReadOnlySet<string>> GetCurrentLicensedBusinessLineKeysAsync(CancellationToken cancellationToken)
    {
        var parentCompanyId = await GetCurrentParentCompanyIdAsync(cancellationToken);
        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken);

        if (license is null || !license.AllowsAccess(DateTime.UtcNow))
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var keys = await dbContext.CompanyLicenseBusinessLines
            .AsNoTracking()
            .Where(x => x.CompanyLicenseId == license.Id && x.BusinessLine.IsActive)
            .Select(x => x.BusinessLine.Key)
            .ToListAsync(cancellationToken);

        return keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<BusinessLineLicenseEntitlement> GetEntitlementAsync(
        string businessLineKey,
        Guid companyId,
        int externalUsedActivations,
        CancellationToken cancellationToken)
    {
        var parentCompanyId = await GetParentCompanyIdForCompanyAsync(companyId, cancellationToken);
        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken)
            ?? throw new InvalidOperationException("Parent company does not have a license");

        if (!license.AllowsAccess(DateTime.UtcNow))
            throw new InvalidOperationException("Parent company license is not active");

        var normalizedKey = BusinessLine.NormalizeKey(businessLineKey);
        var line = await dbContext.CompanyLicenseBusinessLines
            .AsNoTracking()
            .Where(x => x.CompanyLicenseId == license.Id && x.BusinessLine.Key == normalizedKey && x.BusinessLine.IsActive)
            .Select(x => new
            {
                x.BusinessLineId,
                x.BusinessLine.Key,
                x.ActivationLimit
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"Business line is not licensed: {businessLineKey}");

        return new BusinessLineLicenseEntitlement(
            parentCompanyId,
            license.Id,
            line.BusinessLineId,
            line.Key,
            line.ActivationLimit,
            externalUsedActivations);
    }

    public async Task EnsureActivationAvailableAsync(
        string businessLineKey,
        Guid companyId,
        int externalUsedActivations,
        CancellationToken cancellationToken)
    {
        var entitlement = await GetEntitlementAsync(businessLineKey, companyId, externalUsedActivations, cancellationToken);
        if (entitlement.UsedActivations >= entitlement.ActivationLimit)
            throw new InvalidOperationException($"Activation limit reached for business line: {businessLineKey}");
    }

    private async Task<Guid> GetCurrentParentCompanyIdAsync(CancellationToken cancellationToken)
    {
        var companyIdValue = httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(companyIdValue, out var companyId))
            throw new UnauthorizedAccessException("Current user is not linked to a company");

        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Current user's company was not found");

        return company.ParentCompanyId ?? company.Id;
    }

    private async Task<Guid> GetParentCompanyIdForCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new NotFoundException($"Company not found: {companyId}");

        return company.ParentCompanyId ?? company.Id;
    }
}
