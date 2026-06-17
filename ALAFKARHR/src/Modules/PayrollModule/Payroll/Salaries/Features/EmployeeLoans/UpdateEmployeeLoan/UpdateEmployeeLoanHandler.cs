namespace Payroll.Salaries.Features.EmployeeLoans.UpdateEmployeeLoan;

public class UpdateEmployeeLoanHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateEmployeeLoanCommand, UpdateEmployeeLoanResult>
{
    public async Task<UpdateEmployeeLoanResult> Handle(UpdateEmployeeLoanCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var dto = request.EmployeeLoan;
        var loan = await dbContext.EmployeeLoans
            .FirstOrDefaultAsync(x => x.Id == dto.Id && x.CompanyId == dto.CompanyId && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Loan or deduction with ID {dto.Id} not found");

        loan.Update(
            dto.EmployeeId,
            dto.Type,
            dto.Amount,
            dto.InstallmentAmount,
            dto.StartMonth,
            dto.StartYear,
            dto.ReferenceNo,
            dto.Notes,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateEmployeeLoanResult(loan.Id, loan.Status.ToString(), "Loan or deduction updated successfully");
    }
}
