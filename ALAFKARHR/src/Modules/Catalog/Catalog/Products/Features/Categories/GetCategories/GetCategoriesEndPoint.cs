using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Categories.GetCategories;

//public record GetCategoriesRequest
public record GetCategoriesResponse(PaginatedResult<CategoryDto> CategoryList);
public class GetCategoriesEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/categories", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result=await sender.Send(new GetCategoriesQuery(request));

            var response =result.Adapt<GetCategoriesResponse>();

            return Results.Ok(response);
        })
            .WithName("GetCategories")
            .Produces<GetCategoriesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Categories")
            .WithDescription("Get Categories");
    }
}
