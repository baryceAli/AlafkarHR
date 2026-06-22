using Customers.Contracts.Customers.Features.GetCustomerSalesEligibility;
using Sales.Contracts.Sales.Features.CreatePosDirectSale;
using SalesOrder.Orders.Features.CreatePosDirectSalesOrder;

namespace Sales.Sales.Features.CreatePosDirectSale;

public class CreatePosDirectSaleHandler(ISender sender)
    : ICommandHandler<CreatePosDirectSaleCommand, CreatePosDirectSaleResult>
{
    public async Task<CreatePosDirectSaleResult> Handle(CreatePosDirectSaleCommand request, CancellationToken cancellationToken)
    {
        if (!request.Cart.CustomerId.HasValue)
            throw new Exception("Customer is required for POS direct sale.");

        var eligibility = await sender.Send(
            new GetCustomerSalesEligibilityQuery(request.Cart.CustomerId.Value, request.Cart.CompanyId, request.Payment.Amount),
            cancellationToken);

        if (!eligibility.Exists || !eligibility.IsActive)
            throw new Exception(eligibility.BlockReason ?? "Customer is not eligible for POS direct sale.");

        var order = new CreatePosDirectSalesOrderDto
        {
            Number = $"POS-{DateTime.UtcNow:yyyyMMddHHmmss}",
            CustomerId = request.Cart.CustomerId.Value,
            CompanyId = request.Cart.CompanyId,
            PriceListId = request.Cart.PriceListId,
            SourceType = SalesOrderSourceType.POSDirectSale,
            SourceDocumentId = request.Cart.Id,
            SourceDocumentNumber = request.Cart.SessionId ?? request.Cart.Channel,
            PaymentId = request.Payment.PaymentId,
            DeliveryDate = DateTime.UtcNow,
            Notes = request.Cart.Notes,
            InvoicingPolicy = SalesInvoicingPolicy.InvoiceDeliveredQuantity,
            Lines = request.Cart.Lines.Select(x => new SalesOrderLineDto
            {
                ProductId = x.ProductId,
                ProductSkuId = x.ProductSkuId,
                ProductName = x.ProductName,
                ProductNameEng = x.ProductNameEng,
                SkuCode = x.SkuCode,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                DiscountRate = x.DiscountRate,
                TaxRate = eligibility.IsTaxExempt ? 0m : x.TaxRate,
                Notes = x.Notes
            }).ToList()
        };

        var created = await sender.Send(new CreatePosDirectSalesOrderCommand(order), cancellationToken);
        return new CreatePosDirectSaleResult(created.Id, created.Number, created.AccountingDocumentId, created.ZatcaEInvoiceId);
    }
}
