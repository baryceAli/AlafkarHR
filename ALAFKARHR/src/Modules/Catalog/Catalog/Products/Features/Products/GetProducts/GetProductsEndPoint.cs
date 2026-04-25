using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Products.GetProducts;

public record GetProductsResponse(PaginatedResult<ProductDto> ProductList);
public class GetProductsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/products", async ([AsParameters]PaginationRequest request, [FromServices]ISender sender) =>
        {
            var query= new GetProductsQuery(request);
            var result=await sender.Send(query);
            var response=result.Adapt<GetProductsResponse>();

            return Results.Ok(response);
        })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
    }
}
