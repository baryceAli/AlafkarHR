using Catalog.Contracts.Products.Features.GetProductSkuPricingContext;
using Customers.Contracts.Customers.Features.GetCustomerPricingContext;
using Pricing.Pricings.Features.PricingResolution.GetDefaultPriceList;
using Pricing.Pricings.Features.PricingResolution.GetResolvedPriceListItem;

namespace Pricing.Pricings.Services;

public class PriceResolver(ISender sender) : IPriceResolver
{
    public async Task<ResolvedPriceDto> ResolveAsync(ResolvePriceRequest request, CancellationToken cancellationToken)
    {
        var priceDate = request.PriceDate;
        var customerPricingContext = await sender.Send(
            new GetCustomerPricingContextQuery(request.CustomerId, request.CompanyId, priceDate),
            cancellationToken);

        var priceListId = request.RequestedPriceListId
            ?? customerPricingContext.ProfilePriceListId
            ?? customerPricingContext.GroupDefaultPriceListId;

        if (!priceListId.HasValue)
        {
            var defaultPriceList = await sender.Send(
                new GetDefaultPriceListQuery(request.CompanyId, priceDate),
                cancellationToken);

            priceListId = defaultPriceList.PriceListId;
        }

        var discountRate = customerPricingContext.ProfileDiscountPercentage
            ?? customerPricingContext.GroupDefaultDiscountPercentage
            ?? 0m;
        var unitPrice = 0m;
        var source = "Catalog";

        if (priceListId.HasValue)
        {
            var resolvedPriceListItem = await sender.Send(
                new GetResolvedPriceListItemQuery(
                    priceListId.Value,
                    request.CompanyId,
                    request.ProductSkuId,
                    request.UnitId,
                    request.Quantity,
                    priceDate),
                cancellationToken);

            if (resolvedPriceListItem.UnitPrice.HasValue)
            {
                unitPrice = resolvedPriceListItem.UnitPrice.Value;
                priceListId = resolvedPriceListItem.PriceListId;
                source = "PriceList";
            }
        }

        if (source == "Catalog")
        {
            var sku = await sender.Send(
                new GetProductSkuPricingContextQuery(request.ProductSkuId, request.CompanyId),
                cancellationToken);

            unitPrice = sku.BasePrice;
            priceListId = null;
        }

        return new ResolvedPriceDto
        {
            ProductSkuId = request.ProductSkuId,
            PriceListId = priceListId,
            UnitPrice = unitPrice,
            DiscountRate = discountRate,
            TaxRate = customerPricingContext.IsTaxExempt ? 0m : request.RequestedTaxRate,
            PriceSource = source
        };
    }
}
