using Shared.Contracts.CQRS;
using SharedWithUI.Catalog.Dtos;

namespace Catalog.Contracts.Products.Features.GetProductSkuById;

public record GetProductSkuByIdQuery(Guid Id) : IQuery<GetProductSkuByIdResult>;
public record GetProductSkuByIdResult(ProductSkuDto ProductSku);
