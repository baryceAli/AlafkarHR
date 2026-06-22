using Accounting.Contracts.Accounting.Features;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;

namespace Payroll.Salaries.Features.SalaryRuns.CommitSalaryRunsPeriod;

public class CommitSalaryRunsPeriodHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
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
            await PostPayrollAccountingAsync(salaryRun, request.CompanyId, cancellationToken);
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

    private async Task PostPayrollAccountingAsync(SalaryRun salaryRun, Guid companyId, CancellationToken cancellationToken)
    {
        var grossExpense = salaryRun.TotalSalary + salaryRun.TotalAllowances + salaryRun.InsuranceAmount;
        if (grossExpense <= 0)
            return;

        var lines = new List<JournalEntryLineDto>
        {
            new() { AccountRole = AccountRole.Expense, Debit = grossExpense, Description = $"Payroll {salaryRun.SalaryMonth}/{salaryRun.SalaryYear}" }
        };

        if (salaryRun.NetSalary > 0)
            lines.Add(new() { AccountRole = AccountRole.Payable, Credit = salaryRun.NetSalary, Description = "Net salary payable" });
        if (salaryRun.TaxAmount > 0)
            lines.Add(new() { AccountRole = AccountRole.Payable, Credit = salaryRun.TaxAmount, Description = "Payroll tax payable" });
        if (salaryRun.InsuranceAmount > 0)
            lines.Add(new() { AccountRole = AccountRole.Payable, Credit = salaryRun.InsuranceAmount, Description = "Payroll insurance payable" });
        if (salaryRun.TotalDeductions > 0)
            lines.Add(new() { AccountRole = AccountRole.Receivable, Credit = salaryRun.TotalDeductions, Description = "Payroll deductions and loan recovery" });

        await sender.Send(new CreateAndPostJournalEntryCommand(new CreateJournalEntryDto
        {
            CompanyId = companyId,
            EntryDate = DateTime.UtcNow,
            SourceModule = "Payroll",
            SourceDocumentId = salaryRun.Id,
            SourceDocumentNumber = $"{salaryRun.SalaryYear}-{salaryRun.SalaryMonth:00}-{salaryRun.EmployeeId:N}",
            Memo = $"Payroll run {salaryRun.SalaryMonth}/{salaryRun.SalaryYear}",
            Lines = lines
        }), cancellationToken);
    }
}
