namespace Payroll.Salaries.Features.SalaryRuns.ApproveSalaryRun;

public record ApproveSalaryRunCommand(Guid SalaryRunId) : ICommand<ApproveSalaryRunResult>;

public record ApproveSalaryRunResult(Guid SalaryRunId, string Status, string Message);

public class ApproveSalaryRunCommandValidator : AbstractValidator<ApproveSalaryRunCommand>
{
    public ApproveSalaryRunCommandValidator()
    {
        RuleFor(x => x.SalaryRunId)
            .NotEmpty().WithMessage("Salary Run ID is required");
    }
}
