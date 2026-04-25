using Catalog.Contracts.Products.Features.GetProductByProductSKUId;

namespace Catalog.Products.Features.Products.GetProductByProductSKUId;

public class GetProductByProductSKUIdHandler(CatalogDbContext dbContext) 
    : IQueryHandler<GetProductByProductSKUIdQuery, GetProductByProductSKUIdResult>
{
    public async Task<GetProductByProductSKUIdResult> Handle(GetProductByProductSKUIdQuery request, CancellationToken cancellationToken)
    {
        var productSku = await dbContext.ProductSkus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.SkuId);

        if (productSku is null)
            throw new Exception($"ProductSku not found: {request.SkuId}");

        //var product= await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x=> x.Id==productSku.ProductId);
        var product = await (
            from p in dbContext.Products

            join c in dbContext.Categories on p.CategoryId equals c.Id
            join b in dbContext.Brands on p.BrandId equals b.Id
            join u in dbContext.Units on p.UnitId equals u.Id

            where p.Id == productSku.ProductId

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
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            throw new Exception($"The ProductSku of ({request.SkuId}) doesn't belong to any product");

        return new GetProductByProductSKUIdResult(product);
    }
}
