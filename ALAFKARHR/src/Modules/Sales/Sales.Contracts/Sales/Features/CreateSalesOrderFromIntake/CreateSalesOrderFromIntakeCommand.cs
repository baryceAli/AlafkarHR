using Shared.Contracts.CQRS;
using SharedWithUI.Orders.Dtos;

namespace Sales.Contracts.Sales.Features.CreateSalesOrderFromIntake;

public record CreateSalesOrderFromIntakeCommand(OrderIntakeDto Order) : ICommand<CreateSalesOrderFromIntakeResult>;
public record CreateSalesOrderFromIntakeResult(Guid SalesOrderId);
