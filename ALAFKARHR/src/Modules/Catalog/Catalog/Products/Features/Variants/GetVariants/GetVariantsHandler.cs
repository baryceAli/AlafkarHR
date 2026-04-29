namespace Catalog.Products.Features.Variants.GetVariants;

public record GetVariantsQuery(PaginationRequest PaginationRequest) : IQuery<GetVariantsResult>;
public record GetVariantsResult(PaginatedResult<VariantDto> VariantList);

public class GetVariantsHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetVariantsQuery, GetVariantsResult>
{
    public async Task<GetVariantsResult> Handle(GetVariantsQuery request, CancellationToken cancellationToken)
    {
        //var query = dbContext.Variants.Include(x=> x.Values).AsQueryable();

        var pageIndex = request.PaginationRequest.PageIndex;
        var pageSize = request.PaginationRequest.PageSize;
        var count = await dbContext.Variants.LongCountAsync(x => !x.IsDeleted, cancellationToken);



        var variants = await dbContext.Variants
            .Where(x => !x.IsDeleted)
            .Select(x => new VariantDto
            {
                Id = x.Id,
                Name = x.Name,
                Values = x.Values
                    .Where(v => !v.IsDeleted)
                    .Select(v => new VariantValueDto
                    {
                        Id = v.Id,
                        Value = v.Value
                    }).ToList()
            })
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetVariantsResult(new PaginatedResult<VariantDto>(pageIndex, pageSize, count, variants));
    }
}
