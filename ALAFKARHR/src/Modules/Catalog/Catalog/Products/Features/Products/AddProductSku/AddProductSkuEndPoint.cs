using MediatR;

namespace Catalog.Products.Features.Products.AddProductSku;

public record AddProductSkuRequest(ProductSkuDto ProductSku);
public record AddProductSkuResponse(Guid Id);
public class AddProductSkuEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/catalog/products/skus", async (AddProductSkuRequest request, ISender sender) =>
        {
            var command= request.Adapt<AddProductSkuCommand>();
            var result= await sender.Send(command);

            var response=result.Adapt<AddProductSkuResponse>();

            return Results.Created($"/api/v1/catalog/products/skus/{response.Id}", response);

        })
            .WithName("AddProductSku")
            .Produces<AddProductSkuResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Add Product Sku")
            .WithDescription("Add Product Sku")
            .RequireAuthorization($"{PermissionList.ProductPermissions.Create}");
    }
}
