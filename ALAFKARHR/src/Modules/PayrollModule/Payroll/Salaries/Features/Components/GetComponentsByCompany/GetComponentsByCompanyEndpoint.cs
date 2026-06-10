namespace Payroll.Salaries.Features.Components.GetComponentsByCompany;

public record GetComponentsByCompanyResponse(PaginatedResult<ComponentDto> ComponentList);

public class GetComponentsByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/payroll/components/company/{companyId}",
            async (Guid companyId, [AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetComponentsByCompanyQuery(companyId, request));
                return Results.Ok(result.Adapt<GetComponentsByCompanyResponse>());
            })
            .WithName("GetPayrollComponentsByCompany")
            .Produces<GetComponentsByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get Payroll Components By Company")
            .WithDescription("Gets payroll components for a company")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.View);
    }
}
