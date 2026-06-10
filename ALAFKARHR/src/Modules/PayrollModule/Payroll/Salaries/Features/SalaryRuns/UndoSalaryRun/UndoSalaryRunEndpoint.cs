namespace Payroll.Salaries.Features.SalaryRuns.UndoSalaryRun;

public record UndoSalaryRunRequest(Guid SalaryRunId);

public record UndoSalaryRunResponse(Guid SalaryRunId, string Status, string Message);

public class UndoSalaryRunEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/salary-runs/{id}/undo", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new UndoSalaryRunCommand(id));
            return Results.Ok(result.Adapt<UndoSalaryRunResponse>());
        })
            .WithName("UndoSalaryRun")
            .Produces<UndoSalaryRunResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Undo Salary Run")
            .WithDescription("Returns a calculated salary run to draft and removes generated lines")
            .RequireAuthorization(PermissionList.SalaryRunPermissions.Edit);
    }
}
