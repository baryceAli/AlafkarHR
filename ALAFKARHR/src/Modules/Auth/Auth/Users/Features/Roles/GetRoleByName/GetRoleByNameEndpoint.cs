namespace Auth.Users.Features.Roles.GetRoleByName;

public record GetRoleByNameResponse(RoleDto Role);
public class GetRoleByNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/roles/{roleName}", async (string roleName, ClaimsPrincipal user, ISender sender) =>
        {
            var result = await sender.Send(new GetRoleByNameQuery(roleName, GetCompanyId(user)));
            return Results.Ok(new GetRoleByNameResponse(result.Role));
        })
            .WithName("GetRoleByName")
            .Produces<GetRoleByNameResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetRoleByName")
            .WithDescription("GetRoleByName")
            .RequireAuthorization(PermissionList.RolesPermissions.View);
    }

    private static Guid GetCompanyId(ClaimsPrincipal user)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(c => c.Type == "company_id")?.Value, out var companyId))
            throw new BadRequestException("Company claim is missing.");

        return companyId;
    }
}
