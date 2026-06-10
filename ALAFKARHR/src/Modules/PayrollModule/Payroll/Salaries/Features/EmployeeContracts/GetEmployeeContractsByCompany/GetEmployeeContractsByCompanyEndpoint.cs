using Microsoft.AspNetCore.Mvc;

namespace Payroll.Salaries.Features.EmployeeContracts.GetEmployeeContractsByCompany;

public record GetEmployeeContractsByCompanyResponse(PaginatedResult<EmployeeContractDto> EmployeeContractList);

public class GetEmployeeContractsByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/payroll/employee-contracts/company/{companyId}", async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeContractsByCompanyQuery(companyId, request));
            return Results.Ok(result.Adapt<GetEmployeeContractsByCompanyResponse>());
        })
            .WithName("GetEmployeeContractsByCompany")
            .Produces<GetEmployeeContractsByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Employee Contract Assignments")
            .WithDescription("Gets employee contract assignments by company")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.View);
    }
}
