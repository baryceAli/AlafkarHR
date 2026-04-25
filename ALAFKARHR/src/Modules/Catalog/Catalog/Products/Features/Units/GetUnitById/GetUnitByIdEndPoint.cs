using MediatR;

namespace Catalog.Products.Features.Units.GetUnitById;

public record GetUnitByIdResponse(UnitDto Unit);

public class GetUnitByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/units/{id}", async ([FromRoute] Guid id,[FromServices] ISender sender) =>
        {
            var result= await sender.Send(new GetUnitByIdQuery(id));
            var response =result.Adapt<GetUnitByIdResponse>();
            return Results.Ok(response);

        })
            .WithName("GetUnitById")
            .Produces<GetUnitByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Unit By Id")
            .WithDescription("Get Unit By Id");
    }
}
