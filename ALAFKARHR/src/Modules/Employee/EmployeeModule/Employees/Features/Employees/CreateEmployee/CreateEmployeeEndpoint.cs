using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Employees.CreateEmployee;


public record CreateEmployeeRequest(EmployeeDto Employee);
public record CreateEmployeeResponse(EmployeeDto CreatedEmployee);
public class CreateEmployeeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}", async (CreateEmployeeRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateEmployeeCommand>());
            return Results.Created($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/{result.CreatedEmployee.Id}", result.Adapt<CreateEmployeeResponse>());
        })
            .WithName("CreateEmployee")
            .Produces<CreateEmployeeResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateEmployee")
            .WithDescription("CreateEmployee")
            .RequireAuthorization(PermissionList.EmployeePermissions.Create);
    }
}
