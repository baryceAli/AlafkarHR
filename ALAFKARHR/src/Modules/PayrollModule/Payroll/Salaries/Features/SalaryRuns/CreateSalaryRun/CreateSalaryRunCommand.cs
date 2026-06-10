namespace Payroll.Salaries.Features.SalaryRuns.CreateSalaryRun;

public record CreateSalaryRunCommand(
    Guid EmployeeId,
    Guid ContractId,
    int SalaryMonth,
    int SalaryYear,
    decimal BaseSalary) : ICommand<CreateSalaryRunResult>;

public record CreateSalaryRunResult(Guid Id, string Message);

public class CreateSalaryRunCommandValidator : AbstractValidator<CreateSalaryRunCommand>
{
    public CreateSalaryRunCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required");

        RuleFor(x => x.ContractId)
            .NotEmpty().WithMessage("Contract ID is required");

        RuleFor(x => x.SalaryMonth)
            .InclusiveBetween(1, 12).WithMessage("Salary month must be between 1 and 12");

        RuleFor(x => x.SalaryYear)
            .GreaterThan(2000).WithMessage("Salary year must be valid");

        RuleFor(x => x.BaseSalary)
            .GreaterThanOrEqualTo(0).WithMessage("Base salary cannot be negative");
    }
}
