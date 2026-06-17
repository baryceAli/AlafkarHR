namespace Cart.Carts.Features.CreateCart;

public record CreateCartRequest(CartDto Cart);
public record CreateCartResponse(Guid Id);

public class CreateCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/cart/carts", async (CreateCartRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateCartCommand(request.Cart));
            return Results.Created($"/api/v1/cart/carts/{result.Id}", result.Adapt<CreateCartResponse>());
        })
        .WithName("CreateCart")
        .Produces<CreateCartResponse>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.CartPermissions.Create);
    }
}
