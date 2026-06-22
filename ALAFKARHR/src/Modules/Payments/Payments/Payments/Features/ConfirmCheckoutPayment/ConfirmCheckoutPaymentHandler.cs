using Customers.Contracts.Customers.Features.GetCustomerSalesEligibility;
using Payments.Contracts.Payments.Features.ConfirmCheckoutPayment;
using Payments.Payments.Models;

namespace Payments.Payments.Features.ConfirmCheckoutPayment;

public class ConfirmCheckoutPaymentHandler(PaymentsDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ConfirmCheckoutPaymentCommand, ConfirmCheckoutPaymentResult>
{
    public async Task<ConfirmCheckoutPaymentResult> Handle(ConfirmCheckoutPaymentCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var status = request.Payment.Method is PaymentMethodType.Cash or PaymentMethodType.CardRecorded
            ? PaymentStatus.Received
            : PaymentStatus.Approved;
        string? reason = null;

        if (!request.Payment.CustomerId.HasValue)
        {
            status = PaymentStatus.Rejected;
            reason = "Customer is required for checkout payment.";
        }
        else
        {
            var eligibility = await sender.Send(
                new GetCustomerSalesEligibilityQuery(request.Payment.CustomerId.Value, request.Payment.CompanyId, request.Payment.Amount),
                cancellationToken);

            if (!eligibility.Exists || !eligibility.IsActive)
            {
                status = PaymentStatus.Rejected;
                reason = eligibility.BlockReason ?? "Customer is not eligible for checkout.";
            }
            else if (request.Payment.Method == PaymentMethodType.PayOnReceive && !eligibility.IsCreditAllowed)
            {
                status = PaymentStatus.Rejected;
                reason = eligibility.BlockReason ?? "Customer is not eligible for pay on receive.";
            }
        }

        var payment = PaymentAttempt.Create(request.Payment, status, reason, userId);
        await dbContext.PaymentAttempts.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (status == PaymentStatus.Received)
        {
            await sender.Send(new RecordAccountingReceiptCommand(
                request.Payment.CompanyId,
                null,
                request.Payment.CustomerId,
                null,
                "Payments",
                request.Payment.SourceDocumentId,
                request.Payment.SourceDocumentNumber,
                request.Payment.Amount,
                request.Payment.Method == PaymentMethodType.CardRecorded,
                DateTime.UtcNow), cancellationToken);
        }

        return new ConfirmCheckoutPaymentResult(payment.ToDecisionDto());
    }
}
