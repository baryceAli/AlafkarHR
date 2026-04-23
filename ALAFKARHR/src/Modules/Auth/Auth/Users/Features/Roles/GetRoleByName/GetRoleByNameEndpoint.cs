namespace Auth.Users.Features.Roles.GetRoleByName;

public record GetRoleByNameResponse(RoleDto Role);
public class GetRoleByNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/roles/{roleName}", async (string roleName, ISender sender) =>
        {
            var result = await sender.Send(new GetRoleByNameQuery(roleName));
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
}
