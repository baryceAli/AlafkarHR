

namespace SalesOrder.Orders.Features.AddLine;

public record AddLineCommand(Guid salesOrderId, SalesOrderLineDto  SalesOrderLine):ICommand<AddLineResult>;
public record AddLineResult(Guid Id);
public class AddLineHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
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
                salesOrder.AddLine(
                    command.SalesOrderLine.ProductId,
                    command.SalesOrderLine.ProductSkuId,
                    command.SalesOrderLine.ProductName,
                    command.SalesOrderLine.ProductNameEng,
                    command.SalesOrderLine.SkuCode,
                    command.SalesOrderLine.Quantity,
                    command.SalesOrderLine.UnitPrice,
                    command.SalesOrderLine.UnitOfMeasureId,
                    command.SalesOrderLine.DiscountRate,
                    command.SalesOrderLine.TaxRate,
                    command.SalesOrderLine.Notes,
                    user);
            //}
        //}

        await dbContext.SalesOrders.AddAsync(salesOrder,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddLineResult(salesOrder.Id);

    }
}
