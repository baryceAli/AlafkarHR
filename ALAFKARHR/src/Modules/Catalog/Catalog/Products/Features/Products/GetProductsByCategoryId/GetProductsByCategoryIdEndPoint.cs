using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Products.GetProductsByCategoryId;

public record GetProductsByCategoryIdRequest(Guid categoryId, PaginationRequest PaginationRequest);
public record GetProductsByCategoryIdResponse(PaginatedResult<ProductDto> ProductList);
public class GetProductsByCategoryIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/products/GetByCategory/{categoryId}", async ( [FromRoute] Guid categoryId,[AsParameters] PaginationRequest request,[FromServices] ISender sender) =>
        {
            //var query = request.Adapt<GetProductsByCategoryIdQuery>();
            var query=new GetProductsByCategoryIdQuery(categoryId,request);
            var result=await sender.Send(query);
            var response=result.Adapt<GetProductsByCategoryIdResponse>();
            return Results.Ok(response);
        })
            .WithName("GetProductsByCategoryId")
            .Produces<GetProductsByCategoryIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products By Category Id")
            .WithDescription("Get Products By Category Id");
    }
}
