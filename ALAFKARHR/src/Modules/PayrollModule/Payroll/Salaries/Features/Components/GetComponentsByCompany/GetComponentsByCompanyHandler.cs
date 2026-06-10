namespace Payroll.Salaries.Features.Components.GetComponentsByCompany;

public class GetComponentsByCompanyHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetComponentsByCompanyQuery, GetComponentsByCompanyResult>
{
    public async Task<GetComponentsByCompanyResult> Handle(GetComponentsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Components
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.IsActive && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            query = query.Where(x =>
                x.Name.Contains(request.PaginationRequest.SearchText) ||
                x.NameEng.Contains(request.PaginationRequest.SearchText));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var components = await query
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(x => new ComponentDto
            {
                Id = x.Id,
                Name = x.Name,
                NameEng = x.NameEng,
                ComponentType = x.ComponentType,
                IsTaxable = x.IsTaxable,
                IsActive = x.IsActive,
                Order = x.Order,
                Description = x.Description,
                CompanyId = x.CompanyId
            })
            .ToListAsync(cancellationToken);

        return new GetComponentsByCompanyResult(new PaginatedResult<ComponentDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            components));
    }
}
