namespace Payroll.Salaries.Features.SalaryRuns.CalculateSalaryRun;

public class CalculateSalaryRunHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CalculateSalaryRunCommand, CalculateSalaryRunResult>
{
    public async Task<CalculateSalaryRunResult> Handle(CalculateSalaryRunCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var salaryRun = await dbContext.Set<SalaryRun>()
            .FirstOrDefaultAsync(x => x.Id == request.SalaryRunId, cancellationToken)
            ?? throw new KeyNotFoundException($"Salary run with ID {request.SalaryRunId} not found");

        if (salaryRun.Status != SalaryRunStatus.Draft)
            throw new InvalidOperationException("Only draft salary runs can be calculated");

        // Get employee allowances and deductions
        var allowances = await dbContext.Set<EmployeeAllowance>()
            .Where(x => x.EmployeeId == salaryRun.EmployeeId && x.IsActive)
            .ToListAsync(cancellationToken);

        var deductions = await dbContext.Set<EmployeeDeduction>()
            .Where(x => x.EmployeeId == salaryRun.EmployeeId && x.IsActive)
            .ToListAsync(cancellationToken);

        decimal totalAllowances = 0;
        decimal totalDeductions = 0;

        // Calculate allowances
        foreach (var allowance in allowances)
        {
            var allowanceAmount = allowance.CalculationType == CalculationType.Fixed
                ? allowance.Value
                : (salaryRun.totalSalary * allowance.Value) / 100;

            totalAllowances += allowanceAmount;
        }

        // Calculate deductions
        foreach (var deduction in deductions)
        {
            decimal deductionAmount;

            if (deduction.CalculationType == CalculationType.Fixed)
            {
                deductionAmount = deduction.Value;
            }
            else if (deduction.CalculationType == CalculationType.Percentage)
            {
                deductionAmount = (salaryRun.totalSalary * deduction.Value) / 100;
            }
            else // Installment
            {
                deductionAmount = deduction.InstallmentAmount ?? 0;
            }

            totalDeductions += deductionAmount;
        }

        salaryRun.TotalAllowances = totalAllowances;
        salaryRun.totalDeductions = totalDeductions;
        salaryRun.Status = SalaryRunStatus.Calculated;

        dbContext.Set<SalaryRun>().Update(salaryRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CalculateSalaryRunResult(
            salaryRun.Id,
            salaryRun.totalSalary,
            totalAllowances,
            totalDeductions,
            salaryRun.NetSalary,
            "Salary run calculated successfully");
    }
}
