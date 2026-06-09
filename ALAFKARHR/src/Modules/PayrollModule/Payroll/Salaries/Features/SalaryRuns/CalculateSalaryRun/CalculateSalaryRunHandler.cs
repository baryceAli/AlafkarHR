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
            .Include(x => x.SalaryRunItems)
            .FirstOrDefaultAsync(x => x.Id == request.SalaryRunId, cancellationToken)
            ?? throw new KeyNotFoundException($"Salary run with ID {request.SalaryRunId} not found");

        if (salaryRun.Status != SalaryRunStatus.Draft)
            throw new InvalidOperationException("Only draft salary runs can be calculated");

        var contract = await dbContext.Set<Contract>()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == salaryRun.ContractId, cancellationToken)
            ?? throw new KeyNotFoundException($"Contract with ID {salaryRun.ContractId} not found");

        var componentIds = contract.Items.Select(x => x.ComponentId).ToList();
        var components = await dbContext.Set<Component>()
            .Where(x => componentIds.Contains(x.Id) && x.IsActive)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var allowances = await dbContext.Set<EmployeeAllowance>()
            .Where(x => x.EmployeeId == salaryRun.EmployeeId && x.IsActive)
            .ToListAsync(cancellationToken);

        var deductions = await dbContext.Set<EmployeeDeduction>()
            .Where(x => x.EmployeeId == salaryRun.EmployeeId && x.IsActive)
            .ToListAsync(cancellationToken);

        decimal totalAllowances = 0;
        decimal totalDeductions = 0;

        salaryRun.ClearItems();
        salaryRun.AddItem(salaryRun.ContractId, ComponentType.Basic, salaryRun.TotalSalary);

        foreach (var item in contract.Items)
        {
            if (!components.TryGetValue(item.ComponentId, out var component) || component.ComponentType == ComponentType.Basic)
                continue;

            if (component.ComponentType == ComponentType.Allowance)
            {
                totalAllowances += item.Amount;
            }
            else if (component.ComponentType == ComponentType.Deduction)
            {
                totalDeductions += item.Amount;
            }

            salaryRun.AddItem(item.Id, component.ComponentType, item.Amount);
        }

        foreach (var allowance in allowances)
        {
            var allowanceAmount = allowance.CalculationType == CalculationType.Fixed
                ? allowance.Value
                : (salaryRun.TotalSalary * allowance.Value) / 100;

            totalAllowances += allowanceAmount;
            salaryRun.AddItem(allowance.Id, ComponentType.Allowance, allowanceAmount);
        }

        foreach (var deduction in deductions)
        {
            decimal deductionAmount;

            if (deduction.CalculationType == CalculationType.Fixed)
            {
                deductionAmount = deduction.Value;
            }
            else if (deduction.CalculationType == CalculationType.Percentage)
            {
                deductionAmount = (salaryRun.TotalSalary * deduction.Value) / 100;
            }
            else // Installment
            {
                deductionAmount = deduction.InstallmentAmount ?? 0;
            }

            totalDeductions += deductionAmount;
            salaryRun.AddItem(deduction.Id, ComponentType.Deduction, deductionAmount);
        }

        salaryRun.TotalAllowances = totalAllowances;
        salaryRun.TotalDeductions = totalDeductions;
        salaryRun.Status = SalaryRunStatus.Calculated;

        dbContext.Set<SalaryRun>().Update(salaryRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CalculateSalaryRunResult(
            salaryRun.Id,
            salaryRun.TotalSalary,
            totalAllowances,
            totalDeductions,
            salaryRun.NetSalary,
            "Salary run calculated successfully");
    }
}
