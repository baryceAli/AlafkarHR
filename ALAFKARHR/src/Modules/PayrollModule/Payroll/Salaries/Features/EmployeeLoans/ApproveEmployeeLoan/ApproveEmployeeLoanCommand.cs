namespace Payroll.Salaries.Features.EmployeeLoans.ApproveEmployeeLoan;

public record ApproveEmployeeLoanCommand(Guid Id) : ICommand<ApproveEmployeeLoanResult>;

public record ApproveEmployeeLoanResult(Guid Id, string Status, string Message);
