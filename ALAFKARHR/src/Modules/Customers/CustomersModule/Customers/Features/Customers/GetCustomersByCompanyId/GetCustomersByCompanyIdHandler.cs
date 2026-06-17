using Shared.Pagination;

namespace CustomersModule.Customers.Features.Customers.GetCustomersByCompanyId;

public record GetCustomersByCompanyIdQuery(Guid CompanyId, PaginationRequest PaginationRequest) : ICommand<GetCustomersByCompanyIdResult>;
public record GetCustomersByCompanyIdResult(PaginatedResult<CustomerDto> CustomerList);
public class GetCustomersByCompanyIdHandler(CustomerDbContext dbContext)
    : ICommandHandler<GetCustomersByCompanyIdQuery, GetCustomersByCompanyIdResult>
{
    public async Task<GetCustomersByCompanyIdResult> Handle(GetCustomersByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Customers.AsNoTracking().AsQueryable();
        query = query.Where(c => c.CompanyId == request.CompanyId);

        var searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(c =>
                c.Name.Contains(searchText) ||
                (c.CustomerCode != null && c.CustomerCode.Contains(searchText)) ||
                (c.CommercialName != null && c.CommercialName.Contains(searchText)) ||
                (c.VatNumber != null && c.VatNumber.Contains(searchText)) ||
                (c.CommercialRegistrationNumber != null && c.CommercialRegistrationNumber.Contains(searchText)));
        }

        var count = await query.LongCountAsync(cancellationToken);

        var customers = await query
                               .Include(c => c.Addresses)
                               .Include(c => c.Contacts)
                               .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                               .Take(request.PaginationRequest.PageSize)
                               .ToListAsync(cancellationToken);

        var customerDtoList = customers.Adapt<List<CustomerDto>>();
        return new GetCustomersByCompanyIdResult(new PaginatedResult<CustomerDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            customerDtoList));
    }
}
