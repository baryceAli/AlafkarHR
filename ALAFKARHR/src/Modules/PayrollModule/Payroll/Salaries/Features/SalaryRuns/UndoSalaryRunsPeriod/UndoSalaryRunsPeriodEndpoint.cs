namespace Payroll.Salaries.Features.SalaryRuns.UndoSalaryRunsPeriod;

public record UndoSalaryRunsPeriodRequest(Guid CompanyId, int SalaryMonth, int SalaryYear);

public record UndoSalaryRunsPeriodResponse(int DeletedCount, string Message);

public class UndoSalaryRunsPeriodEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/salary-runs/period/undo", async (UndoSalaryRunsPeriodRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UndoSalaryRunsPeriodCommand>());
            return Results.Ok(result.Adapt<UndoSalaryRunsPeriodResponse>());
        })
            .WithName("UndoSalaryRunsPeriod")
            .Produces<UndoSalaryRunsPeriodResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Undo Salary Runs Period")
            .WithDescription("Removes all salary runs for a company month and year")
            .RequireAuthorization();
    }
}
