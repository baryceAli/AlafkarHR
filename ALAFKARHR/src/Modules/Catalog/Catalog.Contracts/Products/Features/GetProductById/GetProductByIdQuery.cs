using Shared.Contracts.CQRS;
using SharedWithUI.Catalog.Dtos;

namespace Catalog.Contracts.Products.Features.GetProductById;


public record GetProductByIdQuery(Guid ProductId) : IQuery<GetProductByIdResult>;
public record GetProductByIdResult(ProductDto Product);