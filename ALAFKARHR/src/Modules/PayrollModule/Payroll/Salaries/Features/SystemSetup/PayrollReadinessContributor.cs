using EmployeeModule.Contracts.Employees.Features.GetCompanyEmployeeRosterProfiles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Setup;
using SharedWithUI.Permissions;

namespace Payroll.Salaries.Features.SystemSetup;

public sealed class PayrollReadinessContributor(PayrollDbContext dbContext, ISender sender) : ISetupReadinessContributor
{
    public string ModuleKey => "payroll";
    public IReadOnlyCollection<SetupStepDefinition> Steps =>
    [
        new("payroll-foundations", "workforce", "Payroll structures", "هياكل الرواتب", "Configure components, salary structures, periods, contracts, and assignments.", "اضبط المكونات وهياكل الرواتب والفترات والعقود والتعيينات.", "bi-cash-stack", "/HR/PayrollStructures", PermissionList.PayrollStructurePermissions.View, 140, false),
        new("saudi-payroll", "workforce", "Saudi payroll & WPS", "الرواتب السعودية وحماية الأجور", "Complete Saudi payroll and WPS information when applicable.", "أكمل معلومات الرواتب السعودية وحماية الأجور عند الحاجة.", "bi-bank", "/HR/SaudiPayroll", PermissionList.PayrollPayslipPermissions.Generate, 145, true, ["payroll-foundations"])
    ];

    public async Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var employeeIds = (await sender.Send(new GetCompanyEmployeeRosterProfilesQuery(companyId), cancellationToken)).Employees
            .Where(x => x.IsActive)
            .Select(x => x.EmployeeId)
            .ToHashSet();
        var hasComponent = await dbContext.Components.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted, cancellationToken);
        var hasStructure = await dbContext.SalaryStructures.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.Lines.Any(), cancellationToken);
        var hasPeriod = await dbContext.PayrollPeriods.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.StartDate <= today && x.EndDate >= today && !x.IsClosed, cancellationToken);
        var contractEmployeeIds = await dbContext.EmployeeContracts.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.EffectiveFrom <= today)
            .Select(x => x.EmployeeId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var structureEmployeeIds = await dbContext.SalaryStructureAssignments.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.EffectiveFrom <= today && (!x.EffectiveTo.HasValue || x.EffectiveTo >= today))
            .Select(x => x.EmployeeId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var uncovered = employeeIds.Count(id => !contractEmployeeIds.Contains(id) || !structureEmployeeIds.Contains(id));
        var complete = hasComponent && hasStructure && hasPeriod && employeeIds.Count > 0 && uncovered == 0;
        var hasSaudiInfo = await dbContext.SaudiPayrollInfos.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted, cancellationToken);

        return
        [
            new SetupReadinessResult(
                "payroll-foundations",
                complete,
                complete ? "Payroll components, structures, period, contracts, and assignments are ready." : $"Complete payroll foundations and contract/structure coverage for {uncovered} active employee(s).",
                complete ? "مكونات الرواتب والهياكل والفترة والعقود والتعيينات جاهزة." : $"أكمل أسس الرواتب وتغطية العقود والهياكل لعدد {uncovered} من الموظفين النشطين."),
            new SetupReadinessResult(
                "saudi-payroll",
                hasSaudiInfo,
                hasSaudiInfo ? "Saudi payroll information is available." : "Saudi payroll and WPS information is optional.",
                hasSaudiInfo ? "معلومات الرواتب السعودية متوفرة." : "معلومات الرواتب السعودية وحماية الأجور اختيارية.")
        ];
    }
}
