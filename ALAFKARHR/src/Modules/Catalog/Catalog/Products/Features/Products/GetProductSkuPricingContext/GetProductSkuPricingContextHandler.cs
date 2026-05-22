using Catalog.Contracts.Products.Features.GetProductSkuPricingContext;

namespace Catalog.Products.Features.Products.GetProductSkuPricingContext;

public class GetProductSkuPricingContextHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductSkuPricingContextQuery, GetProductSkuPricingContextResult>
{
    public async Task<GetProductSkuPricingContextResult> Handle(GetProductSkuPricingContextQuery request, CancellationToken cancellationToken)
    {
        var sku = await dbContext.ProductSkus
            .AsNoTracking()
            .Where(s => s.Id == request.ProductSkuId && s.CompanyId == request.CompanyId)
            .Select(s => new GetProductSkuPricingContextResult(
                s.Id,
                s.ProductId,
                s.UnitId,
                s.Price))
            .FirstOrDefaultAsync(cancellationToken);

        if (sku is null)
            throw new Exception($"Product SKU not found: {request.ProductSkuId}");

        return sku;
    }
}
