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

        var componentIds = contract.Items
            .Select(x => x.ComponentId)
            .Concat(await dbContext.Set<EmployeeAllowance>()
                .Where(x => x.EmployeeId == salaryRun.EmployeeId && x.IsActive)
                .Select(x => x.ComponentId)
                .ToListAsync(cancellationToken))
            .Distinct()
            .ToList();

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
        decimal taxableAmount = 0;

        salaryRun.ClearItems();
        salaryRun.AddItem(salaryRun.ContractId, ComponentType.Basic, salaryRun.TotalSalary);

        if (contract.Items.Any(x =>
                components.TryGetValue(x.ComponentId, out var component) &&
                component.ComponentType == ComponentType.Basic &&
                component.IsTaxable))
        {
            taxableAmount += salaryRun.TotalSalary;
        }

        foreach (var item in contract.Items)
        {
            if (!components.TryGetValue(item.ComponentId, out var component) || component.ComponentType == ComponentType.Basic)
                continue;

            if (component.ComponentType == ComponentType.Allowance)
            {
                totalAllowances += item.Amount;
                if (component.IsTaxable)
                {
                    taxableAmount += item.Amount;
                }
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
            if (components.TryGetValue(allowance.ComponentId, out var allowanceComponent) && allowanceComponent.IsTaxable)
            {
                taxableAmount += allowanceAmount;
            }
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

        var insuranceAmount = (salaryRun.TotalSalary + totalAllowances) * contract.InsurancePercentage / 100m;

        salaryRun.TotalAllowances = totalAllowances;
        salaryRun.TotalDeductions = totalDeductions + insuranceAmount;
        salaryRun.TaxPercentage = contract.TaxPercentage;
        salaryRun.TaxableAmount = taxableAmount;
        salaryRun.TaxAmount = taxableAmount * contract.TaxPercentage / 100m;
        salaryRun.InsurancePercentage = contract.InsurancePercentage;
        salaryRun.InsuranceAmount = insuranceAmount;
        salaryRun.Status = SalaryRunStatus.Calculated;

        dbContext.Set<SalaryRun>().Update(salaryRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CalculateSalaryRunResult(
            salaryRun.Id,
            salaryRun.EmployeeId,
            salaryRun.ContractId,
            salaryRun.SalaryMonth,
            salaryRun.SalaryYear,
            salaryRun.TotalSalary,
            totalAllowances,
            salaryRun.TotalDeductions,
            salaryRun.TaxPercentage,
            salaryRun.TaxableAmount,
            salaryRun.TaxAmount,
            salaryRun.InsurancePercentage,
            salaryRun.InsuranceAmount,
            salaryRun.NetSalary,
            salaryRun.Status.ToString(),
            "Salary run calculated successfully");
    }
}
