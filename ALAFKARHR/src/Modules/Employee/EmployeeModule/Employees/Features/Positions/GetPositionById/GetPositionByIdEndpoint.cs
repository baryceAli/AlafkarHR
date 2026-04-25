using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Positions.GetPositionById;


public record GetPositionByIdResponse(PositionDto Position);
public class GetPositionByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Position_Endpoint}" + "/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetPositionByIdQuery(id));
            return Results.Ok(result.Adapt<PositionDto>());
        })
            .WithName("GetPositionById")
            .Produces<GetPositionByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetPositionById")
            .WithDescription("GetPositionById")
            .RequireAuthorization(PermissionList.PositionPermissions.View);
    }
}
