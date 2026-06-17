namespace Cart.Carts.Features.AddCartLine;

public record AddCartLineRequest(CartLineDto Line);
public record AddCartLineResponse(bool IsSuccess);

public class AddCartLineEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/cart/carts/{id}/lines", async (Guid id, AddCartLineRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AddCartLineCommand(id, request.Line));
            return Results.Ok(result.Adapt<AddCartLineResponse>());
        })
        .WithName("AddCartLine")
        .Produces<AddCartLineResponse>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.CartPermissions.Edit);
    }
}
