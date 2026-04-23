using Microsoft.AspNetCore.Mvc;

namespace Auth.Users.Features.Roles.GetRoles;


public record GetRolesResponse(List<RoleDto> RoleList);
public class GetRolesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/roles/{companyId}", async ([FromRoute] Guid companyId, ISender sender) =>
        {
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
}
