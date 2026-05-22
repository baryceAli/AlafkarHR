using Microsoft.AspNetCore.Http.HttpResults;

namespace SalesOrder.Orders.Features.InvoiceOrder;

public record InvoiceOrderCommand(SalesOrderDto SalesOrder) : ICommand<InvoiceOrderResult>;
public record InvoiceOrderResult(bool IsSuccess);
public class InvoiceOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<InvoiceOrderCommand, InvoiceOrderResult>
{
    public async Task<InvoiceOrderResult> Handle(InvoiceOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == request.SalesOrder.Id, cancellationToken);
        if (order is null)
            throw new NotFoundException($"Order not found: {request.SalesOrder.Id}");

        var user = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        if (!request.SalesOrder.Lines.Any())
            throw new Exception("No lines provided");

        var linesToInvoice = request.SalesOrder.Lines.ToDictionary(o => o.Id, o => o.InvoicedQuantity);
        order.Invoice(linesToInvoice);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new InvoiceOrderResult(true);
    }
}
