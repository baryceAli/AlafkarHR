namespace Payroll.Salaries.Features.EmployeeLoans.ApproveEmployeeLoan;

public class ApproveEmployeeLoanHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ApproveEmployeeLoanCommand, ApproveEmployeeLoanResult>
{
    public async Task<ApproveEmployeeLoanResult> Handle(ApproveEmployeeLoanCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var loan = await dbContext.EmployeeLoans
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Loan or deduction with ID {request.Id} not found");

        loan.Approve(userId);
        await EnsureLoanDeductionComponentAsync(dbContext, loan.CompanyId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApproveEmployeeLoanResult(loan.Id, loan.Status.ToString(), "Loan or deduction approved successfully");
    }

    private static async Task EnsureLoanDeductionComponentAsync(PayrollDbContext dbContext, Guid companyId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Components
            .AnyAsync(x => x.CompanyId == companyId && x.NameEng == "Loan/Deduction" && !x.IsDeleted, cancellationToken);

        if (exists)
        {
            return;
        }

        var order = await dbContext.Components
            .Where(x => x.CompanyId == companyId)
            .Select(x => (int?)x.Order)
            .MaxAsync(cancellationToken) ?? 0;

        var component = Component.Create(
            Guid.NewGuid(),
            "السلف والخصومات",
            "Loan/Deduction",
            ComponentType.Deduction,
            false,
            order + 1,
            "System component for employee loan and one-month deduction repayments",
            companyId);

        await dbContext.Components.AddAsync(component, cancellationToken);
    }
}
