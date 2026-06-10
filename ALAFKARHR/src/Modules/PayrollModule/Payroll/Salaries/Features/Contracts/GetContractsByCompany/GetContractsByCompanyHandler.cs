namespace Payroll.Salaries.Features.Contracts.GetContractsByCompany;

public class GetContractsByCompanyHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetContractsByCompanyQuery, GetContractsByCompanyResult>
{
    public async Task<GetContractsByCompanyResult> Handle(GetContractsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Contracts
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            query = query.Where(x =>
                x.Name.Contains(request.PaginationRequest.SearchText) ||
                x.NameEng.Contains(request.PaginationRequest.SearchText));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var contracts = await query
            .OrderBy(x => x.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(x => new ContractDto
            {
                Id = x.Id,
                Name = x.Name,
                NameEng = x.NameEng,
                Description = x.Description,
                TaxPercentage = x.TaxPercentage,
                InsurancePercentage = x.InsurancePercentage,
                CompanyId = x.CompanyId,
                Items = x.Items.Select(i => new ContractItemDto
                {
                    ComponentId = i.ComponentId,
                    Value = i.Amount
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new GetContractsByCompanyResult(new PaginatedResult<ContractDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            contracts));
    }
}
