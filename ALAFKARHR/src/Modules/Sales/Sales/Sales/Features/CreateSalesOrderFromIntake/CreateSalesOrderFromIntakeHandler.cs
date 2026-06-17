using Customers.Contracts.Customers.Features.GetCustomerSalesEligibility;
using Sales.Contracts.Sales.Features.CreateSalesOrderFromIntake;
using SalesOrder.Orders.Features.CreateOrder;

namespace Sales.Sales.Features.CreateSalesOrderFromIntake;

public class CreateSalesOrderFromIntakeHandler(ISender sender)
    : ICommandHandler<CreateSalesOrderFromIntakeCommand, CreateSalesOrderFromIntakeResult>
{
    public async Task<CreateSalesOrderFromIntakeResult> Handle(CreateSalesOrderFromIntakeCommand request, CancellationToken cancellationToken)
    {
        if (!request.Order.CustomerId.HasValue)
            throw new Exception("Customer is required before accepting an order into sales.");

        var requestedAmount = request.Order.Lines.Sum(x => x.Quantity * x.RequestedUnitPrice);
        var eligibility = await sender.Send(
            new GetCustomerSalesEligibilityQuery(request.Order.CustomerId.Value, request.Order.CompanyId, requestedAmount),
            cancellationToken);

        if (!eligibility.Exists || !eligibility.IsActive || !eligibility.IsCreditAllowed)
            throw new Exception(eligibility.BlockReason ?? "Customer is not eligible for sales.");

        var salesOrder = new SalesOrderDto
        {
            Number = $"SO-{request.Order.Number}",
            CustomerId = request.Order.CustomerId.Value,
            CompanyId = request.Order.CompanyId,
            Lines = request.Order.Lines.Select(x => new SalesOrderLineDto
            {
                ProductId = x.ProductId,
                ProductSkuId = x.ProductSkuId,
                ProductName = x.ProductName,
                ProductNameEng = x.ProductNameEng,
                SkuCode = x.SkuCode,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Quantity = x.Quantity,
                UnitPrice = x.RequestedUnitPrice,
                DiscountRate = x.RequestedDiscountRate,
                TaxRate = eligibility.IsTaxExempt ? 0m : x.RequestedTaxRate,
                Notes = x.Notes
            }).ToList()
        };

        var result = await sender.Send(new CreateOrderCommand(salesOrder), cancellationToken);
        return new CreateSalesOrderFromIntakeResult(result.Id);
    }
}
