

namespace SalesOrder.Orders.Features.AddLine;

public record AddLineCommand(Guid salesOrderId, SalesOrderLineDto  SalesOrderLine):ICommand<AddLineResult>;
public record AddLineResult(Guid Id);
public class AddLineHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, IPriceResolver priceResolver)
    : ICommandHandler<AddLineCommand, AddLineResult>
{
    public async Task<AddLineResult> Handle(AddLineCommand command, CancellationToken cancellationToken)
    {
        var salesOrder = await dbContext.SalesOrders.Include(so => so.Lines).FirstOrDefaultAsync(so => so.Id == command.salesOrderId, cancellationToken);
        if (salesOrder is null)
            throw new NotFoundException($"Sales order not found: {command.salesOrderId}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not uthenticated");

        
        //if (command.SalesOrderLine.Any())
        //{
            //foreach(var line in command.SalesOrder.Lines)
            //{
                var resolvedPrice = await priceResolver.ResolveAsync(
                    new ResolvePriceRequest(
                        salesOrder.CustomerId,
                        command.SalesOrderLine.ProductSkuId,
                        command.SalesOrderLine.UnitOfMeasureId,
                        command.SalesOrderLine.Quantity,
                        salesOrder.CompanyId,
                        salesOrder.PriceListId,
                        command.SalesOrderLine.TaxRate,
                        salesOrder.OrderDate),
                    cancellationToken);

                if (!salesOrder.PriceListId.HasValue && resolvedPrice.PriceListId.HasValue)
                    salesOrder.ApplyResolvedPriceList(resolvedPrice.PriceListId);

                salesOrder.AddLine(
                    command.SalesOrderLine.ProductId,
                    command.SalesOrderLine.ProductSkuId,
                    command.SalesOrderLine.ProductName,
                    command.SalesOrderLine.ProductNameEng,
                    command.SalesOrderLine.SkuCode,
                    command.SalesOrderLine.Quantity,
                    resolvedPrice.UnitPrice,
                    command.SalesOrderLine.UnitOfMeasureId,
                    resolvedPrice.DiscountRate,
                    resolvedPrice.TaxRate,
                    command.SalesOrderLine.Notes,
                    user);
            //}
        //}

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddLineResult(salesOrder.Id);

    }
}
