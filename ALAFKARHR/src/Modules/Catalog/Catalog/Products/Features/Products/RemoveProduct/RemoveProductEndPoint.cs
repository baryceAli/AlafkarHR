using MediatR;

namespace Catalog.Products.Features.Products.RemoveProduct;

public record RemoveProductResponse(bool IsSuccess);
public class RemoveProductEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/catalog/products/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveProductCommand(id, "baryce@gmail.com"));
            var response = result.Adapt<RemoveProductResponse>();
            return Results.Ok(response);
        })
            .WithName("RemoveProduct")
            .Produces<RemoveProductResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Remove Product")
            .WithDescription("Remove Product")
            .RequireAuthorization($"{PermissionList.ProductPermissions.Delete}");
    }
}
