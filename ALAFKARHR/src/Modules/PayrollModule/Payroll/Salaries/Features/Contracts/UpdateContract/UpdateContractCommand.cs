using Payroll.Salaries.Features.Contracts.CreateContract;

namespace Payroll.Salaries.Features.Contracts.UpdateContract;

public record UpdateContractCommand(
    Guid Id,
    string Name,
    string NameEng,
    string? Description,
    List<ContractItemDto> ContractItems) : ICommand<UpdateContractResult>;

public record UpdateContractResult(Guid Id, string Name);

public class UpdateContractCommandValidator : AbstractValidator<UpdateContractCommand>
{
    public UpdateContractCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Contract ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.NameEng)
            .NotEmpty().WithMessage("English name is required");

        RuleFor(x => x.ContractItems)
            .NotEmpty().WithMessage("At least one contract item is required");
    }
}
