namespace Payroll.Salaries.Features.SalaryRuns.CalculateSalaryRun;

public record CalculateSalaryRunCommand(Guid SalaryRunId) : ICommand<CalculateSalaryRunResult>;

public record CalculateSalaryRunResult(
    Guid Id,
    Guid EmployeeId,
    Guid ContractId,
    int SalaryMonth,
    int SalaryYear,
    decimal TotalSalary,
    decimal TotalAllowances,
    decimal TotalDeductions,
    decimal TaxPercentage,
    decimal TaxableAmount,
    decimal TaxAmount,
    decimal InsurancePercentage,
    decimal InsuranceAmount,
    decimal NetSalary,
    string Status,
    string Message);

public class CalculateSalaryRunCommandValidator : AbstractValidator<CalculateSalaryRunCommand>
{
    public CalculateSalaryRunCommandValidator()
    {
        RuleFor(x => x.SalaryRunId)
            .NotEmpty().WithMessage("Salary Run ID is required");
    }
}
