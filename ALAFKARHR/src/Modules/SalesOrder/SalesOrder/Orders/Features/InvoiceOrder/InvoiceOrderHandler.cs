using Microsoft.AspNetCore.Http.HttpResults;

namespace SalesOrder.Orders.Features.InvoiceOrder;

public record InvoiceOrderCommand(SalesOrderDto SalesOrder) : ICommand<InvoiceOrderResult>;
public record InvoiceOrderResult(bool IsSuccess);
public class InvoiceOrderHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<InvoiceOrderCommand, InvoiceOrderResult>
{
    public async Task<InvoiceOrderResult> Handle(InvoiceOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.SalesOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == request.SalesOrder.Id, cancellationToken);
        if (order is null)
            throw new NotFoundException($"Order not found: {request.SalesOrder.Id}");
        await SalesOrderBranchScope.EnsureCanMutateAsync(sender, order.CompanyId, order.BranchId, cancellationToken);

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
            Lines = order.Lines
                .Where(x => linesToInvoice.ContainsKey(x.Id))
                .Select(x =>
                {
                    var quantity = linesToInvoice[x.Id];
                    var gross = quantity * x.UnitPrice;
                    var discount = gross * x.DiscountRate / 100m;
                    var net = gross - discount;
                    var tax = net * x.TaxRate / 100m;

                    return new AccountingDocumentLineDto
                    {
                        Description = string.IsNullOrWhiteSpace(x.ProductNameEng) ? x.ProductName : x.ProductNameEng,
                        ProductId = x.ProductId,
                        ProductSkuId = x.ProductSkuId,
                        Quantity = quantity,
                        UnitPrice = x.UnitPrice,
                        DiscountAmount = discount,
                        TaxRate = x.TaxRate,
                        NetAmount = net,
                        TaxAmount = tax,
                        TotalAmount = net + tax
                    };
                })
                .ToList()
        };

        var created = await sender.Send(new CreateAccountingDocumentCommand(accountingDocument), cancellationToken);
        await sender.Send(new PostAccountingDocumentCommand(created.Id), cancellationToken);
        await sender.Send(new GenerateZatcaInvoiceCommand(created.Id, ZatcaInvoiceType.StandardTaxInvoice), cancellationToken);
        return new InvoiceOrderResult(true);
    }
}
