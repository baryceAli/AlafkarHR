namespace Pricing.Pricings.Features.PricingResolution.GetResolvedPriceListItem;

public record GetResolvedPriceListItemQuery(
    Guid PriceListId,
    Guid CompanyId,
    Guid ProductSkuId,
    Guid? UnitId,
    decimal Quantity,
    DateTime PriceDate) : IQuery<GetResolvedPriceListItemResult>;

public record GetResolvedPriceListItemResult(Guid? PriceListId, decimal? UnitPrice);

public class GetResolvedPriceListItemHandler(PricingDbContext dbContext)
    : IQueryHandler<GetResolvedPriceListItemQuery, GetResolvedPriceListItemResult>
{
    public async Task<GetResolvedPriceListItemResult> Handle(GetResolvedPriceListItemQuery request, CancellationToken cancellationToken)
    {
        var priceList = await dbContext.PriceLists
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PriceListId &&
                                      p.CompanyId == request.CompanyId &&
                                      p.IsActive &&
                                      p.EffectiveFrom <= request.PriceDate &&
                                      (!p.EffectiveTo.HasValue || p.EffectiveTo.Value >= request.PriceDate),
                cancellationToken);

        if (priceList is null)
            return new GetResolvedPriceListItemResult(null, null);

        var item = await dbContext.PriceListItems
            .AsNoTracking()
            .Where(i => i.PriceListId == priceList.Id &&
                        i.ProductSkuId == request.ProductSkuId &&
                        i.IsActive &&
                        (!i.UnitId.HasValue || i.UnitId == request.UnitId) &&
                        (!i.MinQuantity.HasValue || i.MinQuantity.Value <= request.Quantity) &&
                        i.EffectiveFrom <= request.PriceDate &&
                        (!i.EffectiveTo.HasValue || i.EffectiveTo.Value >= request.PriceDate))
            .OrderByDescending(i => i.UnitId.HasValue)
            .ThenByDescending(i => i.MinQuantity ?? 0m)
            .ThenByDescending(i => i.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? new GetResolvedPriceListItemResult(priceList.Id, null)
            : new GetResolvedPriceListItemResult(priceList.Id, item.UnitPrice);
    }
}
