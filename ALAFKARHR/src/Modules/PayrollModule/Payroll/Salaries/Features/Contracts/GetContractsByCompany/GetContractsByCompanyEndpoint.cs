namespace Payroll.Salaries.Features.Contracts.GetContractsByCompany;

public record GetContractsByCompanyResponse(PaginatedResult<ContractDto> ContractList);

public class GetContractsByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/payroll/contracts/company/{companyId}",
            async (Guid companyId, [AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetContractsByCompanyQuery(companyId, request));
                return Results.Ok(result.Adapt<GetContractsByCompanyResponse>());
            })
            .WithName("GetPayrollContractsByCompany")
            .Produces<GetContractsByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get Payroll Contracts By Company")
            .WithDescription("Gets salary contracts for a company")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.View);
    }
}
