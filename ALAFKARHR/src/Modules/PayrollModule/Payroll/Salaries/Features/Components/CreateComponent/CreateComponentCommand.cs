namespace Payroll.Salaries.Features.Components.CreateComponent;

public record CreateComponentCommand(ComponentDto Component) : ICommand<CreateComponentResult>;

public record CreateComponentResult(Guid Id, string Name);

public class CreateComponentCommandValidator : AbstractValidator<CreateComponentCommand>
{
    public CreateComponentCommandValidator()
    {
        RuleFor(x => x.Component.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Component.NameEng)
            .NotEmpty().WithMessage("English name is required");

        RuleFor(x => x.Component.CompanyId)
            .NotEmpty().WithMessage("Company ID is required");
    }
}
