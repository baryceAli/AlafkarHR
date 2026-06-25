namespace Cart.Carts.Features.CheckoutCart;

public record CheckoutCartRequest(PaymentMethodType PaymentMethod = PaymentMethodType.Cash, string? PaymentReference = null, string? PaymentNotes = null, Guid? BankAccountId = null);
public record CheckoutCartResponse(Guid OrderIntakeId, string Number, Guid PaymentId, PaymentStatus PaymentStatus, decimal CheckoutTotal);

public class CheckoutCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/cart/carts/{id}/checkout", async (Guid id, CheckoutCartRequest? request, ISender sender) =>
        {
            request ??= new CheckoutCartRequest();
            var result = await sender.Send(new CheckoutCartCommand(id, request.PaymentMethod, request.PaymentReference, request.PaymentNotes, BankAccountId: request.BankAccountId));
            return Results.Ok(result.Adapt<CheckoutCartResponse>());
        })
        .WithName("CheckoutCart")
        .Produces<CheckoutCartResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
