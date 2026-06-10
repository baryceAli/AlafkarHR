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
    decimal TaxPercentage,
    decimal TaxableAmount,
    decimal TaxAmount,
    decimal InsurancePercentage,
    decimal InsuranceAmount,
    decimal NetSalary,
    string Status);
