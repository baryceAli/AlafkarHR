namespace SalesOrder.Orders.Features.UpdateOrder;


public record UpdateOrderCommand(SalesOrderDto SalesOrder) : ICommand<UpdateOrderResult>;
public record UpdateOrderResult(bool IsSuccess);
public class UpdateOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, IPriceResolver priceResolver)
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

        foreach (var line in request.SalesOrder.Lines)
        {
            var resolvedPrice = await priceResolver.ResolveAsync(
                new ResolvePriceRequest(
                    order.CustomerId,
                    line.ProductSkuId,
                    line.UnitOfMeasureId,
                    line.Quantity,
                    order.CompanyId,
                    request.SalesOrder.PriceListId,
                    line.TaxRate,
                    order.OrderDate),
                cancellationToken);

            line.UnitPrice = resolvedPrice.UnitPrice;
            line.DiscountRate = resolvedPrice.DiscountRate;
            line.TaxRate = resolvedPrice.TaxRate;

            if (!request.SalesOrder.PriceListId.HasValue && resolvedPrice.PriceListId.HasValue)
                request.SalesOrder.PriceListId = resolvedPrice.PriceListId;
        }

        order.Update(request.SalesOrder.PriceListId, request.SalesOrder.Lines.Adapt<List<Models.SalesOrderLine>>(), user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateOrderResult(true);
    }
}
