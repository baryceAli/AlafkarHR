using Microsoft.AspNetCore.Mvc;

namespace Auth.Users.Features.Roles.DeleteRole;


public record DeleteRoleResponse(bool IsSuccess);
public class DeleteRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/auth/roles/{roleName}", async ([FromRoute] string roleName, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new DeleteRoleCommand(roleName));

            return Results.Ok(new DeleteRoleResponse(result.IsSuccess));
        })
            .WithName("DeleteRole")
            .Produces<DeleteRoleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeleteRole")
            .WithDescription("DeleteRole")
            .RequireAuthorization(PermissionList.RolesPermissions.Delete);
    }
}
