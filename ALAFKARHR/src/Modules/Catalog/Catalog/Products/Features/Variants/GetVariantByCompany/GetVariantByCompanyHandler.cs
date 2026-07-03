using Catalog.Products.Features.Variants.GetVariants;

namespace Catalog.Products.Features.Variants.GetVariantByCompany;


public record GetVariantByCompanyQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetVariantByCompanyResult>;
public record GetVariantByCompanyResult(PaginatedResult<VariantDto> VariantList);
public class GetVariantByCompanyHandler(CatalogDbContext dbContext)
    : IQueryHandler<GetVariantByCompanyQuery, GetVariantByCompanyResult>
{
    public async Task<GetVariantByCompanyResult> Handle(GetVariantByCompanyQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PaginationRequest.PageIndex;
        var pageSize = request.PaginationRequest.PageSize;
        var query = dbContext.Variants
            .Where(x => !x.IsDeleted && x.CompanyId == request.CompanyId);

        if (!request.PaginationRequest.IncludeInactive)
            query = query.Where(x => x.IsActive);

        var count = await query.LongCountAsync(cancellationToken);

        var variants = await query
            .Select(x => new VariantDto
            {
                Id = x.Id,
                Name = x.Name,
                NameEng=x.NameEng,
                DisplayType = x.DisplayType,
                CreationMode = x.CreationMode,
                IsActive = x.IsActive,
                CompanyId = x.CompanyId,
                
                Values = x.Values
                    .Where(v => !v.IsDeleted && (request.PaginationRequest.IncludeInactive || v.IsActive))
                    .Select(v => new VariantValueDto
                    {
                        Id = v.Id,
                        Value = v.Value,
                        ValueEng = v.ValueEng,
                        VariantId=v.VariantId,
                        IsActive = v.IsActive,
                        
                    }).ToList()
            })
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetVariantByCompanyResult(new PaginatedResult<VariantDto>(pageIndex, pageSize, count, variants));

    }
}
