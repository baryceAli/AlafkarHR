namespace Payroll.Salaries.Features.Contracts.DeleteContract;

public class DeleteContractHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteContractCommand, DeleteContractResult>
{
    public async Task<DeleteContractResult> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var contract = await dbContext.Contracts
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Contract with ID {request.Id} not found");

        contract.Remove(userId);
        dbContext.Contracts.Update(contract);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteContractResult(true);
    }
}
