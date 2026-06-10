namespace Payroll.Salaries.Features.Components.DeleteComponent;

public record DeleteComponentCommand(Guid Id) : ICommand<DeleteComponentResult>;

public record DeleteComponentResult(bool IsSuccess);

public class DeleteComponentCommandValidator : AbstractValidator<DeleteComponentCommand>
{
    public DeleteComponentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Component ID is required");
    }
}
