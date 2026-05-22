using Catalog.Data;
using CustomersModule.Data;

namespace Pricing.Pricings.Services;

public class PriceResolver(
    PricingDbContext pricingDbContext,
    CustomerDbContext customerDbContext,
    CatalogDbContext catalogDbContext) : IPriceResolver
{
    public async Task<ResolvedPriceDto> ResolveAsync(ResolvePriceRequest request, CancellationToken cancellationToken)
    {
        var customer = await customerDbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.CompanyId == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException($"Customer not found: {request.CustomerId}");

        var priceDate = request.PriceDate;
        var customerGroup = customer.CustomerGroupId.HasValue
            ? await customerDbContext.CustomerGroups.AsNoTracking().FirstOrDefaultAsync(g => g.Id == customer.CustomerGroupId.Value, cancellationToken)
            : null;

        var profile = await customerDbContext.CustomerPricingProfiles
            .AsNoTracking()
            .Where(p => p.CustomerId == request.CustomerId &&
                        p.CompanyId == request.CompanyId &&
                        p.EffectiveFrom <= priceDate &&
                        (!p.EffectiveTo.HasValue || p.EffectiveTo.Value >= priceDate))
            .OrderByDescending(p => p.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        var priceListId = request.RequestedPriceListId
            ?? profile?.PriceListId
            ?? customerGroup?.DefaultPriceListId
            ?? await pricingDbContext.PriceLists
                .AsNoTracking()
                .Where(p => p.CompanyId == request.CompanyId &&
                            p.IsDefault &&
                            p.IsActive &&
                            p.EffectiveFrom <= priceDate &&
                            (!p.EffectiveTo.HasValue || p.EffectiveTo.Value >= priceDate))
                .OrderByDescending(p => p.EffectiveFrom)
                .Select(p => (Guid?)p.Id)
                .FirstOrDefaultAsync(cancellationToken);

        var discountRate = profile?.DiscountPercentage ?? customerGroup?.DefaultDiscountPercentage ?? 0m;
        var unitPrice = 0m;
        var source = "Catalog";

        if (priceListId.HasValue)
        {
            var priceList = await pricingDbContext.PriceLists
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == priceListId.Value &&
                                          p.CompanyId == request.CompanyId &&
                                          p.IsActive &&
                                          p.EffectiveFrom <= priceDate &&
                                          (!p.EffectiveTo.HasValue || p.EffectiveTo.Value >= priceDate),
                    cancellationToken);

            if (priceList is not null)
            {
                var item = await pricingDbContext.PriceListItems
                    .AsNoTracking()
                    .Where(i => i.PriceListId == priceList.Id &&
                                i.ProductSkuId == request.ProductSkuId &&
                                i.IsActive &&
                                (!i.UnitId.HasValue || i.UnitId == request.UnitId) &&
                                (!i.MinQuantity.HasValue || i.MinQuantity.Value <= request.Quantity) &&
                                i.EffectiveFrom <= priceDate &&
                                (!i.EffectiveTo.HasValue || i.EffectiveTo.Value >= priceDate))
                    .OrderByDescending(i => i.UnitId.HasValue)
                    .ThenByDescending(i => i.MinQuantity ?? 0m)
                    .ThenByDescending(i => i.EffectiveFrom)
                    .FirstOrDefaultAsync(cancellationToken);

                if (item is not null)
                {
                    unitPrice = item.UnitPrice;
                    source = "PriceList";
                }
            }
        }

        if (source == "Catalog")
        {
            var sku = await catalogDbContext.ProductSkus
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.ProductSkuId && s.CompanyId == request.CompanyId, cancellationToken)
                ?? throw new NotFoundException($"Product SKU not found: {request.ProductSkuId}");

            unitPrice = sku.Price;
            priceListId = null;
        }

        return new ResolvedPriceDto
        {
            ProductSkuId = request.ProductSkuId,
            PriceListId = priceListId,
            UnitPrice = unitPrice,
            DiscountRate = discountRate,
            TaxRate = customer.IsTaxExempt ? 0m : request.RequestedTaxRate,
            PriceSource = source
        };
    }
}
