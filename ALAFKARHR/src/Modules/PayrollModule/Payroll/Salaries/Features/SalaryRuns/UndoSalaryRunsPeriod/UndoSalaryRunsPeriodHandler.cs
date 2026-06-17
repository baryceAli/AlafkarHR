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

        if (hasApprovedRuns)
        {
            var approvedSalaryRunIds = salaryRuns
                .Where(x => x.Status == SalaryRunStatus.Approved)
                .Select(x => x.Id)
                .ToList();

            await ReverseLoanRepaymentsAsync(approvedSalaryRunIds, user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, cancellationToken);
        }

        await dbContext.Set<SalaryRunItem>()
            .Where(x => salaryRunIds.Contains(x.SalaryRunId))
            .ExecuteDeleteAsync(cancellationToken);

        var deletedCount = await dbContext.Set<SalaryRun>()
            .Where(x => salaryRunIds.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return new UndoSalaryRunsPeriodResult(deletedCount, "Salary runs removed successfully");
    }

    private async Task ReverseLoanRepaymentsAsync(List<Guid> salaryRunIds, string userId, CancellationToken cancellationToken)
    {
        if (salaryRunIds.Count == 0)
        {
            return;
        }

        var loanIds = await dbContext.Set<EmployeeLoan>()
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (loanIds.Count == 0)
        {
            return;
        }

        var repayments = await dbContext.Set<SalaryRunItem>()
            .Where(x => salaryRunIds.Contains(x.SalaryRunId) && x.ComponentType == ComponentType.Deduction && loanIds.Contains(x.ItemId))
            .GroupBy(x => x.ItemId)
            .Select(x => new { LoanId = x.Key, Amount = x.Sum(i => i.Amount) })
            .ToListAsync(cancellationToken);

        if (repayments.Count == 0)
        {
            return;
        }

        var repaymentLoanIds = repayments.Select(x => x.LoanId).ToList();
        var loans = await dbContext.Set<EmployeeLoan>()
            .Where(x => repaymentLoanIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var repayment in repayments)
        {
            if (loans.TryGetValue(repayment.LoanId, out var loan))
            {
                loan.ReverseRepayment(repayment.Amount, userId);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
