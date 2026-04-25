
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
                        .Where(v=> v.DeletedAt==null)
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

        return new GetProductByIdResult(product);
    }
}
