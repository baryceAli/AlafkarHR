using EmployeeModule.Employees.Config;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeeById;


public record GetEmployeeByIdResponse(EmployeeDto Employee);
public class GetEmployeeByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}" + "/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeByIdQuery(id));
            return Results.Ok(result.Adapt<GetEmployeeByIdResponse>());
        })
            .WithName("GetEmployeeById")
            .Produces<GetEmployeeByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetEmployeeById")
            .WithDescription("GetEmployeeById")
            .RequireAuthorization(PermissionList.EmployeePermissions.View);
    }
}
