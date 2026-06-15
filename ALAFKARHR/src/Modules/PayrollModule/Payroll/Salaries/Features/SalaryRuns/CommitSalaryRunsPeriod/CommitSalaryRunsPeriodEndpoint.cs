namespace Payroll.Salaries.Features.SalaryRuns.CommitSalaryRunsPeriod;

public record CommitSalaryRunsPeriodRequest(Guid CompanyId, int SalaryMonth, int SalaryYear);

public record CommitSalaryRunsPeriodResponse(int CommittedCount, string Status, string Message);

public class CommitSalaryRunsPeriodEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/salary-runs/period/commit", async (CommitSalaryRunsPeriodRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CommitSalaryRunsPeriodCommand>());
            return Results.Ok(result.Adapt<CommitSalaryRunsPeriodResponse>());
        })
            .WithName("CommitSalaryRunsPeriod")
            .Produces<CommitSalaryRunsPeriodResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Commit Salary Runs Period")
            .WithDescription("Commits all calculated salary runs for a company month and year")
            .RequireAuthorization(PermissionList.SalaryRunPermissions.Approve);
    }
}
