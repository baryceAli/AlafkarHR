using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Products.GetProductSkuById;

public record GetProductSkuByIdResponse(ProductSkuDto ProductSku);
public class GetProductSkuByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/products/skus/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProductSkuByIdQuery(id));
            var response = result.Adapt<GetProductSkuByIdResponse>();
            return Results.Ok(response);
        })
            .WithName("GetProductskuById")
            .Produces<GetProductSkuByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Product Sku")
            .WithDescription("Product Sku");
    }
}
