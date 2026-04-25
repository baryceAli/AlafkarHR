using Shared.Contracts.CQRS;
using SharedWithUI.Catalog.Dtos;

namespace Catalog.Contracts.Products.Features.GetProductByProductSKUId;


public record GetProductByProductSKUIdQuery(Guid SkuId) : IQuery<GetProductByProductSKUIdResult>;
public record GetProductByProductSKUIdResult(ProductDto Product);
