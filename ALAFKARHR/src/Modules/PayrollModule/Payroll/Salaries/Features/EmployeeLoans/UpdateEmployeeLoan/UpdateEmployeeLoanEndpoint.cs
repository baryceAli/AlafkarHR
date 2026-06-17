namespace Payroll.Salaries.Features.EmployeeLoans.UpdateEmployeeLoan;

public record UpdateEmployeeLoanRequest(UpdateEmployeeLoanDto EmployeeLoan);
public record UpdateEmployeeLoanResponse(Guid Id, string Status, string Message);

public class UpdateEmployeeLoanEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/payroll/loans/{id}", async (Guid id, UpdateEmployeeLoanRequest request, ISender sender) =>
        {
            var dto = request.EmployeeLoan;
            dto.Id = id;
            var result = await sender.Send(new UpdateEmployeeLoanCommand(dto));
            return Results.Ok(result.Adapt<UpdateEmployeeLoanResponse>());
        })
            .WithName("UpdateEmployeeLoan")
            .Produces<UpdateEmployeeLoanResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Update Employee Loan or Deduction")
            .WithDescription("Updates a draft employee loan or one-month deduction")
            .RequireAuthorization(PermissionList.PayrollLoanPermissions.Edit);
    }
}
