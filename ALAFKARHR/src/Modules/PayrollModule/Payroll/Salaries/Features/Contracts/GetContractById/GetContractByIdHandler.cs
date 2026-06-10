using Payroll.Data;
namespace Payroll.Salaries.Features.Contracts.GetContractById;

public class GetContractByIdHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetContractByIdQuery, GetContractByIdResult>
{
    public async Task<GetContractByIdResult> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await dbContext.Set<Contract>()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"Contract with ID {request.Id} not found");

        return new GetContractByIdResult(
            contract.Id,
            contract.Name,
            contract.NameEng,
            contract.Description,
            contract.TaxPercentage,
            contract.InsurancePercentage,
            contract.CompanyId,
            contract.Items.Select(x => new ContractItemDto
            {
                ComponentId = x.ComponentId,
                Value = x.Amount
            }).ToList());
    }
}
