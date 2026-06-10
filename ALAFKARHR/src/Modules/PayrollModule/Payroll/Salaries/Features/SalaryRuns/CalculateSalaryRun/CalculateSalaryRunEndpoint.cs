namespace Payroll.Salaries.Features.SalaryRuns.CalculateSalaryRun;

public record CalculateSalaryRunRequest(Guid SalaryRunId);

public record CalculateSalaryRunResponse(
    Guid Id,
    Guid EmployeeId,
    Guid ContractId,
    int SalaryMonth,
    int SalaryYear,
    decimal TotalSalary,
    decimal TotalAllowances,
    decimal TotalDeductions,
    decimal TaxPercentage,
    decimal TaxableAmount,
    decimal TaxAmount,
    decimal InsurancePercentage,
    decimal InsuranceAmount,
    decimal NetSalary,
    string Status,
    string Message);

public class CalculateSalaryRunEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/salary-runs/{id}/calculate", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CalculateSalaryRunCommand(id));
            return Results.Ok(result.Adapt<CalculateSalaryRunResponse>());
        })
            .WithName("CalculateSalaryRun")
            .Produces<CalculateSalaryRunResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Calculate Salary Run")
            .WithDescription("Calculates all allowances and deductions for a salary run")
            .RequireAuthorization(PermissionList.SalaryRunPermissions.Edit);
    }
}
