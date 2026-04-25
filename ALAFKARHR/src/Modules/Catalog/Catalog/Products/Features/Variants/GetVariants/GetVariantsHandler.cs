namespace Catalog.Products.Features.Variants.GetVariants;

public record GetVariantsQuery(PaginationRequest PaginationRequest) : IQuery<GetVariantsResult>;
public record GetVariantsResult(PaginatedResult<VariantDto> VariantList);

public class GetVariantsHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetVariantsQuery, GetVariantsResult>
{
    public async Task<GetVariantsResult> Handle(GetVariantsQuery request, CancellationToken cancellationToken)
    {
        var pageIndex= request.PaginationRequest.PageIndex;
        var pageSize= request.PaginationRequest.PageSize;
        var count = await dbContext.Variants.LongCountAsync();

        var variants = await dbContext.Variants
            .AsNoTracking()
            .Where(x => x.DeletedAt == null)
            .Skip(pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return new GetVariantsResult(new PaginatedResult<VariantDto>(pageIndex, pageSize,count, variants.Adapt<List<VariantDto>>()));
    }
}
