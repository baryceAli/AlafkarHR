using Pricing.Contracts.Pricings.Features.ResolvePrice;

namespace Pricing.Pricings.Features.ResolvePrice;

public class ResolvePriceHandler(IPriceResolver priceResolver)
    : IQueryHandler<ResolvePriceQuery, ResolvePriceResult>
{
    public async Task<ResolvePriceResult> Handle(ResolvePriceQuery request, CancellationToken cancellationToken)
    {
        var price = await priceResolver.ResolveAsync(
            new ResolvePriceRequest(
                request.CustomerId,
                request.ProductSkuId,
                request.UnitId,
                request.Quantity,
                request.CompanyId,
                request.RequestedPriceListId,
                request.RequestedTaxRate,
                request.PriceDate),
            cancellationToken);

        return new ResolvePriceResult(price);
    }
}
