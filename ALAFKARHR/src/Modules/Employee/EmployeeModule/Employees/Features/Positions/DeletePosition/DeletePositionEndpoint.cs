using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Positions.DeletePosition;

public record DeletePositionResponse(bool IsSuccess);
public class DeletePositionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete($"{Utils.URL_PATTERN}/{Utils.Position_Endpoint}" + "/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeletePositionCommand(id));
            return Results.Ok(result.Adapt<DeletePositionResponse>());
        })
            .WithName("DeletePosition")
            .Produces<DeletePositionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeletePosition")
            .WithDescription("DeletePosition")
            .RequireAuthorization(PermissionList.PositionPermissions.Delete);
    }
}
