namespace SalesOrder.Orders.Features.CreateOrder;

public record CreateOrderCommand(SalesOrderDto SalesOrder) : ICommand<CreateOrderResult>;
public record CreateOrderResult(Guid Id);
public class CreateOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?
                       .User?
                       .FindFirst(ClaimTypes.NameIdentifier)?
                       .Value ??
                       throw new UnauthorizedAccessException("User is not authenticated");
        await SalesOrderBranchScope.EnsureCanMutateAsync(sender, request.SalesOrder.CompanyId, request.SalesOrder.BranchId, cancellationToken);

        var order=Models.SalesOrder.Create(
            Guid.NewGuid(),
            request.SalesOrder.Number,
            request.SalesOrder.CustomerId,
            request.SalesOrder.PriceListId,
            request.SalesOrder.QuotationTemplateId,
            request.SalesOrder.CompanyId,
            request.SalesOrder.BranchId,
            request.SalesOrder.StoreFrontId,
            request.SalesOrder.PosCashierSessionId,
            user,
            request.SalesOrder.SalespersonId,
            request.SalesOrder.SourceQuotationId,
            request.SalesOrder.InvoicingPolicy,
            request.SalesOrder.SourceType,
            request.SalesOrder.SourceDocumentId,
            request.SalesOrder.SourceDocumentNumber,
            request.SalesOrder.PaymentId,
            request.SalesOrder.DeliveryDate,
            request.SalesOrder.CustomerPurchaseOrderNumber,
            request.SalesOrder.InvoiceAddressId,
            request.SalesOrder.DeliveryAddressId,
            request.SalesOrder.Notes,
            request.SalesOrder.Terms,
            request.SalesOrder.RequiresCustomerSignature,
            request.SalesOrder.RequiresOnlinePayment,
            request.SalesOrder.DownPaymentAmount,
            request.SalesOrder.DownPaymentPercent,
            request.SalesOrder.IsProForma);

        if (request.SalesOrder.Lines.Any())
        {
            var orderSubtotal = request.SalesOrder.Lines.Sum(line => line.Quantity * line.UnitPrice);
            if (orderSubtotal <= 0m)
                orderSubtotal = request.SalesOrder.Subtotal;

            foreach(var line in request.SalesOrder.Lines)
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

                var orderLine = order.AddLine(line.ProductId,
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
        }

        await dbContext.SalesOrders.AddAsync(order,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id);
    }
}
