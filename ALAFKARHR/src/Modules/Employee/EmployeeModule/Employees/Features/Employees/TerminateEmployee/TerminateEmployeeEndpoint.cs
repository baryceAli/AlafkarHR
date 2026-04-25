using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Employees.TerminateEmployee;



public record TerminateEmployeeRequest(TerminateEmployeeDto TerminateEmployee);
public record TerminateEmployeeResponse(bool IsSuccess);
public class TerminateEmployeeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/Terminate", async (TerminateEmployeeRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<TerminateEmployeeCommand>());
            return Results.Ok(result.Adapt<TerminateEmployeeResponse>());
        })
            .WithName("TerminateEmployee")
            .Produces<TerminateEmployeeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("TerminateEmployee")
            .WithDescription("TerminateEmployee")
            .RequireAuthorization(PermissionList.EmployeePermissions.Edit)
            .RequireAuthorization(PermissionList.EmployeePermissions.Edit);
    }
}
