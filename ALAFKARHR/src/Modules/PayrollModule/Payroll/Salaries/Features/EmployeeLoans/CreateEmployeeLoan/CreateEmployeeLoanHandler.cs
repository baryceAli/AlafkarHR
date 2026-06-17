namespace Payroll.Salaries.Features.EmployeeLoans.CreateEmployeeLoan;

public class CreateEmployeeLoanHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateEmployeeLoanCommand, CreateEmployeeLoanResult>
{
    public async Task<CreateEmployeeLoanResult> Handle(CreateEmployeeLoanCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var dto = request.EmployeeLoan;
        var loan = EmployeeLoan.Create(
            dto.CompanyId,
            dto.EmployeeId,
            dto.Type,
            dto.Amount,
            dto.InstallmentAmount,
            dto.StartMonth,
            dto.StartYear,
            dto.ReferenceNo,
            dto.Notes,
            userId);

        await dbContext.EmployeeLoans.AddAsync(loan, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateEmployeeLoanResult(loan.Id, loan.Status.ToString(), "Loan or deduction created successfully");
    }
}
