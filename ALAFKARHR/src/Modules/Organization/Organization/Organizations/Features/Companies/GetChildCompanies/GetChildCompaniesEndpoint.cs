namespace Organization.Organizations.Features.Companies.GetChildCompanies;

public record GetChildCompaniesResponse(PaginatedResult<CompanyDto> CompanyList);

public class GetChildCompaniesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/child-companies", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetChildCompaniesQuery(request));
            return Results.Ok(result.Adapt<GetChildCompaniesResponse>());
        })
            .WithName("GetChildCompanies")
            .Produces<GetChildCompaniesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get child companies")
            .WithDescription("Get child companies")
            .RequireAuthorization(PermissionList.CompanyPermissions.ViewChild);
    }
}
