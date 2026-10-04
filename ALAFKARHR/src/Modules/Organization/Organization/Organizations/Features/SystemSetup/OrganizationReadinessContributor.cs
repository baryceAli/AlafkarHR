using Microsoft.EntityFrameworkCore;
using Organization.Data;
using Shared.Setup;

namespace Organization.Organizations.Features.SystemSetup;

public sealed class OrganizationReadinessContributor(OrganizationDbContext dbContext)
    : ISetupReadinessContributor
{
    public string ModuleKey => "organization";
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("company-profile", "core", "Company profile & license", "ملف الشركة والترخيص", "Confirm company identity, currency, time zone, and licensed business lines.", "تحقق من هوية الشركة والعملة والمنطقة الزمنية وخطوط الأعمال المرخصة.", "bi-building", "/Organization/Company/List", PermissionList.CompanyPermissions.View, 10, false),
        new("organization-structure", "core", "Organization structure", "الهيكل التنظيمي", "Create the main branch, administrations, and departments.", "أنشئ الفرع الرئيسي والإدارات والأقسام.", "bi-diagram-3", "/Organization/Dashboard", PermissionList.BranchPermissions.View, 20, false)
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies.AsNoTracking()
            .Where(x => x.Id == companyId)
            .Select(x => new
            {
                x.IsActive,
                x.Name,
                x.NameEng,
                x.Code,
                x.CurrencyId,
                x.TimeZone
            })
            .FirstOrDefaultAsync(cancellationToken);

        var profileComplete = company is not null
            && company.IsActive
            && !string.IsNullOrWhiteSpace(company.Name)
            && !string.IsNullOrWhiteSpace(company.NameEng)
            && !string.IsNullOrWhiteSpace(company.Code)
            && company.CurrencyId != Guid.Empty
            && !string.IsNullOrWhiteSpace(company.TimeZone);

        var mainBranchExists = await dbContext.Branches.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.IsMainBranch, cancellationToken);
        var administrationExists = await dbContext.Administrations.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.IsActive, cancellationToken);
        var departmentExists = await dbContext.Departments.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.IsActive, cancellationToken);
        var structureComplete = mainBranchExists && administrationExists && departmentExists;

        return
        [
            new SetupReadinessResult(
                "company-profile",
                profileComplete,
                profileComplete ? "The active company profile is complete." : "Complete the company name, code, currency, and time zone.",
                profileComplete ? "ملف الشركة النشطة مكتمل." : "أكمل اسم الشركة والرمز والعملة والمنطقة الزمنية."),
            new SetupReadinessResult(
                "organization-structure",
                structureComplete,
                structureComplete ? "A main branch, administration, and department are available." : "Create the main branch and at least one administration and department.",
                structureComplete ? "يتوفر فرع رئيسي وإدارة وقسم." : "أنشئ الفرع الرئيسي وإدارة واحدة وقسماً واحداً على الأقل.")
        ];
    }
}
