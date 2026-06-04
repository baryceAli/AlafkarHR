
namespace Payroll.Salaries.Features.Contracts.CreateContract;

public record CreateContractCommand(
    string Name,
    string NameEng,
    string? Description,
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

        RuleFor(x => x.ContractItems)
            .NotEmpty().WithMessage("At least one contract item is required");
    }
}

public record ContractItemDto(
    Guid ComponentId,
    decimal Value);
