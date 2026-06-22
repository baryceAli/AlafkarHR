using SalesOrder.Orders.Models;
using Shared.Pagination;

namespace SalesOrder.Orders.Features;

internal static class SalesDocumentFeatureHelpers
{
    public static string CurrentUser(IHttpContextAccessor accessor) =>
        accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static SalesPricingSnapshotDto ToPricing(this SalesQuotationLine line) => new()
    {
        PriceSource = line.PriceSource,
        PriceSourceId = line.PriceSourceId,
        SourceUnitPrice = line.SourceUnitPrice,
        PromotionUnitPrice = line.PromotionUnitPrice,
        BulkDiscountRate = line.BulkDiscountRate,
        BulkDiscountAmount = line.BulkDiscountAmount,
        CustomerDiscountRate = line.CustomerDiscountRate,
        CustomerDiscountAmount = line.CustomerDiscountAmount,
        CouponCode = line.CouponCode,
        CouponStatus = line.CouponStatus,
        CouponDiscountType = line.CouponDiscountType,
        CouponDiscountValue = line.CouponDiscountValue,
        CouponDiscountAmount = line.CouponDiscountAmount,
        TaxableAmount = line.TaxableAmount,
        FinalUnitAmount = line.FinalUnitAmount
    };

    public static SalesPricingSnapshotDto ToPricing(this SalesOrderLine line) => new()
    {
        PriceSource = line.PriceSource,
        PriceSourceId = line.PriceSourceId,
        SourceUnitPrice = line.SourceUnitPrice,
        PromotionUnitPrice = line.PromotionUnitPrice,
        BulkDiscountRate = line.BulkDiscountRate,
        BulkDiscountAmount = line.BulkDiscountAmount,
        CustomerDiscountRate = line.CustomerDiscountRate,
        CustomerDiscountAmount = line.CustomerDiscountAmount,
        CouponCode = line.CouponCode,
        CouponStatus = line.CouponStatus,
        CouponDiscountType = line.CouponDiscountType,
        CouponDiscountValue = line.CouponDiscountValue,
        CouponDiscountAmount = line.CouponDiscountAmount,
        TaxableAmount = line.TaxableAmount,
        FinalUnitAmount = line.FinalUnitAmount,
        IsManualPriceOverride = line.IsManualPriceOverride,
        PriceOverrideBy = line.PriceOverrideBy,
        PriceOverrideAt = line.PriceOverrideAt
    };

    public static SalesQuotationDto ToDto(this SalesQuotation quotation) => new()
    {
        Id = quotation.Id,
        Number = quotation.Number,
        CompanyId = quotation.CompanyId,
        CustomerId = quotation.CustomerId,
        CustomerName = quotation.CustomerName,
        PriceListId = quotation.PriceListId,
        CouponCode = quotation.CouponCode,
        SalespersonId = quotation.SalespersonId,
        Status = quotation.Status,
        QuotationDate = quotation.QuotationDate,
        ValidUntil = quotation.ValidUntil,
        Notes = quotation.Notes,
        Terms = quotation.Terms,
        Subtotal = quotation.Subtotal,
        TaxAmount = quotation.TaxAmount,
        TotalAmount = quotation.TotalAmount,
        SalesOrderId = quotation.SalesOrderId,
        SentAt = quotation.SentAt,
        AcceptedAt = quotation.AcceptedAt,
        RejectedAt = quotation.RejectedAt,
        ConvertedAt = quotation.ConvertedAt,
        RejectionReason = quotation.RejectionReason,
        Lines = quotation.Lines.OrderBy(x => x.LineNumber).Select(x => new SalesQuotationLineDto
        {
            Id = x.Id,
            LineNumber = x.LineNumber,
            ProductId = x.ProductId,
            ProductSkuId = x.ProductSkuId,
            ProductName = x.ProductName,
            ProductNameEng = x.ProductNameEng,
            SkuCode = x.SkuCode,
            UnitOfMeasureId = x.UnitOfMeasureId,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            DiscountRate = x.DiscountRate,
            TaxRate = x.TaxRate,
            Notes = x.Notes,
            Pricing = x.ToPricing()
        }).ToList()
    };

    public static SalesDeliveryNoteDto ToDto(this SalesDeliveryNote note) => new()
    {
        Id = note.Id,
        Number = note.Number,
        CompanyId = note.CompanyId,
        CustomerId = note.CustomerId,
        SalesOrderId = note.SalesOrderId,
        SalesOrderNumber = note.SalesOrderNumber,
        WarehouseId = note.WarehouseId,
        DeliveryDate = note.DeliveryDate,
        Status = note.Status,
        Notes = note.Notes,
        PostedAt = note.PostedAt,
        PostedBy = note.PostedBy,
        Lines = note.Lines.OrderBy(x => x.LineNumber).Select(x => new SalesDeliveryNoteLineDto
        {
            Id = x.Id,
            LineNumber = x.LineNumber,
            SalesOrderLineId = x.SalesOrderLineId,
            ProductId = x.ProductId,
            ProductSkuId = x.ProductSkuId,
            ProductName = x.ProductName,
            ProductNameEng = x.ProductNameEng,
            SkuCode = x.SkuCode,
            UnitOfMeasureId = x.UnitOfMeasureId,
            BatchId = x.BatchId,
            CurrencyId = x.CurrencyId,
            Quantity = x.Quantity,
            UnitCost = x.UnitCost,
            TotalCost = x.TotalCost,
            Notes = x.Notes
        }).ToList()
    };

