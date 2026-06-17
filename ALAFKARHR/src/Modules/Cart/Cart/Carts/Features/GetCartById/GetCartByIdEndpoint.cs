namespace Cart.Carts.Features.GetCartById;

public record GetCartByIdResponse(CartDto Cart);

public class GetCartByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/cart/carts/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCartByIdQuery(id));
            return Results.Ok(result.Adapt<GetCartByIdResponse>());
        })
        .WithName("GetCartById")
        .Produces<GetCartByIdResponse>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.CartPermissions.View);
    }
}
