namespace Catalog.Products.Features.Products.GetProducts;


public record GetProductsQuery(PaginationRequest PaginationRequest) : IQuery<GetProductsResult>;
public record GetProductsResult(PaginatedResult<ProductDto> ProductList);
public class GetProductsHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PaginationRequest.PageIndex;
        var pageSize = request.PaginationRequest.PageSize;
        var totalCount = await dbContext.Products.LongCountAsync(cancellationToken);
        
        var products = await (
            from p in dbContext.Products
            //.Include(x => x.ProductVariants)
            //.Include(x => x.Packages)

            join c in dbContext.Categories on p.CategoryId equals c.Id
            join b in dbContext.Brands on p.BrandId equals b.Id
            join u in dbContext.Units on p.UnitId equals u.Id

            where p.DeletedAt == null

            select new ProductDto
            {
                Id = p.Id,
                CategoryId = c.Id,
                CategoryName = c.Name,
                CategoryNameEng = c.NameEng,
                BrandId = b.Id,
                BrandName = b.Name,
                BrandNameEng = b.NameEng,
                UnitId = u.Id,
                UnitName = u.UnitName,
                UnitNameEng = u.UnitNameEng,
                Name = p.Name,
                NameEng = p.NameEng,
                Price = p.Price,
                ImageUrl = p.ImageUrl,

                // ✅ FIX HERE
                ProductSkus = p.ProductSkus
        .Where(v => v.DeletedAt == null)
        .Select(v => new ProductSkuDto
        {
            Id = v.Id,
            VariantId = v.VariantId,
            ProductId = v.ProductId,
            PackageId = v.PackageId,
            Sku = v.Sku,
            SkuEng = v.SkuEng,
            VariantValue = v.VariantValue,
            Price = v.Price,
            ShowOnStore = v.ShowOnStore
        })
        .ToList()

            }
            )
            .AsNoTracking()
            .ToListAsync();


        var productDtos = products.Adapt<List<ProductDto>>();

        //var productDtos=ProjectToProductDto(products);

        return new GetProductsResult(new PaginatedResult<ProductDto>(
            pageIndex,
            pageSize,
            totalCount,
            productDtos
            ));
    }
}
