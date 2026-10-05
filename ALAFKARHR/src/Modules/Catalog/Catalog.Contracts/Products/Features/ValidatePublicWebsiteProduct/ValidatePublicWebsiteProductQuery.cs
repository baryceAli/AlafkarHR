using Shared.Contracts.CQRS;

namespace Catalog.Contracts.Products.Features.ValidatePublicWebsiteProduct;

public record ValidatePublicWebsiteProductQuery(Guid CompanyId, Guid ProductSkuId) : IQuery<ValidatePublicWebsiteProductResult>;
public record ValidatePublicWebsiteProductResult(bool IsValid);
