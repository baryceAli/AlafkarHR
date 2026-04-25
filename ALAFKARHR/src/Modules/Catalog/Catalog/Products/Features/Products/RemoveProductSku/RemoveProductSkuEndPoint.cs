using MediatR;

namespace Catalog.Products.Features.Products.RemoveProductSku;

public record RemoveProductSkuResponse(bool IsSuccess);

public class RemoveProductSkuEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/catalog/products/Skus/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveProductSkuCommand(id, "baryce@gmail.com"));
            var response = result.Adapt<RemoveProductSkuResponse>();
            return Results.Ok(response);
        })
            .WithName("RemoveProductSku")
            .Produces<RemoveProductSkuResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Remove Product Sku")
            .WithDescription("Remove Product Sku")
            .RequireAuthorization($"{PermissionList.ProductPermissions.Delete}");
    }
}
