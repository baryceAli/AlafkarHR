using MediatR;

namespace Catalog.Products.Features.Units.CreateUnit;


public record CreateUnitRequest(UnitDto Unit);
public record CreateUnitResponse(Guid Id);
public class CreateUnitEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/catalog/units", async (CreateUnitRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateUnitCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateUnitResponse>();
            return Results.Created($"/api/v1/catalog/units/{response.Id}", response);

        })
            .WithName("CreateUnit")
            .Produces<CreateUnitResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Unit")
            .WithDescription("Create Unit")
            .RequireAuthorization($"{PermissionList.UnitPermissions.Create}");
    }
}