    public static SalesReturnDto ToDto(this SalesReturn salesReturn) => new()
    {
        Id = salesReturn.Id,
        Number = salesReturn.Number,
        CompanyId = salesReturn.CompanyId,
        CustomerId = salesReturn.CustomerId,
        SalesOrderId = salesReturn.SalesOrderId,
        DeliveryNoteId = salesReturn.DeliveryNoteId,
        AccountingDocumentId = salesReturn.AccountingDocumentId,
        WarehouseId = salesReturn.WarehouseId,
        ReturnDate = salesReturn.ReturnDate,
        Status = salesReturn.Status,
        CreateCreditNote = salesReturn.CreateCreditNote,
        Reason = salesReturn.Reason,
        Subtotal = salesReturn.Subtotal,
        TaxAmount = salesReturn.TaxAmount,
        TotalAmount = salesReturn.TotalAmount,
        PostedAt = salesReturn.PostedAt,
        PostedBy = salesReturn.PostedBy,
        Lines = salesReturn.Lines.OrderBy(x => x.LineNumber).Select(x => new SalesReturnLineDto
        {
            Id = x.Id,
            LineNumber = x.LineNumber,
            SalesOrderLineId = x.SalesOrderLineId,
            DeliveryNoteLineId = x.DeliveryNoteLineId,
            ProductId = x.ProductId,
            ProductSkuId = x.ProductSkuId,
            ProductName = x.ProductName,
            ProductNameEng = x.ProductNameEng,
            SkuCode = x.SkuCode,
            UnitOfMeasureId = x.UnitOfMeasureId,
            BatchId = x.BatchId,
            CurrencyId = x.CurrencyId,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            DiscountRate = x.DiscountRate,
            TaxRate = x.TaxRate,
            UnitCost = x.UnitCost,
            TotalCost = x.TotalCost,
            Notes = x.Notes
        }).ToList()
    };

    public static async Task ResolveQuotationPricingAsync(SalesQuotationDto quotation, ISender sender, CancellationToken cancellationToken)
    {
        var orderSubtotal = quotation.Lines.Sum(x => x.Quantity * x.UnitPrice);
        if (orderSubtotal <= 0m)
            orderSubtotal = quotation.Subtotal;

        foreach (var line in quotation.Lines)
        {
            var resolvedPrice = await sender.Send(
                new ResolvePriceQuery(
                    quotation.CustomerId,
                    line.ProductSkuId,
                    line.UnitOfMeasureId,
                    line.Quantity,
                    quotation.CompanyId,
                    quotation.PriceListId,
                    line.TaxRate,
                    quotation.QuotationDate == default ? DateTime.UtcNow : quotation.QuotationDate,
                    quotation.CouponCode,
                    orderSubtotal),
                cancellationToken);

            if (!quotation.PriceListId.HasValue && resolvedPrice.Price.PriceListId.HasValue)
                quotation.PriceListId = resolvedPrice.Price.PriceListId;

            line.UnitPrice = resolvedPrice.Price.UnitPrice;
            line.DiscountRate = resolvedPrice.Price.DiscountRate;
            line.TaxRate = resolvedPrice.Price.TaxRate;
            line.Pricing = new SalesPricingSnapshotDto
            {
                PriceSource = resolvedPrice.Price.PriceSource,
                PriceSourceId = resolvedPrice.Price.SourceId,
                SourceUnitPrice = resolvedPrice.Price.SourceUnitPrice,
                PromotionUnitPrice = resolvedPrice.Price.PromotionUnitPrice,
                BulkDiscountRate = resolvedPrice.Price.BulkDiscountRate,
                BulkDiscountAmount = resolvedPrice.Price.BulkDiscountAmount,
                CustomerDiscountRate = resolvedPrice.Price.CustomerDiscountRate,
                CustomerDiscountAmount = resolvedPrice.Price.CustomerDiscountAmount,
                CouponCode = resolvedPrice.Price.CouponCode,
                CouponStatus = resolvedPrice.Price.CouponStatus,
                CouponDiscountType = resolvedPrice.Price.CouponDiscountType,
                CouponDiscountValue = resolvedPrice.Price.CouponDiscountValue,
                CouponDiscountAmount = resolvedPrice.Price.CouponDiscountAmount,
                TaxableAmount = resolvedPrice.Price.TaxableAmount,
                FinalUnitAmount = resolvedPrice.Price.FinalUnitAmount
            };
        }
    }

    public static PaginatedResult<T> Page<T>(IReadOnlyList<T> items, PaginationRequest request, long count)
        where T : class =>
        new(request.PageIndex, request.PageSize, count, items.ToList());
}
