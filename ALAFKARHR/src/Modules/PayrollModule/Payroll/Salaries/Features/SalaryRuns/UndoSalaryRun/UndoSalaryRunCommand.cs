namespace Payroll.Salaries.Features.SalaryRuns.UndoSalaryRun;

public record UndoSalaryRunCommand(Guid SalaryRunId) : ICommand<UndoSalaryRunResult>;

public record UndoSalaryRunResult(Guid SalaryRunId, string Status, string Message);

public class UndoSalaryRunCommandValidator : AbstractValidator<UndoSalaryRunCommand>
{
    public UndoSalaryRunCommandValidator()
    {
        RuleFor(x => x.SalaryRunId)
            .NotEmpty().WithMessage("Salary Run ID is required");
    }
}
