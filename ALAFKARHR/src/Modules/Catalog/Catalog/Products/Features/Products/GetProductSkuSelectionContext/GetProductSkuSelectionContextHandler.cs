using Catalog.Contracts.Products.Features.GetProductSkuSelectionContext;

namespace Catalog.Products.Features.Products.GetProductSkuSelectionContext;

public class GetProductSkuSelectionContextHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductSkuSelectionContextQuery, GetProductSkuSelectionContextResult>
{
    public async Task<GetProductSkuSelectionContextResult> Handle(
        GetProductSkuSelectionContextQuery request,
        CancellationToken cancellationToken)
    {
        var sku = await (
            from productSku in dbContext.ProductSkus.AsNoTracking()
            join unit in dbContext.Units.AsNoTracking()
                on productSku.UnitId equals unit.Id
            where productSku.Id == request.ProductSkuId
                  && productSku.CompanyId == request.CompanyId
                  && !productSku.IsDeleted
                  && !unit.IsDeleted
            select new GetProductSkuSelectionContextResult(
                productSku.Id,
                productSku.ProductId,
                dbContext.ProductSkuPackages
                    .Where(package => package.ProductSkuId == productSku.Id && !package.IsDeleted)
                    .Select(package => (Guid?)package.ProductPackageId)
                    .FirstOrDefault(),
                productSku.Name,
                productSku.NameEng,
                productSku.SkuCode,
                productSku.SkuCodeEng,
                productSku.UnitId,
                unit.UnitName,
                unit.UnitNameEng,
                productSku.Calories))
            .FirstOrDefaultAsync(cancellationToken);

        if (sku is null)
            throw new Exception($"Product SKU not found or missing unit: {request.ProductSkuId}");

        return sku;
    }
}
