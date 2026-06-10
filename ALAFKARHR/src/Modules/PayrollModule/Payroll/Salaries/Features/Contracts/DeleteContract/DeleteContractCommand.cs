namespace Payroll.Salaries.Features.Contracts.DeleteContract;

public record DeleteContractCommand(Guid Id) : ICommand<DeleteContractResult>;

public record DeleteContractResult(bool IsSuccess);

public class DeleteContractCommandValidator : AbstractValidator<DeleteContractCommand>
{
    public DeleteContractCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Contract ID is required");
    }
}
