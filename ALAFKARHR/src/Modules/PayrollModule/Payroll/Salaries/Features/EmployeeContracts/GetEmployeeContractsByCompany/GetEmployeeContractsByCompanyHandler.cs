namespace Payroll.Salaries.Features.EmployeeContracts.GetEmployeeContractsByCompany;

public class GetEmployeeContractsByCompanyHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetEmployeeContractsByCompanyQuery, GetEmployeeContractsByCompanyResult>
{
    public async Task<GetEmployeeContractsByCompanyResult> Handle(GetEmployeeContractsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.EmployeeContracts
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

        var count = await query.LongCountAsync(cancellationToken);
        var assignments = await query
            .OrderByDescending(x => x.IsActive)
            .ThenByDescending(x => x.EffectiveFrom)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(x => new EmployeeContractDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                ContractId = x.ContractId,
                CompanyId = x.CompanyId,
                EffectiveFrom = x.EffectiveFrom,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new GetEmployeeContractsByCompanyResult(new PaginatedResult<EmployeeContractDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            assignments));
    }
}
