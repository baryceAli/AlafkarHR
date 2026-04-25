using Mapster;

namespace Catalog.Products.Features.Products.GetProductsByProductSKUIds;

public record GetProductsByProductSKUIdsQuery(List<Guid> ProductSkuIds) : IQuery<GetProductsByProductSKUIdsResult>;
public record GetProductsByProductSKUIdsResult(List<ProductDto> ProductList);
public class GetProductsByProductSKUIdsHandler(CatalogDbContext dbContext) : IQueryHandler<GetProductsByProductSKUIdsQuery, GetProductsByProductSKUIdsResult>
{
    public async Task<GetProductsByProductSKUIdsResult> Handle(GetProductsByProductSKUIdsQuery request, CancellationToken cancellationToken)
    {
        var products = await (
            from p in dbContext.Products

            join c in dbContext.Categories on p.CategoryId equals c.Id
            join b in dbContext.Brands on p.BrandId equals b.Id
            join u in dbContext.Units on p.UnitId equals u.Id
            join sku in dbContext.ProductSkus on p.Id equals sku.ProductId
            where request.ProductSkuIds.Any(x => x.Equals(sku.Id))

            select new ProductDto(
                p.Id,
                c.Id,
                c.Name,
                c.NameEng,
                b.Id,
                b.Name,
                b.NameEng,
                u.Id,
                u.UnitName,
                u.UnitNameEng,
                p.Name,
                p.NameEng,
                p.Price,
                p.ImageUrl,

                // ✅ FIX HERE
                p.ProductSkus
                    .Where(v => v.DeletedAt == null)
                    .Select(v => new ProductSkuDto(
                        v.Id,
                        v.VariantId,
                        v.ProductId,
                        v.PackageId,
                        v.Sku,
                        v.SkuEng,
                        v.VariantValue,
                        v.Price,
                        v.ShowOnStore
                    ))
                    .ToList()

            )
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);
            //.FirstOrDefaultAsync(cancellationToken);
        return new GetProductsByProductSKUIdsResult(products.Adapt<List<ProductDto>>());
    }
}
