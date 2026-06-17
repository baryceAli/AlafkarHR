using Shared.Contracts.CQRS;
using SharedWithUI.Pricing.Dtos;

namespace Pricing.Contracts.Pricings.Features.ResolvePrice;

public record ResolvePriceQuery(
    Guid CustomerId,
    Guid ProductSkuId,
    Guid? UnitId,
    decimal Quantity,
    Guid CompanyId,
    Guid? RequestedPriceListId,
    decimal RequestedTaxRate,
    DateTime PriceDate,
    string? CouponCode = null,
    decimal? OrderSubtotal = null) : IQuery<ResolvePriceResult>;

public record ResolvePriceResult(ResolvedPriceDto Price);
