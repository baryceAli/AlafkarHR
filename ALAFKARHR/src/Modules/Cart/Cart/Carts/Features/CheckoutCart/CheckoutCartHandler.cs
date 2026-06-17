using Orders.Contracts.Orders.Features.SubmitOrderIntake;
using Payments.Contracts.Payments.Features.ConfirmCheckoutPayment;
using Pricing.Contracts.Pricings.Features.ResolvePrice;

namespace Cart.Carts.Features.CheckoutCart;

public record CheckoutCartCommand(Guid CartId, PaymentMethodType PaymentMethod, string? PaymentReference, string? PaymentNotes, string? CouponCode = null) : ICommand<CheckoutCartResult>;
public record CheckoutCartResult(Guid OrderIntakeId, string Number, Guid PaymentId, PaymentStatus PaymentStatus, decimal CheckoutTotal);

public class CheckoutCartHandler(CartDbContext dbContext, ISender sender)
    : ICommandHandler<CheckoutCartCommand, CheckoutCartResult>
{
    public async Task<CheckoutCartResult> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await dbContext.Carts.Include("_lines").FirstOrDefaultAsync(x => x.Id == request.CartId, cancellationToken)
            ?? throw new NotFoundException($"Cart not found: {request.CartId}");
        if (!cart.Lines.Any())
            throw new Exception("Cart is empty.");

        if (!cart.CustomerId.HasValue)
            throw new Exception("Customer is required for checkout pricing and payment.");

        var userId = "checkout";
        var cartSubtotal = cart.Lines.Where(x => !x.IsDeleted).Sum(x => x.Quantity * x.UnitPrice);
        foreach (var line in cart.Lines.Where(x => !x.IsDeleted))
        {
            var priceResult = await sender.Send(
                new ResolvePriceQuery(
                    cart.CustomerId.Value,
                    line.ProductSkuId,
                    line.UnitOfMeasureId,
                    line.Quantity,
                    cart.CompanyId,
                    cart.PriceListId,
                    line.TaxRate,
                    DateTime.UtcNow,
                    request.CouponCode,
                    cartSubtotal),
                cancellationToken);

            cart.UpdateLineCheckoutPrice(
                line.Id,
                priceResult.Price.UnitPrice,
                priceResult.Price.DiscountRate,
                priceResult.Price.TaxRate,
                userId);
        }

        var repricedCart = cart.ToDto();
        var paymentResult = await sender.Send(new ConfirmCheckoutPaymentCommand(new CheckoutPaymentRequestDto
        {
            CompanyId = cart.CompanyId,
            CustomerId = cart.CustomerId,
            Source = cart.Source,
            Channel = cart.Channel,
            Method = request.PaymentMethod,
            Amount = repricedCart.TotalAmount,
            Reference = request.PaymentReference,
            Notes = request.PaymentNotes
        }), cancellationToken);

        if (!paymentResult.Payment.IsApproved)
            throw new Exception(paymentResult.Payment.DecisionReason ?? "Checkout payment was rejected.");

        var orderResult = await sender.Send(new SubmitOrderIntakeCommand(cart.ToOrderIntakeDto(paymentResult.Payment)), cancellationToken);
        cart.Clear("checkout");
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CheckoutCartResult(orderResult.Id, orderResult.Number, paymentResult.Payment.PaymentId, paymentResult.Payment.Status, paymentResult.Payment.Amount);
    }
}
