namespace Payroll.Salaries.Features.SalaryRuns.UndoSalaryRun;

public class UndoSalaryRunHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UndoSalaryRunCommand, UndoSalaryRunResult>
{
    public async Task<UndoSalaryRunResult> Handle(UndoSalaryRunCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var salaryRun = await dbContext.Set<SalaryRun>()
            .Include(x => x.SalaryRunItems)
            .FirstOrDefaultAsync(x => x.Id == request.SalaryRunId && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Salary run with ID {request.SalaryRunId} not found");

        salaryRun.UndoGeneration(userId);
        dbContext.Set<SalaryRun>().Update(salaryRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UndoSalaryRunResult(
            salaryRun.Id,
            salaryRun.Status.ToString(),
            "Salary generation undone successfully");
    }
}
