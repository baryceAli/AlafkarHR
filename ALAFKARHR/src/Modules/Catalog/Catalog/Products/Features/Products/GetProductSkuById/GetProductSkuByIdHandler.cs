namespace Catalog.Products.Features.Products.GetProductSkuById;

public record GetProductSkuByIdQuery(Guid Id):IQuery<GetProductSkuByIdResult>;
public record GetProductSkuByIdResult(ProductSkuDto ProductSku);
public class GetProductSkuByIdHandler (CatalogDbContext dbContext): IQueryHandler<GetProductSkuByIdQuery, GetProductSkuByIdResult>
{
    public async Task<GetProductSkuByIdResult> Handle(GetProductSkuByIdQuery request, CancellationToken cancellationToken)
    {
        var productSku = await dbContext.ProductSkus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id);
        if(productSku is null)
            throw new Exception($"Product Sku not found: {request.Id}");

        return new GetProductSkuByIdResult(productSku.Adapt<ProductSkuDto>());
    }
}
