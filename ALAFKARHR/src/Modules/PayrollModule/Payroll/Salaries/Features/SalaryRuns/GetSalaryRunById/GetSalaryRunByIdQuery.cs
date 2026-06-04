namespace Payroll.Salaries.Features.SalaryRuns.GetSalaryRunById;

public record GetSalaryRunByIdQuery(Guid Id) : IQuery<GetSalaryRunByIdResult>;

public record GetSalaryRunByIdResult(
    Guid Id,
    Guid EmployeeId,
    Guid ContractId,
    int SalaryMonth,
    int SalaryYear,
    decimal TotalSalary,
    decimal TotalAllowances,
    decimal TotalDeductions,
    decimal NetSalary,
    string Status);
