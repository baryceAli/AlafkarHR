namespace Payroll.Salaries.Features.SalaryRuns.ApproveSalaryRun;

public class ApproveSalaryRunHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ApproveSalaryRunCommand, ApproveSalaryRunResult>
{
    public async Task<ApproveSalaryRunResult> Handle(ApproveSalaryRunCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var salaryRun = await dbContext.Set<SalaryRun>()
            .FirstOrDefaultAsync(x => x.Id == request.SalaryRunId, cancellationToken)
            ?? throw new KeyNotFoundException($"Salary run with ID {request.SalaryRunId} not found");

        if (salaryRun.Status != SalaryRunStatus.Calculated)
            throw new InvalidOperationException("Only calculated salary runs can be approved");

        salaryRun.Status = SalaryRunStatus.Approved;

        dbContext.Set<SalaryRun>().Update(salaryRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApproveSalaryRunResult(
            salaryRun.Id,
            salaryRun.Status.ToString(),
            "Salary run approved successfully");
    }
}
