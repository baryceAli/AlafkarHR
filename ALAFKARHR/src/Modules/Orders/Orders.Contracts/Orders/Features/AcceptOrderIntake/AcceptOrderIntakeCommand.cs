using Shared.Contracts.CQRS;

namespace Orders.Contracts.Orders.Features.AcceptOrderIntake;

public record AcceptOrderIntakeCommand(Guid Id) : ICommand<AcceptOrderIntakeResult>;
public record AcceptOrderIntakeResult(Guid Id, Guid SalesOrderId);
