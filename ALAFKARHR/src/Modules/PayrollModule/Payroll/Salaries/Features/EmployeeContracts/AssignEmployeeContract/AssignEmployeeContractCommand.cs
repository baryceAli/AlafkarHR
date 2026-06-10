namespace Payroll.Salaries.Features.EmployeeContracts.AssignEmployeeContract;

public record AssignEmployeeContractCommand(EmployeeContractDto EmployeeContract) : ICommand<EmployeeContractDto>;

public class AssignEmployeeContractCommandValidator : AbstractValidator<AssignEmployeeContractCommand>
{
    public AssignEmployeeContractCommandValidator()
    {
        RuleFor(x => x.EmployeeContract.EmployeeId)
            .NotEmpty().WithMessage("Employee is required");

        RuleFor(x => x.EmployeeContract.ContractId)
            .NotEmpty().WithMessage("Contract is required");

        RuleFor(x => x.EmployeeContract.CompanyId)
            .NotEmpty().WithMessage("Company is required");
    }
}
