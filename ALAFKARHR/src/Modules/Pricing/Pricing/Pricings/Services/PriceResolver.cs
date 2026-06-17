using Catalog.Contracts.Products.Features.GetProductSkuPricingContext;
using Customers.Contracts.Customers.Features.GetCustomerPricingContext;
using Pricing.Pricings.Features.PricingResolution.GetDefaultPriceList;
using Pricing.Pricings.Features.PricingResolution.GetResolvedPriceListItem;

namespace Pricing.Pricings.Services;

public class PriceResolver(ISender sender, PricingDbContext dbContext) : IPriceResolver
{
    public async Task<ResolvedPriceDto> ResolveAsync(ResolvePriceRequest request, CancellationToken cancellationToken)
    {
        var priceDate = request.PriceDate;
        var customerPricingContext = await sender.Send(
            new GetCustomerPricingContextQuery(request.CustomerId, request.CompanyId, priceDate),
            cancellationToken);

        var source = await ResolveSourcePriceAsync(request, customerPricingContext, cancellationToken);

        if (source is null)
        {
            var sku = await sender.Send(
                new GetProductSkuPricingContextQuery(request.ProductSkuId, request.CompanyId),
                cancellationToken);

            source = new ResolvedSourcePrice(
                "Catalog",
                null,
                null,
                sku.BasePrice,
                sku.BasePrice,
                null);
        }

        var customerDiscountRate = customerPricingContext.ProfileDiscountPercentage
            ?? customerPricingContext.GroupDefaultDiscountPercentage
            ?? 0m;

        var quantity = request.Quantity;
        var sourceLineAmount = source.SourceUnitPrice * quantity;
        var runningLineAmount = source.SelectedUnitPrice * quantity;
        var bulkDiscountAmount = Math.Max(sourceLineAmount - runningLineAmount, 0m);
        var bulkDiscountRate = sourceLineAmount == 0m ? 0m : bulkDiscountAmount / sourceLineAmount * 100m;

        var customerDiscountAmount = runningLineAmount * customerDiscountRate / 100m;
        runningLineAmount = Math.Max(runningLineAmount - customerDiscountAmount, 0m);

        var couponResult = await ResolveCouponAsync(
            request,
            customerPricingContext.CustomerGroupId,
            request.OrderSubtotal ?? runningLineAmount,
            runningLineAmount,
            cancellationToken);

        runningLineAmount = Math.Max(runningLineAmount - couponResult.DiscountAmount, 0m);

        var taxRate = customerPricingContext.IsTaxExempt ? 0m : request.RequestedTaxRate;
        var taxAmount = runningLineAmount * taxRate / 100m;
        var lineTotal = runningLineAmount + taxAmount;
        var effectiveDiscountAmount = Math.Max(sourceLineAmount - runningLineAmount, 0m);
        var effectiveDiscountRate = sourceLineAmount == 0m ? 0m : effectiveDiscountAmount / sourceLineAmount * 100m;

        return new ResolvedPriceDto
        {
            ProductSkuId = request.ProductSkuId,
            PriceListId = source.PriceListId,
            UnitPrice = source.SourceUnitPrice,
            DiscountRate = Math.Min(effectiveDiscountRate, 100m),
            TaxRate = taxRate,
            PriceSource = source.Source,
            SourceId = source.SourceId,
            SourceUnitPrice = source.SourceUnitPrice,
            PromotionUnitPrice = source.PromotionUnitPrice,
            BulkDiscountRate = bulkDiscountRate,
            BulkDiscountAmount = bulkDiscountAmount,
            CustomerDiscountRate = customerDiscountRate,
            CustomerDiscountAmount = customerDiscountAmount,
            CouponCode = couponResult.Code,
            CouponStatus = couponResult.Status,
            CouponDiscountType = couponResult.DiscountType,
            CouponDiscountValue = couponResult.DiscountValue,
            CouponDiscountAmount = couponResult.DiscountAmount,
            TaxableAmount = runningLineAmount,
            TaxAmount = taxAmount,
            FinalUnitAmount = quantity == 0m ? 0m : runningLineAmount / quantity,
            LineSubtotal = sourceLineAmount,
            LineTotal = lineTotal
        };
    }

