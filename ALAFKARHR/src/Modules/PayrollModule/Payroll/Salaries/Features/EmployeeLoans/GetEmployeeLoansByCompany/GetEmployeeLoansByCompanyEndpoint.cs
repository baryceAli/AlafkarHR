namespace Payroll.Salaries.Features.EmployeeLoans.GetEmployeeLoansByCompany;

public record GetEmployeeLoansByCompanyResponse(PaginatedResult<EmployeeLoanDto> EmployeeLoanList);

public class GetEmployeeLoansByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/payroll/loans/company/{companyId}",
            async (Guid companyId, Guid? employeeId, EmployeeLoanStatus? status, [AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetEmployeeLoansByCompanyQuery(companyId, employeeId, status, request));
                return Results.Ok(result.Adapt<GetEmployeeLoansByCompanyResponse>());
            })
            .WithName("GetEmployeeLoansByCompany")
            .Produces<GetEmployeeLoansByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get Employee Loans By Company")
            .WithDescription("Gets employee loans and one-month deductions for a company")
            .RequireAuthorization(PermissionList.PayrollLoanPermissions.View);
    }
}
