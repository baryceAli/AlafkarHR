using Shared.Contracts.CQRS;
using SharedWithUI.Pricing.Dtos;

namespace Pricing.Contracts.Pricings.Features.ResolvePrices;

public record ResolvePricesQuery(
    Guid CustomerId,
    Guid CompanyId,
    Guid? RequestedPriceListId,
    DateTime PriceDate,
    IReadOnlyList<ResolvePriceLineDto> Lines) : IQuery<ResolvePricesResult>;

public record ResolvePriceLineDto(
    Guid ProductSkuId,
    Guid? UnitId,
    decimal Quantity,
    decimal RequestedTaxRate,
    string? CouponCode = null,
    decimal? OrderSubtotal = null);

public record ResolvePricesResult(IReadOnlyList<ResolvedPriceDto> Prices);
