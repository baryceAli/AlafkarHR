using Shared.Contracts.CQRS;
using SharedWithUI.Payments.Dtos;

namespace Payments.Contracts.Payments.Features.ConfirmCheckoutPayment;

public record ConfirmCheckoutPaymentCommand(CheckoutPaymentRequestDto Payment)
    : ICommand<ConfirmCheckoutPaymentResult>;

public record ConfirmCheckoutPaymentResult(CheckoutPaymentDecisionDto Payment);
