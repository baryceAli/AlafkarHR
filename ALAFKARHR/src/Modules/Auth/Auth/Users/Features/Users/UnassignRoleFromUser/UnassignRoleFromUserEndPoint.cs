namespace Auth.Users.Features.Users.UnassignRoleFromUser;

public record UnassignRoleFromUserRequest(UserRoleDto UserRole);
public record UnassignRoleFromUserResponse(bool IsSuccess);
public class UnassignRoleFromUserEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/users/unassignRole", async (UnassignRoleFromUserRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UnassignRoleFromUserCommand>());
            return Results.Ok(result.Adapt<UnassignRoleFromUserResponse>());
        })
            .WithName("UnassignRoleFromUser")
            .Produces<UnassignRoleFromUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UnassignRoleFromUser")
            .WithDescription("UnassignRoleFromUser")
            .RequireAuthorization(PermissionList.UsersPermissions.Delete);
    }
}
