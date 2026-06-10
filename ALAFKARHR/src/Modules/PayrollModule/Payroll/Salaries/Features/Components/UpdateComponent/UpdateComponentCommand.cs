namespace Payroll.Salaries.Features.Components.UpdateComponent;

public record UpdateComponentCommand(Guid Id, ComponentDto Component) : ICommand<UpdateComponentResult>;

public record UpdateComponentResult(Guid Id, string Name);

public class UpdateComponentCommandValidator : AbstractValidator<UpdateComponentCommand>
{
    public UpdateComponentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Component ID is required");

        RuleFor(x => x.Component.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Component.NameEng)
            .NotEmpty().WithMessage("English name is required");

        RuleFor(x => x.Component.CompanyId)
            .NotEmpty().WithMessage("Company ID is required");
    }
}
