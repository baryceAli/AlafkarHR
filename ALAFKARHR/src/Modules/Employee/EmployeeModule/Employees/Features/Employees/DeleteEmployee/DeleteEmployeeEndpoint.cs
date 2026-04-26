namespace EmployeeModule.Employees.Features.Employees.DeleteEmployee;

public record DeleteEmployeeResponse(bool IsSuccess);
public class DeleteEmployeeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/Employee/Employees/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteEmployeeCommand(id));
            return Results.Ok(result);
        })
            .WithName("DeleteEmployee")
            .Produces<DeleteEmployeeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeleteEmployee")
            .WithDescription("DeleteEmployee")
            .RequireAuthorization(PermissionList.EmployeePermissions.Delete);
    }
}
