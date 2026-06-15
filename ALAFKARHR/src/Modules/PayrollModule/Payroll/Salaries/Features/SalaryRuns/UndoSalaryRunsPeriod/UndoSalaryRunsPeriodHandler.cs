namespace Payroll.Salaries.Features.SalaryRuns.UndoSalaryRunsPeriod;

public class UndoSalaryRunsPeriodHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UndoSalaryRunsPeriodCommand, UndoSalaryRunsPeriodResult>
{
    public async Task<UndoSalaryRunsPeriodResult> Handle(UndoSalaryRunsPeriodCommand request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var hasEditPermission = user.Claims.Any(c => c.Value == PermissionList.SalaryRunPermissions.Edit);
        var hasAdminOverridePermission = user.Claims.Any(c => c.Value == PermissionList.SalaryRunPermissions.AdminOverride);

        var salaryRuns = await dbContext.Set<SalaryRun>()
            .Join(
                dbContext.Set<Contract>(),
                salaryRun => salaryRun.ContractId,
                contract => contract.Id,
                (salaryRun, contract) => new { salaryRun, contract })
            .Where(x =>
                x.contract.CompanyId == request.CompanyId &&
                x.salaryRun.SalaryMonth == request.SalaryMonth &&
                x.salaryRun.SalaryYear == request.SalaryYear &&
                !x.salaryRun.IsDeleted &&
                !x.contract.IsDeleted)
            .Select(x => x.salaryRun)
            .ToListAsync(cancellationToken);

        if (salaryRuns.Count == 0)
        {
            return new UndoSalaryRunsPeriodResult(0, "No salary runs found for this period");
        }

        var hasApprovedRuns = salaryRuns.Any(x => x.Status == SalaryRunStatus.Approved);
        if (hasApprovedRuns && !hasAdminOverridePermission)
        {
            throw new UnauthorizedAccessException("Admin override permission is required to undo committed salary runs");
        }

        if (!hasApprovedRuns && !hasEditPermission)
        {
            throw new UnauthorizedAccessException("Edit permission is required to undo generated salary runs");
        }

        if (!hasApprovedRuns && salaryRuns.Any(x => x.Status != SalaryRunStatus.Calculated))
        {
            throw new InvalidOperationException("Only calculated salary runs can be undone");
        }

        var salaryRunIds = salaryRuns.Select(x => x.Id).ToList();

        await dbContext.Set<SalaryRunItem>()
            .Where(x => salaryRunIds.Contains(x.SalaryRunId))
            .ExecuteDeleteAsync(cancellationToken);

        var deletedCount = await dbContext.Set<SalaryRun>()
            .Where(x => salaryRunIds.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return new UndoSalaryRunsPeriodResult(deletedCount, "Salary runs removed successfully");
    }
}
