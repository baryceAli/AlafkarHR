using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Positions.CreatePosition;

public record CreatePositionRequest(PositionDto Position);
public record CreatePositionResponse(PositionDto CreatedPosition);
public class CreatePositionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.URL_PATTERN}/{Utils.Position_Endpoint}", async (CreatePositionRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreatePositionCommand>());
            return Results.Created($"{Utils.URL_PATTERN}/{Utils.Position_Endpoint}/{result.CreatedPosition.Id}", result.Adapt<CreatePositionResponse>());
        })
            .WithName("CreatePosition")
            .Produces<CreatePositionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreatePosition")
            .WithDescription("CreatePosition")
            .RequireAuthorization(PermissionList.PositionPermissions.Create);
    }
}
