using Shared.Contracts.CQRS;
using SharedWithUI.Orders.Dtos;

namespace Orders.Contracts.Orders.Features.SubmitOrderIntake;

public record SubmitOrderIntakeCommand(OrderIntakeDto Order) : ICommand<SubmitOrderIntakeResult>;
public record SubmitOrderIntakeResult(Guid Id, string Number);
