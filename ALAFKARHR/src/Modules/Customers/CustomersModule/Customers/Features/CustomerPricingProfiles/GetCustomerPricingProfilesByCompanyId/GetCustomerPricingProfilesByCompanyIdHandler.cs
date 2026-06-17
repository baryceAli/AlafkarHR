using Shared.Pagination;

namespace CustomersModule.Customers.Features.CustomerPricingProfiles.GetCustomerPricingProfilesByCompanyId;

public record GetCustomerPricingProfilesByCompanyIdQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetCustomerPricingProfilesByCompanyIdResult>;
public record GetCustomerPricingProfilesByCompanyIdResult(PaginatedResult<CustomerPricingProfileDto> CustomerPricingProfileList);

public class GetCustomerPricingProfilesByCompanyIdHandler(CustomerDbContext dbContext, ISender sender)
    : IQueryHandler<GetCustomerPricingProfilesByCompanyIdQuery, GetCustomerPricingProfilesByCompanyIdResult>
{
    public async Task<GetCustomerPricingProfilesByCompanyIdResult> Handle(GetCustomerPricingProfilesByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CustomerPricingProfiles.AsNoTracking().Where(c => c.CompanyId == request.CompanyId);

        var plResult = await sender.Send(new GetPriceListsByCompanyQuery(request.CompanyId, new PaginationRequest(0, int.MaxValue, null)), cancellationToken);
        var priceLists = plResult.PriceList.Data.ToList();

        var customerProfileQuery =
            from cpp in query
            join c in dbContext.Customers on cpp.CustomerId equals c.Id
            select new CustomerPricingProfileDto
            {
                Id = cpp.Id,
                CustomerId = cpp.CustomerId,
                CustomerName = c.Name,
                PriceListId = cpp.PriceListId,
                DiscountPercentage = cpp.DiscountPercentage,
                AllowAdditionalDiscounts = cpp.AllowAdditionalDiscounts,
                EffectiveFrom = cpp.EffectiveFrom,
                EffectiveTo = cpp.EffectiveTo,
                CompanyId = cpp.CompanyId
            };

        var searchText = request.PaginationRequest.SearchText;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var priceListIds = priceLists
                .Where(x => x.Name.Contains(searchText))
                .Select(x => x.Id)
                .ToList();

            customerProfileQuery = customerProfileQuery.Where(x =>
                (x.CustomerName != null && x.CustomerName.Contains(searchText)) ||
                priceListIds.Contains(x.PriceListId));
        }

        var count = await customerProfileQuery.LongCountAsync(cancellationToken);

        var customerProfileListDtos = await customerProfileQuery
            .OrderBy(x => x.CustomerName)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        var priceListDict = priceLists.ToDictionary(x => x.Id, x => x.Name);

        foreach (var dto in customerProfileListDtos)
        {
            dto.PriceListName = priceListDict.GetValueOrDefault(dto.PriceListId);
        }

        return new GetCustomerPricingProfilesByCompanyIdResult(new PaginatedResult<CustomerPricingProfileDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            customerProfileListDtos));
    }
}
