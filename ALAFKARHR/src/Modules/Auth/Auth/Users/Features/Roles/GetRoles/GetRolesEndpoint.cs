using Microsoft.AspNetCore.Mvc;

namespace Auth.Users.Features.Roles.GetRoles;


public record GetRolesResponse(List<RoleDto> RoleList);
public class GetRolesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/roles/company/{companyId}", async ([FromRoute] Guid companyId, ClaimsPrincipal user, ISender sender) =>
        {
            if (companyId != GetCompanyId(user))
                throw new BadRequestException("Cannot view roles for another company.");

            var result = await sender.Send(new GetRolesQuery(companyId));
            return Results.Ok(new GetRolesResponse(result.RoleList));
        })
            .WithName("GetRoles")
            .Produces<GetRolesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetRoles")
            .WithDescription("GetRoles")
            .RequireAuthorization(PermissionList.RolesPermissions.View);
    }

    private static Guid GetCompanyId(ClaimsPrincipal user)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(c => c.Type == "company_id")?.Value, out var companyId))
            throw new BadRequestException("Company claim is missing.");

        return companyId;
    }
}
