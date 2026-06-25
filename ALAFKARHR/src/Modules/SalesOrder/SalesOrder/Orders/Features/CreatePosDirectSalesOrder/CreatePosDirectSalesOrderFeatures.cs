namespace SalesOrder.Orders.Features.CreatePosDirectSalesOrder;

public record CreatePosDirectSalesOrderCommand(CreatePosDirectSalesOrderDto SalesOrder) : ICommand<CreatePosDirectSalesOrderResult>;
public record CreatePosDirectSalesOrderResult(Guid Id, string Number, Guid? AccountingDocumentId, Guid? ZatcaEInvoiceId);

public class CreatePosDirectSalesOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreatePosDirectSalesOrderCommand, CreatePosDirectSalesOrderResult>
{
    public async Task<CreatePosDirectSalesOrderResult> Handle(CreatePosDirectSalesOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.SalesOrder.CustomerId == Guid.Empty)
            throw new Exception("Customer is required.");

        if (request.SalesOrder.CompanyId == Guid.Empty)
            throw new Exception("Company is required.");

        if (request.SalesOrder.Lines.Count == 0)
            throw new Exception("Sales order must include at least one line.");
        await SalesOrderBranchScope.EnsureCanMutateAsync(sender, request.SalesOrder.CompanyId, request.SalesOrder.BranchId, cancellationToken);

        var user = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "checkout";
        var order = Models.SalesOrder.Create(
            Guid.NewGuid(),
            string.IsNullOrWhiteSpace(request.SalesOrder.Number) ? $"POS-{DateTime.UtcNow:yyyyMMddHHmmss}" : request.SalesOrder.Number,
            request.SalesOrder.CustomerId,
            request.SalesOrder.PriceListId,
            request.SalesOrder.CompanyId,
            request.SalesOrder.BranchId,
            request.SalesOrder.StoreFrontId,
            request.SalesOrder.PosCashierSessionId,
            user,
            request.SalesOrder.SalespersonId ?? user,
            request.SalesOrder.SourceQuotationId,
            SalesInvoicingPolicy.InvoiceDeliveredQuantity,
            SalesOrderSourceType.POSDirectSale,
            request.SalesOrder.SourceDocumentId,
            request.SalesOrder.SourceDocumentNumber,
            request.SalesOrder.PaymentId,
            request.SalesOrder.DeliveryDate ?? DateTime.UtcNow,
            request.SalesOrder.CustomerPurchaseOrderNumber,
            request.SalesOrder.Notes,
            request.SalesOrder.Terms);

        var orderSubtotal = request.SalesOrder.Lines.Sum(line => line.Quantity * line.UnitPrice);
        if (orderSubtotal <= 0m)
            orderSubtotal = request.SalesOrder.Subtotal;

        foreach (var line in request.SalesOrder.Lines)
        {
            var resolvedPrice = await sender.Send(
                new ResolvePriceQuery(
                    request.SalesOrder.CustomerId,
                    line.ProductSkuId,
                    line.UnitOfMeasureId,
                    line.Quantity,
                    request.SalesOrder.CompanyId,
                    request.SalesOrder.PriceListId,
                    line.TaxRate,
                    order.OrderDate,
                    request.SalesOrder.CouponCode,
                    orderSubtotal),
                cancellationToken);

            if (!request.SalesOrder.PriceListId.HasValue && resolvedPrice.Price.PriceListId.HasValue)
                order.ApplyResolvedPriceList(resolvedPrice.Price.PriceListId);

            var orderLine = order.AddLine(
                line.ProductId,
                line.ProductSkuId,
                line.ProductName,
                line.ProductNameEng,
                line.SkuCode,
                line.Quantity,
                resolvedPrice.Price.UnitPrice,
                line.UnitOfMeasureId,
                resolvedPrice.Price.DiscountRate,
                resolvedPrice.Price.TaxRate,
                line.Notes,
                user);

            orderLine.ApplyPricingSnapshot(
                resolvedPrice.Price.PriceSource,
                resolvedPrice.Price.SourceId,
                resolvedPrice.Price.SourceUnitPrice,
                resolvedPrice.Price.PromotionUnitPrice,
                resolvedPrice.Price.BulkDiscountRate,
                resolvedPrice.Price.BulkDiscountAmount,
                resolvedPrice.Price.CustomerDiscountRate,
                resolvedPrice.Price.CustomerDiscountAmount,
                resolvedPrice.Price.CouponCode,
                resolvedPrice.Price.CouponStatus,
                resolvedPrice.Price.CouponDiscountType,
                resolvedPrice.Price.CouponDiscountValue,
                resolvedPrice.Price.CouponDiscountAmount,
                resolvedPrice.Price.TaxableAmount,
                resolvedPrice.Price.FinalUnitAmount);
        }

