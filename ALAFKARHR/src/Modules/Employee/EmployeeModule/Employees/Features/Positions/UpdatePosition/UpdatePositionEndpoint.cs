using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Positions.UpdatePosition;


public record UpdatePositionRequest(PositionDto Position);
public record UpdatePositionResponse(bool IsSuccess);
public class UpdatePositionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.URL_PATTERN}/{Utils.Position_Endpoint}", async (UpdatePositionRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdatePositionCommand>());
            return Results.Ok(result.Adapt<UpdatePositionResponse>());
        })
            .WithName("UpdatePosition")
            .Produces<UpdatePositionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdatePosition")
            .WithDescription("UpdatePosition")
            .RequireAuthorization(PermissionList.PositionPermissions.Edit);
    }
}
