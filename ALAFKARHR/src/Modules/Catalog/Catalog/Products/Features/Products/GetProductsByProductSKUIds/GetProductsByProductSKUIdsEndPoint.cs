using MediatR;

namespace Catalog.Products.Features.Products.GetProductsByProductSKUIds;

public record GetProductsByProductSKUIdsRequest(List<Guid> ProductSkus);
public record GetProductsByProductSKUIdsResponse(List<ProductDto> ProductList);
public class GetProductsByProductSKUIdsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/catalog/products/getBySkuIds", async (GetProductsByProductSKUIdsRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetProductsByProductSKUIdsQuery(request.ProductSkus);
            var result = await sender.Send(query);
            return Results.Ok(result.Adapt<GetProductsByProductSKUIdsResponse>());
        })
            .WithName("GetProductsByProductSKUIds")
            .Produces<GetProductsByProductSKUIdsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetProductsByProductSKUIds")
            .WithDescription("GetProductsByProductSKUIds")
            .RequireAuthorization(PermissionList.ProductPermissions.View);
    }
}
