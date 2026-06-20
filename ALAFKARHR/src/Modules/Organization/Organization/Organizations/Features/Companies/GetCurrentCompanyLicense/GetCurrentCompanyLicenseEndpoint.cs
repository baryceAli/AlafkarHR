namespace Organization.Organizations.Features.Companies.GetCurrentCompanyLicense;

public record GetCurrentCompanyLicenseResponse(CompanyLicenseSummaryDto License);

public class GetCurrentCompanyLicenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/current/license", async (ISender sender) =>
        {
            var result = await sender.Send(new GetCurrentCompanyLicenseQuery());
            return Results.Ok(result.Adapt<GetCurrentCompanyLicenseResponse>());
        })
            .WithName("GetCurrentCompanyLicense")
            .Produces<GetCurrentCompanyLicenseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.CompanyPermissions.ViewLicense);
    }
}
