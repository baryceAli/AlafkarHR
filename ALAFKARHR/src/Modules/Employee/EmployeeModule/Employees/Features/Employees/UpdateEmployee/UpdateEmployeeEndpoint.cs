using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Employees.UpdateEmployee;


public record UpdateEmployeeRequest(EmployeeDto Employee);
public record UpdateEmployeeResponse(bool IsSuccess);
public class UpdateEmployeeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}", async (UpdateEmployeeRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateEmployeeCommand>());
            return Results.Ok(result.Adapt<UpdateEmployeeResponse>());
        })
            .WithName("UpdateEmployee")
            .Produces<UpdateEmployeeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateEmployee")
            .WithDescription("UpdateEmployee")
            .RequireAuthorization(PermissionList.EmployeePermissions.Edit);
    }
}
