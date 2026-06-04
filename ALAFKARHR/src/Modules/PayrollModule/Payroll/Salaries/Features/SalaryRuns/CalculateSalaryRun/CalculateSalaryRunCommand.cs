namespace Payroll.Salaries.Features.SalaryRuns.CalculateSalaryRun;

public record CalculateSalaryRunCommand(Guid SalaryRunId) : ICommand<CalculateSalaryRunResult>;

public record CalculateSalaryRunResult(
    Guid SalaryRunId,
    decimal TotalSalary,
    decimal TotalAllowances,
    decimal TotalDeductions,
    decimal NetSalary,
    string Message);

public class CalculateSalaryRunCommandValidator : AbstractValidator<CalculateSalaryRunCommand>
{
    public CalculateSalaryRunCommandValidator()
    {
        RuleFor(x => x.SalaryRunId)
            .NotEmpty().WithMessage("Salary Run ID is required");
    }
}