        order.Confirm();
        order.ConfirmedBy = user;

        var allLineQuantities = order.Lines.ToDictionary(x => x.Id, x => x.Quantity);
        order.Deliver(allLineQuantities);
        order.Invoice(allLineQuantities);
        order.Complete();

        await dbContext.SalesOrders.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var accountingDocument = new AccountingDocumentDto
        {
            CompanyId = order.CompanyId,
            BranchId = order.BranchId,
            Type = AccountingDocumentType.SalesInvoice,
            DocumentDate = DateTime.UtcNow,
            PartyId = order.CustomerId,
            SourceModule = "SalesOrder",
            SourceDocumentId = order.Id,
            SourceDocumentNumber = order.Number,
            Lines = order.Lines.Select(x =>
            {
                var gross = x.Quantity * x.UnitPrice;
                var discount = gross * x.DiscountRate / 100m;
                var net = gross - discount;
                var tax = net * x.TaxRate / 100m;

                return new AccountingDocumentLineDto
                {
                    Description = string.IsNullOrWhiteSpace(x.ProductNameEng) ? x.ProductName : x.ProductNameEng,
                    ProductId = x.ProductId,
                    ProductSkuId = x.ProductSkuId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    DiscountAmount = discount,
                    TaxRate = x.TaxRate,
                    NetAmount = net,
                    TaxAmount = tax,
                    TotalAmount = net + tax
                };
            }).ToList()
        };

        var createdDocument = await sender.Send(new CreateAccountingDocumentCommand(accountingDocument), cancellationToken);
        await sender.Send(new PostAccountingDocumentCommand(createdDocument.Id), cancellationToken);
        var zatca = await sender.Send(new GenerateZatcaInvoiceCommand(createdDocument.Id, ZatcaInvoiceType.StandardTaxInvoice), cancellationToken);
        await ConsumeStoreFrontInventoryAsync(order, cancellationToken);

        order.LinkAccounting(createdDocument.Id, zatca.EInvoiceId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatePosDirectSalesOrderResult(order.Id, order.Number, createdDocument.Id, zatca.EInvoiceId);
    }

    private async Task ConsumeStoreFrontInventoryAsync(Models.SalesOrder order, CancellationToken cancellationToken)
    {
        if (!order.StoreFrontId.HasValue || !order.BranchId.HasValue)
            return;

        var scope = await sender.Send(new GetStoreFrontBranchScopeQuery(order.StoreFrontId.Value), cancellationToken);
        if (scope.CompanyId != order.CompanyId || scope.BranchId != order.BranchId.Value)
            throw new BadRequestException("StoreFront branch scope does not match the POS sale.");

        foreach (var line in order.Lines)
        {
            var sku = await sender.Send(new GetProductSkuByIdQuery(line.ProductSkuId), cancellationToken);
            if (sku.ProductSku.CompanyId != order.CompanyId)
                throw new BadRequestException("POS sale SKU does not belong to the order company.");
            if (!sku.ProductSku.IsInventoryTracked)
                continue;

            await sender.Send(new PostInventoryStockOutBySkuCommand(
                line.ProductId,
                line.ProductSkuId,
                null,
                scope.DefaultWarehouseId,
                line.Quantity,
                0m,
                0m,
                null,
                order.CompanyId,
                $"POS sale {order.Number}",
                order.Number,
                "POSDirectSale"), cancellationToken);
        }
    }
}
