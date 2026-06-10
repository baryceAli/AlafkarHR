
namespace Payroll.Salaries.Features.Contracts.CreateContract;

public record CreateContractCommand(
    string Name,
    string NameEng,
    string? Description,
    decimal TaxPercentage,
    decimal InsurancePercentage,
    Guid CompanyId,
    List<ContractItemDto> ContractItems) : ICommand<CreateContractResult>;

public record CreateContractResult(Guid Id, string Name);

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.NameEng)
            .NotEmpty().WithMessage("English name is required");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company ID is required");

        RuleFor(x => x.TaxPercentage)
            .GreaterThanOrEqualTo(0).WithMessage("Tax percentage cannot be negative");

        RuleFor(x => x.InsurancePercentage)
            .GreaterThanOrEqualTo(0).WithMessage("Insurance percentage cannot be negative");

        RuleFor(x => x.ContractItems)
            .NotEmpty().WithMessage("At least one contract item is required");

        RuleFor(x => x.ContractItems)
            .Must(items => items is not null && items.Select(i => i.ComponentId).Distinct().Count() == items.Count)
            .WithMessage("Contract components cannot be duplicated");
    }
}
