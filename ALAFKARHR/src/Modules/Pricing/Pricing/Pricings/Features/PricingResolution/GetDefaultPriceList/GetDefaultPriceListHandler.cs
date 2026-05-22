namespace Pricing.Pricings.Features.PricingResolution.GetDefaultPriceList;

public record GetDefaultPriceListQuery(Guid CompanyId, DateTime PriceDate) : IQuery<GetDefaultPriceListResult>;
public record GetDefaultPriceListResult(Guid? PriceListId);

public class GetDefaultPriceListHandler(PricingDbContext dbContext)
    : IQueryHandler<GetDefaultPriceListQuery, GetDefaultPriceListResult>
{
    public async Task<GetDefaultPriceListResult> Handle(GetDefaultPriceListQuery request, CancellationToken cancellationToken)
    {
        var priceListId = await dbContext.PriceLists
            .AsNoTracking()
            .Where(p => p.CompanyId == request.CompanyId &&
                        p.IsDefault &&
                        p.IsActive &&
                        p.EffectiveFrom <= request.PriceDate &&
                        (!p.EffectiveTo.HasValue || p.EffectiveTo.Value >= request.PriceDate))
            .OrderByDescending(p => p.EffectiveFrom)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetDefaultPriceListResult(priceListId);
    }
}
