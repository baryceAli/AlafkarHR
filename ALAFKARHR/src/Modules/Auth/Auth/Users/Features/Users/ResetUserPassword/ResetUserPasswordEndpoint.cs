using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auth.Users.Features.Users.ResetUserPassword;

public record ResetUserPasswordRequest(string TemporaryPassword);
public record ResetUserPasswordResponse(bool IsSuccess);

public class ResetUserPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/users/{userName}/reset-password", async (
            [FromRoute] string userName,
            ResetUserPasswordRequest request,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var result = await sender.Send(new ResetUserPasswordCommand(userName, request.TemporaryPassword, user));
            return Results.Ok(result.Adapt<ResetUserPasswordResponse>());
        })
            .WithName("ResetUserPassword")
            .Produces<ResetUserPasswordResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Reset user password")
            .WithDescription("Reset a normal company user's password using an admin-entered temporary password.")
            .RequireAuthorization(PermissionList.UsersPermissions.Edit);
    }
}
