namespace Pricing.Pricings.Features.PriceListItems.GetPriceListItems;

public record GetPriceListItemsQuery(Guid PriceListId) : IQuery<GetPriceListItemsResult>;
public record GetPriceListItemsResult(List<PriceListItemDto> Items);

public class GetPriceListItemsHandler(PricingDbContext dbContext)
    : IQueryHandler<GetPriceListItemsQuery, GetPriceListItemsResult>
{
    public async Task<GetPriceListItemsResult> Handle(GetPriceListItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await dbContext.PriceListItems
            .AsNoTracking()
            .Where(i => i.PriceListId == request.PriceListId)
            .OrderBy(i => i.ProductSkuId)
            .ThenBy(i => i.MinQuantity)
            .Select(i => new PriceListItemDto
            {
                Id = i.Id,
                PriceListId = i.PriceListId,
                ProductSkuId = i.ProductSkuId,
                UnitId = i.UnitId,
                UnitPrice = i.UnitPrice,
                MinQuantity = i.MinQuantity,
                //IsActive = i.IsActive,
                //EffectiveFrom = i.EffectiveFrom,
                //EffectiveTo = i.EffectiveTo
            })
            .ToListAsync(cancellationToken);

        return new GetPriceListItemsResult(items);
    }
}
