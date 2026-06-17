namespace Payroll.Salaries.Features.SalaryRuns.CommitSalaryRunsPeriod;

public class CommitSalaryRunsPeriodHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CommitSalaryRunsPeriodCommand, CommitSalaryRunsPeriodResult>
{
    public async Task<CommitSalaryRunsPeriodResult> Handle(CommitSalaryRunsPeriodCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

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
            return new CommitSalaryRunsPeriodResult(0, SalaryRunStatus.Approved.ToString(), "No salary runs found for this period");
        }

        if (salaryRuns.Any(x => x.Status != SalaryRunStatus.Calculated && x.Status != SalaryRunStatus.Approved))
        {
            throw new InvalidOperationException("Only calculated salary runs can be committed");
        }

        var calculatedRuns = salaryRuns
            .Where(x => x.Status == SalaryRunStatus.Calculated)
            .ToList();

        await PostLoanRepaymentsAsync(calculatedRuns.Select(x => x.Id).ToList(), userId, cancellationToken);

        foreach (var salaryRun in calculatedRuns)
        {
            salaryRun.Status = SalaryRunStatus.Approved;
            salaryRun.ModifiedAt = DateTime.UtcNow;
            salaryRun.ModifiedBy = userId;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CommitSalaryRunsPeriodResult(
            calculatedRuns.Count,
            SalaryRunStatus.Approved.ToString(),
            "Salary runs committed successfully");
    }

    private async Task PostLoanRepaymentsAsync(List<Guid> salaryRunIds, string userId, CancellationToken cancellationToken)
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
                loan.PostRepayment(repayment.Amount, userId);
            }
        }
    }
}
