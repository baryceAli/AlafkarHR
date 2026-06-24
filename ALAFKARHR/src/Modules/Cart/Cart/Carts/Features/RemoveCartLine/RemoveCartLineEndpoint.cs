namespace Cart.Carts.Features.RemoveCartLine;

public record RemoveCartLineResponse(bool IsSuccess);

public class RemoveCartLineEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/cart/carts/{id}/lines/{lineId}", async (Guid id, Guid lineId, ISender sender) =>
        {
            var result = await sender.Send(new RemoveCartLineCommand(id, lineId));
            return Results.Ok(result.Adapt<RemoveCartLineResponse>());
        })
        .WithName("RemoveCartLine")
        .Produces<RemoveCartLineResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
