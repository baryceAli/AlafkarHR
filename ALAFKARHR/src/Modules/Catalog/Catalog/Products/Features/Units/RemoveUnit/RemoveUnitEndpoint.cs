using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Units.RemoveUnit;


public record RemoveUnitResponse(bool IsSuccess);
public class RemoveUnitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/catalog/units/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveUnitCommand(id));
            var response = result.Adapt<RemoveUnitResponse>();
            return Results.Ok(response);
        })
            .WithName("RemoveUnit")
            .Produces<RemoveUnitResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Remove Unit")
            .WithSummary("Remove Unit")
            .RequireAuthorization($"{PermissionList.UnitPermissions.Delete}");
    }
}
