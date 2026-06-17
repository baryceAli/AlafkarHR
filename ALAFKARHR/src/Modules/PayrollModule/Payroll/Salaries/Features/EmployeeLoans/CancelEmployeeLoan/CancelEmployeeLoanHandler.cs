namespace Payroll.Salaries.Features.EmployeeLoans.CancelEmployeeLoan;

public class CancelEmployeeLoanHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CancelEmployeeLoanCommand, CancelEmployeeLoanResult>
{
    public async Task<CancelEmployeeLoanResult> Handle(CancelEmployeeLoanCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var loan = await dbContext.EmployeeLoans
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Loan or deduction with ID {request.Id} not found");

        loan.Cancel(userId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CancelEmployeeLoanResult(loan.Id, loan.Status.ToString(), "Loan or deduction cancelled successfully");
    }
}
