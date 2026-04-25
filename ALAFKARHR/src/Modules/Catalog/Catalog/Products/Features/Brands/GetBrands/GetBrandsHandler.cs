using Mapster;

namespace Catalog.Products.Features.Brands.GetBrands;

public record GetBrandsQuery(PaginationRequest PaginationRequest) : ICommand<GetBrandsResult>;
public record GetBrandsResult(PaginatedResult<BrandDto> Brands);

public class GetBrandsHandler(CatalogDbContext dbContext) : ICommandHandler<GetBrandsQuery, GetBrandsResult>
{
    public async Task<GetBrandsResult> Handle(GetBrandsQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        var totalCount = await dbContext.Brands.LongCountAsync(cancellationToken);

        var brands = await dbContext.Brands
            .Where(x=> x.DeletedAt==null)
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var brandDtos = brands.Adapt<List<BrandDto>>();

        //var productDtos=ProjectToProductDto(products);

        return new GetBrandsResult(new PaginatedResult<BrandDto>(
            pageIndex,
            pageSize,
            totalCount,
            brandDtos
            ));
    }
}
