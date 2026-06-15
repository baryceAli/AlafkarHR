using MediatR;

namespace Catalog.Products.Features.Products.GetPublicStoreProductSkus;

public record GetPublicStoreProductSkusResponse(PaginatedResult<ProductSkuDto> ProductSkus);
public record GetPublicStoreProductSkuFiltersResponse(PublicStoreProductSkuFilterMetadataDto Metadata);

public class GetPublicStoreProductSkusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/public/products/skus",
            async ([AsParameters] PublicStoreProductSkuRequest request, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetPublicStoreProductSkusQuery(request));
                return Results.Ok(result.Adapt<GetPublicStoreProductSkusResponse>());
            })
            .WithName("GetPublicStoreProductSkus")
            .Produces<GetPublicStoreProductSkusResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get public store product SKUs")
            .WithDescription("Returns product SKUs marked to show on the public store without requiring authentication.")
            .AllowAnonymous();

        app.MapGet("/api/v1/catalog/public/products/skus/filters",
            async ([FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetPublicStoreProductSkuFiltersQuery());
                return Results.Ok(result.Adapt<GetPublicStoreProductSkuFiltersResponse>());
            })
            .WithName("GetPublicStoreProductSkuFilters")
            .Produces<GetPublicStoreProductSkuFiltersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get public store product SKU filters")
            .WithDescription("Returns filter options for product SKUs marked to show on the public store.")
            .AllowAnonymous();
    }
}
