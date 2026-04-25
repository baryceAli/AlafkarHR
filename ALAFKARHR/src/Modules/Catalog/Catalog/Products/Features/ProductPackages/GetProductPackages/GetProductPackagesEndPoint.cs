using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.ProductPackages.GetProductPackages;


public record GetProductPackagesResponse(PaginatedResult<ProductPackageDto> ProductPackageList);
public class GetProductPackagesEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/packages", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var query = new GetProductPackagesQuery(request);
            var result = await sender.Send(query);
            var response = result.Adapt<GetProductPackagesResponse>();
            return Results.Ok(response);
        })
            .WithName("GetProductPackages")
            .Produces<GetProductPackagesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product Packages")
            .WithDescription("Get Product Packages");
    }
}
