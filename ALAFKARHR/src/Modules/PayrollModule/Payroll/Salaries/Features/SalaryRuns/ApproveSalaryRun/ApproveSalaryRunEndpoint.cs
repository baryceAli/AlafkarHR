namespace Payroll.Salaries.Features.SalaryRuns.ApproveSalaryRun;

public record ApproveSalaryRunRequest(Guid SalaryRunId);

public record ApproveSalaryRunResponse(Guid SalaryRunId, string Status, string Message);

public class ApproveSalaryRunEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/salary-runs/{id}/approve", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ApproveSalaryRunCommand(id));
            return Results.Ok(result.Adapt<ApproveSalaryRunResponse>());
        })
            .WithName("ApproveSalaryRun")
            .Produces<ApproveSalaryRunResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Approve Salary Run")
            .WithDescription("Approves a calculated salary run");
    }
}
