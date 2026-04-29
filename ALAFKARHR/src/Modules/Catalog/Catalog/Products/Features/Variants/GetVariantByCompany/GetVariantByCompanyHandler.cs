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
        var count = await dbContext.Variants.LongCountAsync(x => !x.IsDeleted && x.CompanyId == request.CompanyId, cancellationToken);



        var variants = await dbContext.Variants
            .Where(x => !x.IsDeleted && x.CompanyId == request.CompanyId)
            .Select(x => new VariantDto
            {
                Id = x.Id,
                Name = x.Name,
                NameEng=x.NameEng,
                CompanyId = x.CompanyId,
                
                Values = x.Values
                    .Where(v => !v.IsDeleted)
                    .Select(v => new VariantValueDto
                    {
                        Id = v.Id,
                        Value = v.Value,
                        ValueEng = v.ValueEng,
                        VariantId=v.VariantId,
                        
                    }).ToList()
            })
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetVariantByCompanyResult(new PaginatedResult<VariantDto>(pageIndex, pageSize, count, variants));

    }
}
