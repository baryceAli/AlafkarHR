namespace Cart.Carts.Features.UpdateCartLine;

public record UpdateCartLineRequest(decimal Quantity);
public record UpdateCartLineResponse(bool IsSuccess);

public class UpdateCartLineEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/cart/carts/{id}/lines/{lineId}", async (Guid id, Guid lineId, UpdateCartLineRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateCartLineCommand(id, lineId, request.Quantity));
            return Results.Ok(result.Adapt<UpdateCartLineResponse>());
        })
        .WithName("UpdateCartLine")
        .Produces<UpdateCartLineResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
