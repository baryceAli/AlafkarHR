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
        var query = dbContext.Variants.Where(x => !x.IsDeleted);
        if (!request.PaginationRequest.IncludeInactive)
            query = query.Where(x => x.IsActive);

        var count = await query.LongCountAsync(cancellationToken);

        var variants = await query
            .Select(x => new VariantDto
            {
                Id = x.Id,
                Name = x.Name,
                NameEng = x.NameEng,
                DisplayType = x.DisplayType,
                CreationMode = x.CreationMode,
                IsActive = x.IsActive,
                CompanyId = x.CompanyId,
                Values = x.Values
                    .Where(v => !v.IsDeleted && (request.PaginationRequest.IncludeInactive || v.IsActive))
                    .Select(v => new VariantValueDto
                    {
                        Id = v.Id,
                        VariantId = v.VariantId,
                        Value = v.Value,
                        ValueEng = v.ValueEng,
                        IsActive = v.IsActive
                    }).ToList()
            })
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetVariantsResult(new PaginatedResult<VariantDto>(pageIndex, pageSize, count, variants));
    }
}
