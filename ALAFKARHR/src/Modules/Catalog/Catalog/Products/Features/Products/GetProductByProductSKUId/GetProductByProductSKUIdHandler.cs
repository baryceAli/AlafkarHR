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
            //join b in dbContext.Brands on p.BrandId equals b.Id
            join u in dbContext.Units on p.UnitId equals u.Id

            where p.Id == productSku.ProductId

            select new ProductDto{
                Id= p.Id,
                CategoryId= c.Id,
                CategoryName= c.Name,
                CategoryNameEng= c.NameEng,
                //BrandId= b.Id,
                //BrandName= b.Name,
                //BrandNameEng= b.NameEng,
                UnitId= u.Id,
                UnitName= u.UnitName,
                 UnitNameEng=u.UnitNameEng,
                Name = p.Name,
                NameEng = p.NameEng,
                //Price = p.Price,
                //ImageUrl = p.ImageUrl,

                // ✅ FIX HERE
               //ProductSkus= p.ProductSkus
               //     .Where(v => v.DeletedAt == null)
               //     .Select(v => new ProductSkuDto{
               //         Id= v.Id,
               //        VariantId= v.VariantId,
               //         ProductId= v.ProductId,
               //         PackageId= v.PackageId,
               //         Sku= v.Sku,
               //         SkuEng= v.SkuEng,
               //          VariantValue=v.VariantValue,
               //         Price = v.Price,
               //         ShowOnStore = v.ShowOnStore
               //     })
               //     .ToList()

            }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            throw new Exception($"The ProductSku of ({request.SkuId}) doesn't belong to any product");

        return new GetProductByProductSKUIdResult(product);
    }
}
