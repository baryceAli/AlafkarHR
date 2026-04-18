using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Employees.ChangePosition;

public record ChangePositionRequest(ChangePositionDto ChangePosition);
public record ChangePositionResponse(bool IsSuccess);
public class ChangePositionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/ChangePosition", async (ChangePositionRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<ChangePositionCommand>());
            return Results.Ok(result.Adapt<ChangePositionResponse>());
        })
            .WithName("ChangePosition")
            .Produces<ChangePositionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("ChangePosition")
            .WithDescription("ChangePosition")
            .RequireAuthorization(PermissionList.EmployeePermissions.Edit);
    }
}