    private async Task<ResolvedSourcePrice?> ResolveSourcePriceAsync(
        ResolvePriceRequest request,
        GetCustomerPricingContextResult customerPricingContext,
        CancellationToken cancellationToken)
    {
        var customerPriceListIds = new[] { request.RequestedPriceListId, customerPricingContext.ProfilePriceListId }
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        foreach (var priceListId in customerPriceListIds)
        {
            var price = await ResolvePriceListSourceAsync(
                priceListId,
                "CustomerPriceList",
                request,
                cancellationToken);

            if (price is not null)
                return price;
        }

        var contractPrice = await ResolveContractSourceAsync(request, cancellationToken);
        if (contractPrice is not null)
            return contractPrice;

        var groupPriceListId = customerPricingContext.GroupDefaultPriceListId;
        if (groupPriceListId.HasValue && !customerPriceListIds.Contains(groupPriceListId.Value))
        {
            var groupPrice = await ResolvePriceListSourceAsync(
                groupPriceListId.Value,
                "GroupPriceList",
                request,
                cancellationToken);

            if (groupPrice is not null)
                return groupPrice;
        }

        var defaultPriceList = await sender.Send(
            new GetDefaultPriceListQuery(request.CompanyId, request.PriceDate),
            cancellationToken);

        if (defaultPriceList.PriceListId.HasValue &&
            !customerPriceListIds.Contains(defaultPriceList.PriceListId.Value) &&
            defaultPriceList.PriceListId != groupPriceListId)
        {
            var defaultPrice = await ResolvePriceListSourceAsync(
                defaultPriceList.PriceListId.Value,
                "DefaultPriceList",
                request,
                cancellationToken);

            if (defaultPrice is not null)
                return defaultPrice;
        }

        return await ResolvePromotionSourceAsync(request, cancellationToken);
    }

    private async Task<ResolvedSourcePrice?> ResolvePriceListSourceAsync(
        Guid priceListId,
        string source,
        ResolvePriceRequest request,
        CancellationToken cancellationToken)
    {
        var resolvedPriceListItem = await sender.Send(
            new GetResolvedPriceListItemQuery(
                priceListId,
                request.CompanyId,
                request.ProductSkuId,
                request.UnitId,
                request.Quantity,
                request.PriceDate),
            cancellationToken);

        if (!resolvedPriceListItem.UnitPrice.HasValue)
            return null;

        var sourceUnitPrice = await GetPriceListBaseUnitPriceAsync(priceListId, request, cancellationToken)
            ?? resolvedPriceListItem.UnitPrice.Value;

        return new ResolvedSourcePrice(
            source,
            priceListId,
            priceListId,
            sourceUnitPrice,
            resolvedPriceListItem.UnitPrice.Value,
            null);
    }

