using SharedWithUI.Organization.Enums;

namespace Organization.Organizations.Features.Companies.GetCurrentCompanyLicense;

public record GetCurrentCompanyLicenseQuery : IQuery<GetCurrentCompanyLicenseResult>;
public record GetCurrentCompanyLicenseResult(CompanyLicenseSummaryDto License);

public class GetCurrentCompanyLicenseHandler(
    OrganizationDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    ICompanyHierarchyContext companyHierarchyContext)
    : IQueryHandler<GetCurrentCompanyLicenseQuery, GetCurrentCompanyLicenseResult>
{
    public async Task<GetCurrentCompanyLicenseResult> Handle(GetCurrentCompanyLicenseQuery request, CancellationToken cancellationToken)
    {
        var companyIdValue = httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(companyIdValue, out var companyId))
            throw new UnauthorizedAccessException("Current user is not linked to a company");

        var parentCompanyId = await companyHierarchyContext.GetParentCompanyIdForCompanyAsync(companyId, cancellationToken);
        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .Include(x => x.LicenseCategory)
            .FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken);
        var businessLinesByLicense = license is null
            ? []
            : await ParentCompanies.ParentCompanyQueryHandler.GetBusinessLinesByLicenseAsync(dbContext, [license.Id], cancellationToken);

        return new GetCurrentCompanyLicenseResult(ToSummary(parentCompanyId, license, businessLinesByLicense));
    }

    private static CompanyLicenseSummaryDto ToSummary(
        Guid companyId,
        CompanyLicense? license,
        IReadOnlyDictionary<Guid, List<LicensedBusinessLineDto>>? businessLinesByLicense = null)
    {
        if (license is null)
        {
            return new CompanyLicenseSummaryDto
            {
                CompanyId = companyId,
                Status = CompanyLicenseStatus.Active,
                EffectiveStatus = CompanyLicenseStatus.Active,
                PlanKey = "legacy",
                PlanName = "Legacy",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddYears(1),
                MaxUsers = 0,
                MaxChildCompanies = 0,
                MaxBranches = 0,
                MonthlyPrice = 0,
                YearlyPrice = 0,
                CurrencyId = null,
                CurrencyCode = null,
                IsExpired = false
            };
        }

        var isExpired = license.EndDate.Date < DateTime.UtcNow.Date;
        return new CompanyLicenseSummaryDto
        {
            CompanyId = license.CompanyId,
            LicenseCategoryId = license.LicenseCategoryId,
            Status = license.Status,
            EffectiveStatus = isExpired ? CompanyLicenseStatus.Expired : license.Status,
            PlanKey = license.EffectivePlanKey,
            PlanName = license.EffectivePlanName,
            StartDate = license.StartDate,
            EndDate = license.EndDate,
            MaxUsers = license.EffectiveMaxUsers,
            MaxChildCompanies = license.EffectiveMaxChildCompanies,
            MaxBranches = license.EffectiveMaxBranches,
            MonthlyPrice = license.EffectiveMonthlyPrice,
            YearlyPrice = license.EffectiveYearlyPrice,
            CurrencyId = license.EffectiveCurrencyId,
            CurrencyCode = license.EffectiveCurrencyCode,
            IsExpired = isExpired,
            BusinessLines = businessLinesByLicense is not null && businessLinesByLicense.TryGetValue(license.Id, out var businessLines)
                ? businessLines
                : []
        };
    }
}
