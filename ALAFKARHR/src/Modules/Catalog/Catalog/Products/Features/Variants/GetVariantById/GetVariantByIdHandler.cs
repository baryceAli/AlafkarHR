using Catalog.Products.Features.Variants.CreateVariant;
using Mapster;

namespace Catalog.Products.Features.Variants.GetVariantById;

public record GetVariantByIdQuery(Guid Id) : IQuery<GetVariantByIdResult>;
public record GetVariantByIdResult(VariantDto Variant);

public class GetVariantByIdHandler (CatalogDbContext dbContext): IQueryHandler<GetVariantByIdQuery, GetVariantByIdResult>
{
    public async Task<GetVariantByIdResult> Handle(GetVariantByIdQuery query, CancellationToken cancellationToken)
    {
        var variant= await dbContext.Variants.AsNoTracking().FirstOrDefaultAsync(x=> x.Id==query.Id && x.DeletedAt==null, cancellationToken);

        if(variant is null)
            throw new Exception($"Variant not found: {query.Id}");

        return new GetVariantByIdResult(variant.Adapt<VariantDto>());
    }
}
