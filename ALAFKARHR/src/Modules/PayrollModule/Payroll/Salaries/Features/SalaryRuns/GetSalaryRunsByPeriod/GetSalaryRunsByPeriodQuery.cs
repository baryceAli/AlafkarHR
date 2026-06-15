namespace Payroll.Salaries.Features.SalaryRuns.GetSalaryRunsByPeriod;

public record GetSalaryRunsByPeriodQuery(
    Guid CompanyId,
    int SalaryMonth,
    int SalaryYear) : IQuery<GetSalaryRunsByPeriodResult>;

public record GetSalaryRunsByPeriodResult(List<SalaryRunPeriodDto> SalaryRunList);

public record SalaryRunPeriodDto(
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

public class GetSalaryRunsByPeriodQueryValidator : AbstractValidator<GetSalaryRunsByPeriodQuery>
{
    public GetSalaryRunsByPeriodQueryValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company ID is required");

        RuleFor(x => x.SalaryMonth)
            .InclusiveBetween(1, 12).WithMessage("Salary month must be between 1 and 12");

        RuleFor(x => x.SalaryYear)
            .GreaterThan(2000).WithMessage("Salary year must be valid");
    }
}
