namespace Payroll.Salaries.Features.SalaryRuns.ApproveSalaryRun;

public class ApproveSalaryRunHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ApproveSalaryRunCommand, ApproveSalaryRunResult>
{
    public async Task<ApproveSalaryRunResult> Handle(ApproveSalaryRunCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var salaryRun = await dbContext.Set<SalaryRun>()
            .FirstOrDefaultAsync(x => x.Id == request.SalaryRunId, cancellationToken)
            ?? throw new KeyNotFoundException($"Salary run with ID {request.SalaryRunId} not found");

        if (salaryRun.Status != SalaryRunStatus.Calculated)
            throw new InvalidOperationException("Only calculated salary runs can be approved");

        await PostLoanRepaymentsAsync([salaryRun.Id], userId, cancellationToken);

        salaryRun.Status = SalaryRunStatus.Approved;
        salaryRun.ModifiedAt = DateTime.UtcNow;
        salaryRun.ModifiedBy = userId;

        dbContext.Set<SalaryRun>().Update(salaryRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApproveSalaryRunResult(
            salaryRun.Id,
            salaryRun.Status.ToString(),
            "Salary run approved successfully");
    }

    private async Task PostLoanRepaymentsAsync(List<Guid> salaryRunIds, string userId, CancellationToken cancellationToken)
    {
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
