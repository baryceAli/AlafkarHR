namespace Auth.Users.Features.Users.AssignRoleToUser;

public record AssignRoleToUserRequest(UserRoleDto UserRole);
public record AssignRoleToUserResponse(bool IsSuccess);
public class AssignRoleToUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/Users/AssignRole", async (AssignRoleToUserRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<AssignRoleToUserCommand>());
            return Results.Ok(result.Adapt<AssignRoleToUserResponse>());
        })
            .WithName("AssignRolesToUser")
            .Produces<AssignRoleToUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("AssignRolesToUser")
            .WithDescription("AssignRolesToUser")
            .RequireAuthorization(PermissionList.UsersPermissions.Create);
    }
}
