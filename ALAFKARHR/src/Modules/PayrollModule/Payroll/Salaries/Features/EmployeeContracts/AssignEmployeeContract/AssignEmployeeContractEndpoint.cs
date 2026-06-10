namespace Payroll.Salaries.Features.EmployeeContracts.AssignEmployeeContract;

public record AssignEmployeeContractRequest(EmployeeContractDto EmployeeContract);

public class AssignEmployeeContractEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/employee-contracts", async (AssignEmployeeContractRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AssignEmployeeContractCommand(request.EmployeeContract));
            return Results.Created($"/api/v1/payroll/employee-contracts/{result.Id}", result);
        })
            .WithName("AssignEmployeeContract")
            .Produces<EmployeeContractDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Assign Contract To Employee")
            .WithDescription("Assigns a salary contract to an employee and deactivates previous active assignments for that employee")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.Create);
    }
}
