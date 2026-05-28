namespace Pricing.Pricings.Features.PriceLists.GetPriceListsByCompany;

public record GetPriceListsByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetPriceListsByCompanyResult>;
public record GetPriceListsByCompanyResult(PaginatedResult<PriceListDto> PriceList);

public class GetPriceListsByCompanyHandler(PricingDbContext dbContext)
    : IQueryHandler<GetPriceListsByCompanyQuery, GetPriceListsByCompanyResult>
{
    public async Task<GetPriceListsByCompanyResult> Handle(GetPriceListsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.PriceLists.Include(pl=>pl.Items).AsNoTracking().Where(p => p.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            query = query.Where(p =>
                p.Name.Contains(request.PaginationRequest.SearchText) ||
                p.Code.Contains(request.PaginationRequest.SearchText));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var priceLists = await query
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(p => new PriceListDto
            { 
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                CompanyId = p.CompanyId,
                CurrencyCode = p.CurrencyCode,
                IsDefault = p.IsDefault,
                IsActive = p.IsActive,
                EffectiveFrom = p.EffectiveFrom,
                EffectiveTo = p.EffectiveTo,
                Items= p.Items.Select(i=> new PriceListItemDto
                {
                    Id = i.Id,
                    PriceListId = i.PriceListId,
                    //ProductId = i.ProductId,
                    UnitPrice = i.UnitPrice,
                    MinQuantity=i.MinQuantity,
                    ProductSkuId = i.ProductSkuId,
                    UnitId=i.UnitId,
                    //CurrencyCode = i.CurrencyCode,
                    //EffectiveFrom = i.EffectiveFrom,
                    //EffectiveTo = i.EffectiveTo
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new GetPriceListsByCompanyResult(new PaginatedResult<PriceListDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            priceLists));
    }
}
