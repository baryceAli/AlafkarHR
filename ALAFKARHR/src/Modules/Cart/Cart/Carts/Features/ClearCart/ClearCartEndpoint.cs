namespace Cart.Carts.Features.ClearCart;

public record ClearCartResponse(bool IsSuccess);

public class ClearCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/cart/carts/{id}/lines", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ClearCartCommand(id));
            return Results.Ok(result.Adapt<ClearCartResponse>());
        })
        .WithName("ClearCart")
        .Produces<ClearCartResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
