namespace Payroll.Salaries.Features.EmployeeLoans.CreateEmployeeLoan;

public record CreateEmployeeLoanRequest(CreateEmployeeLoanDto EmployeeLoan);
public record CreateEmployeeLoanResponse(Guid Id, string Status, string Message);

public class CreateEmployeeLoanEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/loans", async (CreateEmployeeLoanRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateEmployeeLoanCommand>());
            return Results.Created($"/api/v1/payroll/loans/{result.Id}", result.Adapt<CreateEmployeeLoanResponse>());
        })
            .WithName("CreateEmployeeLoan")
            .Produces<CreateEmployeeLoanResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Create Employee Loan or Deduction")
            .WithDescription("Creates a draft employee loan or one-month deduction")
            .RequireAuthorization(PermissionList.PayrollLoanPermissions.Create);
    }
}
