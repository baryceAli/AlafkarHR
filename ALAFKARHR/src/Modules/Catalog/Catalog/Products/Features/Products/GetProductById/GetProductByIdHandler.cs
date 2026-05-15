using Catalog.Contracts.Products.Features.GetProductById;

namespace Catalog.Products.Features.Products.GetProductById;

//public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
//public record GetProductByIdResult(ProductDto Product);

public class GetProductByIdHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await (
            from p in dbContext.Products
            .Include(x => x.Skus)
            .ThenInclude(s => s.Variants)
            //.Include(x => x.Packages)

            join c in dbContext.Categories on p.CategoryId equals c.Id
                //join b in dbContext.Brands on p.Skus equals b.Id
                //join u in dbContext.Units on p.UnitId equals u.Id

                where p.Id == request.ProductId 

                select new ProductDto {
                   Id= p.Id,
                   CategoryId= c.Id,
                   CategoryName= c.Name,
                    CategoryNameEng= c.NameEng,
                   //BrandId= b.Id,
                   //BrandName= b.Name,
                    //BrandNameEng=b.NameEng,
                    //UnitId=u.Id,
                    //UnitName=u.UnitName,
                    //UnitNameEng=u.UnitNameEng,
                    Name = p.Name,
                    NameEng = p.NameEng,
                    //Price = p.Price,
                    //ImageUrl = p.ImageUrl,

                    // ✅ FIX HERE
                    Skus = p.Skus
                        .Where(sku => sku.IsDeleted == false)
                        .Select(sku => new ProductSkuDto
                        {
                            Id = sku.Id,
                            BrandId=sku.BrandId,
                            ProductId = sku.ProductId,
                            PackageId = sku.PackageId,
                            Name=sku.Name,
                            NameEng=sku.NameEng,
                            SkuCode = sku.SkuCode,
                            SkuCodeEng = sku.SkuCodeEng,
                            Barcode=sku.Barcode,
                            CompanyId=sku.CompanyId,
                            ImageUrl=sku.ImageUrl,
                            SkuKey=sku.SkuKey,
                            UnitId=sku.UnitId,
                            //Variants=sku.Variants,
                            Price = sku.Price,
                            ShowOnStore = sku.ShowOnStore,
                            // ✅ ADD THIS
                            Variants = sku.Variants
                                .Where(v => !v.IsDeleted)
                                .Select(v => new ProductSkuVariantDto
                                {
                                    Id= v.Id,
                                    ProductSkuId = v.ProductSkuId,
                                    VariantId = v.VariantId,
                                    VariantValueId = v.VariantValueId
                                })
                                .ToList()

                        })
                        .ToList()

                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return new GetProductByIdResult(product);
    }
}
