namespace Catalog.Products.Features.Brands.GetBrandById;

public record GetBrandByIdQuery(Guid Id) : IQuery<GetBrandByIdResult>;
public record GetBrandByIdResult(BrandDto Brand);
public class GetBrandByIdHandler(CatalogDbContext dbContext) : IQueryHandler<GetBrandByIdQuery, GetBrandByIdResult>
{
    public async Task<GetBrandByIdResult> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
    {
        var brand = await dbContext.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Id == query.Id && x.DeletedAt==null, cancellationToken);

        return new GetBrandByIdResult(brand.Adapt<BrandDto>());
    }
}
