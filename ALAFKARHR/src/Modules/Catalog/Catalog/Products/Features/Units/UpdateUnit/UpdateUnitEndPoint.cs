using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Units.UpdateUnit;


public record UpdateUnitRequest(UnitDto Unit);
public record UpdateUnitResponse(bool IsSuccess);
public class UpdateUnitEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/catalog/units", async (UpdateUnitRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateUnitCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateUnitResponse>();
            return Results.Ok(response);
        })
            .WithName("UpdateUnit")
            .Produces<UpdateUnitResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Update Unit")
            .WithDescription("Update Unit")
            .RequireAuthorization($"{PermissionList.UnitPermissions.Edit}");
    }
}