    private async Task<decimal?> GetPriceListBaseUnitPriceAsync(
        Guid priceListId,
        ResolvePriceRequest request,
        CancellationToken cancellationToken)
    {
        return await dbContext.PriceListItems
            .AsNoTracking()
            .Where(i => i.PriceListId == priceListId &&
                        i.ProductSkuId == request.ProductSkuId &&
                        (!i.UnitId.HasValue || i.UnitId == request.UnitId) &&
                        (!i.MinQuantity.HasValue || i.MinQuantity.Value <= 1m))
            .OrderByDescending(i => i.UnitId.HasValue)
            .ThenByDescending(i => i.MinQuantity ?? 0m)
            .Select(i => (decimal?)i.UnitPrice)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<ResolvedSourcePrice?> ResolveContractSourceAsync(
        ResolvePriceRequest request,
        CancellationToken cancellationToken)
    {
        var contract = await dbContext.CustomerSalesContracts
            .AsNoTracking()
            .Where(c => c.CustomerId == request.CustomerId &&
                        c.CompanyId == request.CompanyId &&
                        c.IsActive &&
                        c.EffectiveFrom <= request.PriceDate &&
                        (!c.EffectiveTo.HasValue || c.EffectiveTo.Value >= request.PriceDate))
            .OrderByDescending(c => c.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (contract is null)
            return null;

        var selectedItem = await dbContext.CustomerSalesContractItems
            .AsNoTracking()
            .Where(i => i.CustomerSalesContractId == contract.Id &&
                        i.ProductSkuId == request.ProductSkuId &&
                        (!i.UnitId.HasValue || i.UnitId == request.UnitId) &&
                        (!i.MinQuantity.HasValue || i.MinQuantity.Value <= request.Quantity))
            .OrderByDescending(i => i.UnitId.HasValue)
            .ThenByDescending(i => i.MinQuantity ?? 0m)
            .FirstOrDefaultAsync(cancellationToken);

        if (selectedItem is null)
            return null;

        var sourceUnitPrice = await dbContext.CustomerSalesContractItems
            .AsNoTracking()
            .Where(i => i.CustomerSalesContractId == contract.Id &&
                        i.ProductSkuId == request.ProductSkuId &&
                        (!i.UnitId.HasValue || i.UnitId == request.UnitId) &&
                        (!i.MinQuantity.HasValue || i.MinQuantity.Value <= 1m))
            .OrderByDescending(i => i.UnitId.HasValue)
            .ThenByDescending(i => i.MinQuantity ?? 0m)
            .Select(i => (decimal?)i.UnitPrice)
            .FirstOrDefaultAsync(cancellationToken)
            ?? selectedItem.UnitPrice;

        return new ResolvedSourcePrice("Contract", null, contract.Id, sourceUnitPrice, selectedItem.UnitPrice, null);
    }

    private async Task<ResolvedSourcePrice?> ResolvePromotionSourceAsync(
        ResolvePriceRequest request,
        CancellationToken cancellationToken)
    {
        var promotion = await dbContext.PromotionPrices
            .AsNoTracking()
            .Where(p => p.CompanyId == request.CompanyId &&
                        p.IsActive &&
                        p.EffectiveFrom <= request.PriceDate &&
                        (!p.EffectiveTo.HasValue || p.EffectiveTo.Value >= request.PriceDate))
            .OrderByDescending(p => p.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (promotion is null)
            return null;

        var selectedItem = await dbContext.PromotionPriceItems
            .AsNoTracking()
            .Where(i => i.PromotionPriceId == promotion.Id &&
                        i.ProductSkuId == request.ProductSkuId &&
                        (!i.UnitId.HasValue || i.UnitId == request.UnitId) &&
                        (!i.MinQuantity.HasValue || i.MinQuantity.Value <= request.Quantity))
            .OrderByDescending(i => i.UnitId.HasValue)
            .ThenByDescending(i => i.MinQuantity ?? 0m)
            .FirstOrDefaultAsync(cancellationToken);

        if (selectedItem is null)
            return null;

        var sourceUnitPrice = await dbContext.PromotionPriceItems
            .AsNoTracking()
            .Where(i => i.PromotionPriceId == promotion.Id &&
                        i.ProductSkuId == request.ProductSkuId &&
                        (!i.UnitId.HasValue || i.UnitId == request.UnitId) &&
                        (!i.MinQuantity.HasValue || i.MinQuantity.Value <= 1m))
            .OrderByDescending(i => i.UnitId.HasValue)
            .ThenByDescending(i => i.MinQuantity ?? 0m)
            .Select(i => (decimal?)i.UnitPrice)
            .FirstOrDefaultAsync(cancellationToken)
            ?? selectedItem.UnitPrice;

        return new ResolvedSourcePrice("Promotion", null, promotion.Id, sourceUnitPrice, selectedItem.UnitPrice, selectedItem.UnitPrice);
    }

    private async Task<CouponResolution> ResolveCouponAsync(
        ResolvePriceRequest request,
        Guid? customerGroupId,
        decimal orderSubtotal,
        decimal eligibleLineAmount,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CouponCode))
            return new CouponResolution(null, "NotProvided", null, null, 0m);

        var normalizedCode = request.CouponCode.Trim().ToUpperInvariant();
        var coupon = await dbContext.DiscountCoupons
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId && c.Code == normalizedCode, cancellationToken);

        if (coupon is null)
            return new CouponResolution(normalizedCode, "Invalid", null, null, 0m);

        if (!coupon.IsActive ||
            coupon.EffectiveFrom > request.PriceDate ||
            (coupon.EffectiveTo.HasValue && coupon.EffectiveTo.Value < request.PriceDate))
            return new CouponResolution(normalizedCode, "ExpiredOrInactive", coupon.DiscountType.ToString(), coupon.DiscountValue, 0m);

        if (coupon.CustomerId.HasValue && coupon.CustomerId.Value != request.CustomerId)
            return new CouponResolution(normalizedCode, "CustomerNotEligible", coupon.DiscountType.ToString(), coupon.DiscountValue, 0m);

        if (coupon.CustomerGroupId.HasValue && coupon.CustomerGroupId != customerGroupId)
            return new CouponResolution(normalizedCode, "CustomerGroupNotEligible", coupon.DiscountType.ToString(), coupon.DiscountValue, 0m);

        if (coupon.ProductSkuId.HasValue && coupon.ProductSkuId.Value != request.ProductSkuId)
            return new CouponResolution(normalizedCode, "ProductNotEligible", coupon.DiscountType.ToString(), coupon.DiscountValue, 0m);

        if (coupon.MinimumOrderAmount.HasValue && orderSubtotal < coupon.MinimumOrderAmount.Value)
            return new CouponResolution(normalizedCode, "MinimumOrderAmountNotReached", coupon.DiscountType.ToString(), coupon.DiscountValue, 0m);

        var discountAmount = coupon.DiscountType == CouponDiscountType.Percentage
            ? eligibleLineAmount * coupon.DiscountValue / 100m
            : GetAllocatedFixedCouponAmount(coupon.DiscountValue, orderSubtotal, eligibleLineAmount);

        discountAmount = Math.Min(Math.Max(discountAmount, 0m), eligibleLineAmount);

        return new CouponResolution(normalizedCode, "Applied", coupon.DiscountType.ToString(), coupon.DiscountValue, discountAmount);
    }

    private sealed record ResolvedSourcePrice(
        string Source,
        Guid? PriceListId,
        Guid? SourceId,
        decimal SourceUnitPrice,
        decimal SelectedUnitPrice,
        decimal? PromotionUnitPrice);

    private sealed record CouponResolution(
        string? Code,
        string Status,
        string? DiscountType,
        decimal? DiscountValue,
        decimal DiscountAmount);

    private static decimal GetAllocatedFixedCouponAmount(decimal couponValue, decimal orderSubtotal, decimal eligibleLineAmount)
    {
        if (orderSubtotal <= 0m || orderSubtotal <= eligibleLineAmount)
            return couponValue;

        return couponValue * (eligibleLineAmount / orderSubtotal);
    }
}
