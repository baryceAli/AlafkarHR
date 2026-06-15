
namespace Payroll.Salaries.Features.SalaryRuns.CreateSalaryRun;

public class CreateSalaryRunHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateSalaryRunCommand, CreateSalaryRunResult>
{
    public async Task<CreateSalaryRunResult> Handle(CreateSalaryRunCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        // Check if salary run already exists for this employee in this month
        var existingRun = await dbContext.Set<SalaryRun>()
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == request.EmployeeId &&
                x.SalaryMonth == request.SalaryMonth &&
                x.SalaryYear == request.SalaryYear &&
                !x.IsDeleted,
                cancellationToken);

        if (existingRun != null)
            return new CreateSalaryRunResult(existingRun.Id, "Salary run already exists");

        var salaryRun = new SalaryRun
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            ContractId = request.ContractId,
            SalaryMonth = request.SalaryMonth,
            SalaryYear = request.SalaryYear,
            TotalSalary = request.BaseSalary,
            TotalAllowances = 0,
            TotalDeductions = 0,
            TaxPercentage = 0,
            TaxableAmount = 0,
            TaxAmount = 0,
            InsurancePercentage = 0,
            InsuranceAmount = 0,
            Status = SalaryRunStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        salaryRun.AddItem(salaryRun.ContractId, ComponentType.Basic, salaryRun.TotalSalary);

        await dbContext.Set<SalaryRun>().AddAsync(salaryRun, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateSalaryRunResult(salaryRun.Id, "Salary run created successfully");
    }
}
