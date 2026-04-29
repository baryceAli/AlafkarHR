namespace Catalog.Products.Features.Variants.GetVariantById;

public record GetVariantByIdQuery(Guid Id) : IQuery<GetVariantByIdResult>;
public record GetVariantByIdResult(VariantDto Variant);

public class GetVariantByIdHandler(CatalogDbContext dbContext) : IQueryHandler<GetVariantByIdQuery, GetVariantByIdResult>
{
    public async Task<GetVariantByIdResult> Handle(GetVariantByIdQuery query, CancellationToken cancellationToken)
    {
        var variant = await dbContext.Variants
                    .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == query.Id, cancellationToken);
        
        if (variant is null)
            throw new Exception($"Variant not found: {query.Id}");
        
        var variantDto = new VariantDto
        {
            Id = variant.Id,
            Name = variant.Name,
            NameEng = variant.NameEng,
            CompanyId = variant.CompanyId,
            Values = variant.Values
            .Where(v => !v.IsDeleted)
            .Select(v => new VariantValueDto
            {
                Id = v.Id,
                VariantId = v.VariantId,
                Value = v.Value,
                ValueEng = v.ValueEng,
            }).ToList()
        };
   
        return new GetVariantByIdResult(variantDto);

    }
}
