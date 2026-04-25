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
            .ToListAsync(cancellationToken);
            //.FirstOrDefaultAsync(cancellationToken);
        return new GetProductsByProductSKUIdsResult(products.Adapt<List<ProductDto>>());
    }
}
