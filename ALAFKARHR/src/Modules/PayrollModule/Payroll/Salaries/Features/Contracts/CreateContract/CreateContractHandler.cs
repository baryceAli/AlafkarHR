namespace Payroll.Salaries.Features.Contracts.CreateContract;

public class CreateContractHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateContractCommand, CreateContractResult>
{
    public async Task<CreateContractResult> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var contract = Contract.Create(
            Guid.NewGuid(),
            request.Name,
            request.NameEng,
            request.Description,
            request.CompanyId,
            userId);

        foreach (var item in request.ContractItems)
        {
            contract.AddContractItem(item.ComponentId, item.Value);
        }

        await dbContext.Set<Contract>().AddAsync(contract, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateContractResult(contract.Id, contract.Name);
    }
}
