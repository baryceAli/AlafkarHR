using MediatR;

namespace Catalog.Products.Features.Products.GetPublicStoreProductSkus;

public record GetPublicStoreProductSkusResponse(PaginatedResult<ProductSkuDto> ProductSkus);

public class GetPublicStoreProductSkusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/public/products/skus",
            async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetPublicStoreProductSkusQuery(request));
                return Results.Ok(result.Adapt<GetPublicStoreProductSkusResponse>());
            })
            .WithName("GetPublicStoreProductSkus")
            .Produces<GetPublicStoreProductSkusResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get public store product SKUs")
            .WithDescription("Returns product SKUs marked to show on the public store without requiring authentication.");
    }
}
