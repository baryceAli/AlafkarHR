using Catalog.Products.Features.Products.GetProducts;
using Mapster;

namespace Catalog.Products.Features.Products.GetProductsByCategoryId;

public record GetProductsByCategoryIdQuery(Guid categoryId, PaginationRequest PaginationRequest) : IQuery<GetProductsByCategoryIdResult>;
public record GetProductsByCategoryIdResult(PaginatedResult<ProductDto> ProductList);
public class GetProductsByCategoryIdHandler(CatalogDbContext dbContext) : IQueryHandler<GetProductsByCategoryIdQuery, GetProductsByCategoryIdResult>
{
    public async Task<GetProductsByCategoryIdResult> Handle(GetProductsByCategoryIdQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PaginationRequest.PageIndex;
        var pageSize = request.PaginationRequest.PageSize;
        var totalCount = await dbContext.Products.AsNoTracking().Where(p => p.CategoryId == request.categoryId).LongCountAsync(cancellationToken);

        var products = await (
            from p in dbContext.Products
                //.Include(x => x.ProductVariants)
                //.Include(x => x.Packages)

            join c in dbContext.Categories on p.CategoryId equals c.Id
            join b in dbContext.Brands on p.BrandId equals b.Id
            join u in dbContext.Units on p.UnitId equals u.Id

            where p.DeletedAt == null
                && p.CategoryId == request.categoryId

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
            .ToListAsync();


        var productDtos = products.Adapt<List<ProductDto>>();

        //var productDtos=ProjectToProductDto(products);

        return new GetProductsByCategoryIdResult(new PaginatedResult<ProductDto>(
            pageIndex,
            pageSize,
            totalCount,
            productDtos
            ));
    }
}
