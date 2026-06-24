using Microsoft.AspNetCore.Mvc;

namespace Auth.Users.Features.Dashboard;

public record GetAuthDashboardResponse(AuthDashboardDto Dashboard);

public class GetAuthDashboardEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/dashboard", async ([FromQuery] Guid companyId, ClaimsPrincipal user, ISender sender) =>
        {
            if (companyId != GetCompanyId(user))
                throw new BadRequestException("Cannot view auth dashboard for another company.");

            var result = await sender.Send(new GetAuthDashboardQuery(companyId));
            return Results.Ok(new GetAuthDashboardResponse(result.Dashboard));
        })
            .WithName("GetAuthDashboard")
            .Produces<GetAuthDashboardResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get auth dashboard")
            .WithDescription("Returns scope-aware authentication dashboard metrics for the current company.")
            .RequireAuthorization(PermissionList.UsersPermissions.Select);
    }

    private static Guid GetCompanyId(ClaimsPrincipal user)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(c => c.Type == "company_id")?.Value, out var companyId))
            throw new BadRequestException("Company claim is missing.");

        return companyId;
    }
}
