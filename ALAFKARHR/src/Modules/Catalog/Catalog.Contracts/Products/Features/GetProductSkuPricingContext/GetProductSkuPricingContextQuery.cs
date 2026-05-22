using Shared.Contracts.CQRS;

namespace Catalog.Contracts.Products.Features.GetProductSkuPricingContext;

public record GetProductSkuPricingContextQuery(Guid ProductSkuId, Guid CompanyId)
    : IQuery<GetProductSkuPricingContextResult>;

public record GetProductSkuPricingContextResult(
    Guid ProductSkuId,
    Guid ProductId,
    Guid UnitId,
    decimal BasePrice);
