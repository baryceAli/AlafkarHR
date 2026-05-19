using Microsoft.EntityFrameworkCore;
using Shared.Pagination;

namespace CustomersModule.Customers.Features.CustomerGroups.GetCustomerGroupByCompanyId;


public record GetCustomerGroupByCompanyIdQuery(Guid CompanyId,PaginationRequest PaginationRequest) : IQuery<GetCustomerGroupByCompanyIdResult>;
public record GetCustomerGroupByCompanyIdResult(PaginatedResult<CustomerGroupDto> CustomerGroupList);
public class GetCustomerGroupByCompanyIdHandler(CustomerDbContext dbContext)
    : IQueryHandler<GetCustomerGroupByCompanyIdQuery, GetCustomerGroupByCompanyIdResult>
{
    public async Task<GetCustomerGroupByCompanyIdResult> Handle(GetCustomerGroupByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CustomerGroups.AsNoTracking().AsQueryable();
    
        query=query.Where(c=>c.CompanyId==request.CompanyId);

        string searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query=query.Where(c=> c.Name.ToLower().Contains(searchText.ToLower()));
        }

        var count = await query.LongCountAsync(cancellationToken);

        var customers= query
                        .OrderBy(c=>c.CreatedAt)
                        .Skip(request.PaginationRequest.PageSize * request.PaginationRequest.PageIndex)
                        .Take(request.PaginationRequest.PageSize)
                        .ToListAsync(cancellationToken);

        return new GetCustomerGroupByCompanyIdResult(
                    new PaginatedResult<CustomerGroupDto>(
                            request.PaginationRequest.PageIndex, 
                            request.PaginationRequest.PageSize, 
                            count, 
                            customers.Adapt<List<CustomerGroupDto>>())
            );
    }
}
