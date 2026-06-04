namespace Payroll.Salaries.Features.Contracts.UpdateContract;

public class UpdateContractHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateContractCommand, UpdateContractResult>
{
    public async Task<UpdateContractResult> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var contract = await dbContext.Set<Contract>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Contract with ID {request.Id} not found");

        contract.Update(request.Name, request.NameEng, request.Description, userId);

        // Clear existing items and add new ones
        contract.ClearItems();
        foreach (var item in request.ContractItems)
        {
            contract.AddContractItem(item.ComponentId, item.Value);
        }

        dbContext.Set<Contract>().Update(contract);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateContractResult(contract.Id, contract.Name);
    }
}
