using Payroll.Data;
using Payroll.Salaries.Features.Contracts.CreateContract;

namespace Payroll.Salaries.Features.Contracts.GetContractById;

public class GetContractByIdHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetContractByIdQuery, GetContractByIdResult>
{
    public async Task<GetContractByIdResult> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Set<Contract>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Contract with ID {request.Id} not found");

        return new GetContractByIdResult(
            contract.Id,
            contract.Name,
            contract.NameEng,
            contract.Description,
            contract.CompanyId,
            contract.Items.Select(x => new ContractItemDto(x.ComponentId, x.Amount)).ToList());
    }
}
