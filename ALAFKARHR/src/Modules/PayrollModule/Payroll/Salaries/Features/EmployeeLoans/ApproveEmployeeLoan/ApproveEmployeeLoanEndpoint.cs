namespace Payroll.Salaries.Features.EmployeeLoans.ApproveEmployeeLoan;

public record ApproveEmployeeLoanResponse(Guid Id, string Status, string Message);

public class ApproveEmployeeLoanEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/loans/{id}/approve", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ApproveEmployeeLoanCommand(id));
            return Results.Ok(result.Adapt<ApproveEmployeeLoanResponse>());
        })
            .WithName("ApproveEmployeeLoan")
            .Produces<ApproveEmployeeLoanResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Approve Employee Loan or Deduction")
            .WithDescription("Approves a draft employee loan or one-month deduction")
            .RequireAuthorization(PermissionList.PayrollLoanPermissions.Approve);
    }
}
