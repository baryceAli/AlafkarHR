using Shared.Pagination;

namespace CustomersModule.Customers.Features.CustomerPricingProfiles.GetCustomerPricingProfilesByCompanyId;

public record GetCustomerPricingProfilesByCompanyIdQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetCustomerPricingProfilesByCompanyIdResult>;
public record GetCustomerPricingProfilesByCompanyIdResult(PaginatedResult<CustomerPricingProfileDto> CustomerPricingProfileList);
public class GetCustomerPricingProfilesByCompanyIdHandler(CustomerDbContext dbContext)
    : IQueryHandler<GetCustomerPricingProfilesByCompanyIdQuery, GetCustomerPricingProfilesByCompanyIdResult>
{
    public async Task<GetCustomerPricingProfilesByCompanyIdResult> Handle(GetCustomerPricingProfilesByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CustomerPricingProfiles.AsNoTracking().AsQueryable();
        query=query.Where(c=>c.CompanyId==request.CompanyId);

        var searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            //query=query.Where(c=>c.)
        }
        var count = await query.LongCountAsync(cancellationToken);

        var customerPricingProfile=await query.OrderBy(c=>c.CreatedAt)
                                                .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                                                .Take(request.PaginationRequest.PageSize)
                                                .ToListAsync(cancellationToken);

        return new GetCustomerPricingProfilesByCompanyIdResult(new PaginatedResult<CustomerPricingProfileDto>(
                        request.PaginationRequest.PageIndex, request.PaginationRequest.PageSize, count,
                        customerPricingProfile.Adapt<List<CustomerPricingProfileDto>>()
            ));
    }
}
