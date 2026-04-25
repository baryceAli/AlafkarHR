using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Products.SearchProducts;


public record SearchProductsResponse(List<ProductDto> ProductList);
public class SearchProductsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/products/search/{searchTerm}", async (string searchTerm, ISender sender) =>
        {
            var result = await sender.Send(new SearchProductsQuery(searchTerm));
            return Results.Ok(result.Adapt<SearchProductsResponse>());
        })
            .WithName("SearchProducts")
            .Produces<SearchProductsResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("SearchProducts")
            .WithDescription("SearchProducts");
    }
}
