using Microsoft.AspNetCore.Mvc;

namespace Auth.Users.Features.Roles.DeleteRole;


public record DeleteRoleResponse(bool IsSuccess);
public class DeleteRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/auth/roles/{roleName}", async ([FromRoute] string roleName, ClaimsPrincipal user, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new DeleteRoleCommand(roleName, GetCompanyId(user)));

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

    private static Guid GetCompanyId(ClaimsPrincipal user)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(c => c.Type == "company_id")?.Value, out var companyId))
            throw new BadRequestException("Company claim is missing.");

        return companyId;
    }
}
