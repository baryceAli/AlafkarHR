namespace Payroll.Salaries.Features.SalaryRuns.UndoSalaryRunsPeriod;

public record UndoSalaryRunsPeriodCommand(
    Guid CompanyId,
    int SalaryMonth,
    int SalaryYear) : ICommand<UndoSalaryRunsPeriodResult>;

public record UndoSalaryRunsPeriodResult(int DeletedCount, string Message);

public class UndoSalaryRunsPeriodCommandValidator : AbstractValidator<UndoSalaryRunsPeriodCommand>
{
    public UndoSalaryRunsPeriodCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company ID is required");

        RuleFor(x => x.SalaryMonth)
            .InclusiveBetween(1, 12).WithMessage("Salary month must be between 1 and 12");

        RuleFor(x => x.SalaryYear)
            .GreaterThan(2000).WithMessage("Salary year must be valid");
    }
}
