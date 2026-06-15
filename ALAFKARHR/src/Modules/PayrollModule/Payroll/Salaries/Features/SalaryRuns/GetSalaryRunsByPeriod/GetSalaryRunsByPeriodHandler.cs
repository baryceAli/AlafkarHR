namespace Payroll.Salaries.Features.SalaryRuns.GetSalaryRunsByPeriod;

public class GetSalaryRunsByPeriodHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetSalaryRunsByPeriodQuery, GetSalaryRunsByPeriodResult>
{
    public async Task<GetSalaryRunsByPeriodResult> Handle(GetSalaryRunsByPeriodQuery request, CancellationToken cancellationToken)
    {
        var salaryRuns = await dbContext.Set<SalaryRun>()
            .Join(
                dbContext.Set<Contract>(),
                salaryRun => salaryRun.ContractId,
                contract => contract.Id,
                (salaryRun, contract) => new { salaryRun, contract })
            .Where(x =>
                x.contract.CompanyId == request.CompanyId &&
                x.salaryRun.SalaryMonth == request.SalaryMonth &&
                x.salaryRun.SalaryYear == request.SalaryYear &&
                !x.salaryRun.IsDeleted &&
                !x.contract.IsDeleted)
            .OrderBy(x => x.salaryRun.EmployeeId)
            .Select(x => new SalaryRunPeriodDto(
                x.salaryRun.Id,
                x.salaryRun.EmployeeId,
                x.salaryRun.ContractId,
                x.salaryRun.SalaryMonth,
                x.salaryRun.SalaryYear,
                x.salaryRun.TotalSalary,
                x.salaryRun.TotalAllowances,
                x.salaryRun.TotalDeductions,
                x.salaryRun.TaxPercentage,
                x.salaryRun.TaxableAmount,
                x.salaryRun.TaxAmount,
                x.salaryRun.InsurancePercentage,
                x.salaryRun.InsuranceAmount,
                x.salaryRun.NetSalary,
                x.salaryRun.Status.ToString()))
            .ToListAsync(cancellationToken);

        return new GetSalaryRunsByPeriodResult(salaryRuns);
    }
}
