using Shared.Contracts.CQRS;
using SharedWithUI.Cart.Dtos;
using SharedWithUI.Payments.Dtos;

namespace Sales.Contracts.Sales.Features.CreatePosDirectSale;

public record CreatePosDirectSaleCommand(CartDto Cart, CheckoutPaymentDecisionDto Payment)
    : ICommand<CreatePosDirectSaleResult>;

public record CreatePosDirectSaleResult(
    Guid SalesOrderId,
    string SalesOrderNumber,
    Guid? AccountingDocumentId,
    Guid? ZatcaEInvoiceId);
