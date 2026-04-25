using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Products.UpdateProductSku;

public record UpdateProductSkuRequest(ProductSkuDto ProductSku);
public record UpdateProductSkuResponse(bool IsSuccess);
public class UpdateProductSkuEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/catalog/products/skus", async (UpdateProductSkuRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateProductSkuCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateProductSkuResponse>();
            return Results.Ok(response);
        })
            .WithName("UpdateProductSku")
            .Produces<UpdateProductSkuResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Product Sku")
            .WithDescription("Update Product Sku")
            .RequireAuthorization($"{PermissionList.ProductPermissions.Edit}");
    }
}
