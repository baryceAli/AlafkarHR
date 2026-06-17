namespace Payroll.Salaries.Features.EmployeeLoans.UpdateEmployeeLoan;

public record UpdateEmployeeLoanCommand(UpdateEmployeeLoanDto EmployeeLoan) : ICommand<UpdateEmployeeLoanResult>;

public record UpdateEmployeeLoanResult(Guid Id, string Status, string Message);

public class UpdateEmployeeLoanCommandValidator : AbstractValidator<UpdateEmployeeLoanCommand>
{
    public UpdateEmployeeLoanCommandValidator()
    {
        RuleFor(x => x.EmployeeLoan.Id)
            .NotEmpty().WithMessage("Loan is required");

        RuleFor(x => x.EmployeeLoan.CompanyId)
            .NotEmpty().WithMessage("Company is required");

        RuleFor(x => x.EmployeeLoan.EmployeeId)
            .NotEmpty().WithMessage("Employee is required");

        RuleFor(x => x.EmployeeLoan.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero");

        RuleFor(x => x.EmployeeLoan.InstallmentAmount)
            .GreaterThan(0).WithMessage("Installment amount must be greater than zero")
            .LessThanOrEqualTo(x => x.EmployeeLoan.Amount).WithMessage("Installment cannot exceed amount")
            .When(x => x.EmployeeLoan.Type == EmployeeLoanType.Loan);

        RuleFor(x => x.EmployeeLoan.StartMonth)
            .InclusiveBetween(1, 12).WithMessage("Start month must be between 1 and 12");

        RuleFor(x => x.EmployeeLoan.StartYear)
            .GreaterThan(0).WithMessage("Start year is required");
    }
}
