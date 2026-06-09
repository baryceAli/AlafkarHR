namespace Payroll.Salaries.Features.SalaryRuns.CreateSalaryRun;

public record CreateSalaryRunRequest(
    Guid EmployeeId,
    Guid ContractId,
    int SalaryMonth,
    int SalaryYear,
    decimal BaseSalary);

public record CreateSalaryRunResponse(Guid Id, string Message);

public class CreateSalaryRunEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/salary-runs", async (CreateSalaryRunRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateSalaryRunCommand>());
            return Results.Created($"/api/v1/payroll/salary-runs/{result.Id}", result.Adapt<CreateSalaryRunResponse>());
        })
            .WithName("CreateSalaryRun")
            .Produces<CreateSalaryRunResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Create Salary Run")
            .WithDescription("Creates a new salary run for an employee")
            .RequireAuthorization(PermissionList.SalaryRunPermissions.Create);
    }
}
