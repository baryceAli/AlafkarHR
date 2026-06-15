namespace Payroll.Salaries.Features.SalaryRuns.CommitSalaryRunsPeriod;

public record CommitSalaryRunsPeriodCommand(
    Guid CompanyId,
    int SalaryMonth,
    int SalaryYear) : ICommand<CommitSalaryRunsPeriodResult>;

public record CommitSalaryRunsPeriodResult(int CommittedCount, string Status, string Message);

public class CommitSalaryRunsPeriodCommandValidator : AbstractValidator<CommitSalaryRunsPeriodCommand>
{
    public CommitSalaryRunsPeriodCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company ID is required");

        RuleFor(x => x.SalaryMonth)
            .InclusiveBetween(1, 12).WithMessage("Salary month must be between 1 and 12");

        RuleFor(x => x.SalaryYear)
            .GreaterThan(2000).WithMessage("Salary year must be valid");
    }
}
