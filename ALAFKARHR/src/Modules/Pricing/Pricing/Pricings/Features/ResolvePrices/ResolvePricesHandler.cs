using Pricing.Contracts.Pricings.Features.ResolvePrices;

namespace Pricing.Pricings.Features.ResolvePrices;

public class ResolvePricesHandler(IPriceResolver priceResolver)
    : IQueryHandler<ResolvePricesQuery, ResolvePricesResult>
{
    public async Task<ResolvePricesResult> Handle(ResolvePricesQuery request, CancellationToken cancellationToken)
    {
        var prices = new List<ResolvedPriceDto>();

        foreach (var line in request.Lines)
        {
            var price = await priceResolver.ResolveAsync(
                new ResolvePriceRequest(
                    request.CustomerId,
                    line.ProductSkuId,
                    line.UnitId,
                    line.Quantity,
                    request.CompanyId,
                    request.RequestedPriceListId,
                    line.RequestedTaxRate,
                    request.PriceDate,
                    line.CouponCode,
                    line.OrderSubtotal),
                cancellationToken);

            prices.Add(price);
        }

        return new ResolvePricesResult(prices);
    }
}
