namespace SalesOrder.Orders.Features.UpdateOrder;


public record UpdateOrderCommand(SalesOrderDto SalesOrder) : ICommand<UpdateOrderResult>;
public record UpdateOrderResult(bool IsSuccess);
public class UpdateOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
{
    public async Task<UpdateOrderResult> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == request.SalesOrder.Id, cancellationToken);
        if (order is null)
            throw new NotFoundException($"Order not found: {request.SalesOrder.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var orderSubtotal = request.SalesOrder.Lines.Sum(line => line.Quantity * line.UnitPrice);
        if (orderSubtotal <= 0m)
            orderSubtotal = request.SalesOrder.Subtotal;

        foreach (var line in request.SalesOrder.Lines)
        {
            var resolvedPrice = await sender.Send(
                new ResolvePriceQuery(
                    order.CustomerId,
                    line.ProductSkuId,
                    line.UnitOfMeasureId,
                    line.Quantity,
                    order.CompanyId,
                    request.SalesOrder.PriceListId,
                    line.TaxRate,
                    order.OrderDate,
                    request.SalesOrder.CouponCode,
                    orderSubtotal),
                cancellationToken);

            line.UnitPrice = resolvedPrice.Price.UnitPrice;
            line.DiscountRate = resolvedPrice.Price.DiscountRate;
            line.TaxRate = resolvedPrice.Price.TaxRate;

            if (!request.SalesOrder.PriceListId.HasValue && resolvedPrice.Price.PriceListId.HasValue)
                request.SalesOrder.PriceListId = resolvedPrice.Price.PriceListId;
        }

        order.Update(request.SalesOrder.PriceListId, request.SalesOrder.Lines.Adapt<List<Models.SalesOrderLine>>(), user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateOrderResult(true);
    }
}
