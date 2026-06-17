namespace Payroll.Salaries.Features.EmployeeLoans.CancelEmployeeLoan;

public record CancelEmployeeLoanResponse(Guid Id, string Status, string Message);

public class CancelEmployeeLoanEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/loans/{id}/cancel", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CancelEmployeeLoanCommand(id));
            return Results.Ok(result.Adapt<CancelEmployeeLoanResponse>());
        })
            .WithName("CancelEmployeeLoan")
            .Produces<CancelEmployeeLoanResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Cancel Employee Loan or Deduction")
            .WithDescription("Cancels an employee loan or one-month deduction")
            .RequireAuthorization(PermissionList.PayrollLoanPermissions.Cancel);
    }
}
