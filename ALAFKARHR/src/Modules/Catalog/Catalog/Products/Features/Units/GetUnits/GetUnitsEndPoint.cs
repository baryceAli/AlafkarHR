using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Units.GetUnits;

public record GetUnitsResponse(PaginatedResult<UnitDto> UnitList);

public class GetUnitsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/units", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetUnitsQuery(request));
            var response = result.Adapt<GetUnitsResponse>();
            return Results.Ok(response);
        })
            .WithName("GetUnits")
            .Produces<GetUnitsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Units")
            .WithDescription("Get Units");
    }
}
