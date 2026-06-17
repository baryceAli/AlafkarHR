namespace Payroll.Salaries.Features.EmployeeLoans.CancelEmployeeLoan;

public record CancelEmployeeLoanCommand(Guid Id) : ICommand<CancelEmployeeLoanResult>;

public record CancelEmployeeLoanResult(Guid Id, string Status, string Message);
