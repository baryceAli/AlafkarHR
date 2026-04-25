namespace Catalog.Products.Features.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
public record GetProductByIdResult(ProductDto Product);

public class GetProductByIdHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await (
            from p in dbContext.Products
            //.Include(x => x.ProductVariants)
            //.Include(x => x.Packages)

                join c in dbContext.Categories on p.CategoryId equals c.Id
                join b in dbContext.Brands on p.BrandId equals b.Id
                join u in dbContext.Units on p.UnitId equals u.Id

                where p.Id == request.Id 

                select new ProductDto {
                   Id= p.Id,
                   CategoryId= c.Id,
                   CategoryName= c.Name,
                    CategoryNameEng= c.NameEng,
                   BrandId= b.Id,
                   BrandName= b.Name,
                    BrandNameEng=b.NameEng,
                    UnitId=u.Id,
                    UnitName=u.UnitName,
                    UnitNameEng=u.UnitNameEng,
                    Name = p.Name,
                    NameEng = p.NameEng,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,

                    // ✅ FIX HERE
                   ProductSkus= p.ProductSkus
                        .Where(v=> v.DeletedAt==null)
                        .Select(v => new ProductSkuDto{
                            Id= v.Id,
                           VariantId= v.VariantId,
                            ProductId= v.ProductId,
                            PackageId= v.PackageId,
                            Sku= v.Sku,
                            SkuEng = v.SkuEng,
                            VariantValue = v.VariantValue,
                            Price = v.Price,
                            ShowOnStore = v.ShowOnStore
                        })
                        .ToList()

                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return new GetProductByIdResult(product);
    }
}
