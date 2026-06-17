namespace Pricing.Pricings.Services;

public record ResolvePriceRequest(
    Guid CustomerId,
    Guid ProductSkuId,
    Guid? UnitId,
    decimal Quantity,
    Guid CompanyId,
    Guid? RequestedPriceListId,
    decimal RequestedTaxRate,
    DateTime PriceDate,
    string? CouponCode = null,
    decimal? OrderSubtotal = null);

public interface IPriceResolver
{
    Task<ResolvedPriceDto> ResolveAsync(ResolvePriceRequest request, CancellationToken cancellationToken);
}
