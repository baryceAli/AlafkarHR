namespace Payroll.Salaries.Features.SalaryRuns.GetSalaryRunById;

public class GetSalaryRunByIdHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetSalaryRunByIdQuery, GetSalaryRunByIdResult>
{
    public async Task<GetSalaryRunByIdResult> Handle(GetSalaryRunByIdQuery request, CancellationToken cancellationToken)
    {
        var salaryRun = await dbContext.Set<SalaryRun>()
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Salary run with ID {request.Id} not found");

        return new GetSalaryRunByIdResult(
            salaryRun.Id,
            salaryRun.EmployeeId,
            salaryRun.ContractId,
            salaryRun.SalaryMonth,
            salaryRun.SalaryYear,
            salaryRun.TotalSalary,
            salaryRun.TotalAllowances,
            salaryRun.TotalDeductions,
            salaryRun.TaxPercentage,
            salaryRun.TaxableAmount,
            salaryRun.TaxAmount,
            salaryRun.InsurancePercentage,
            salaryRun.InsuranceAmount,
            salaryRun.NetSalary,
            salaryRun.Status.ToString());
    }
}
