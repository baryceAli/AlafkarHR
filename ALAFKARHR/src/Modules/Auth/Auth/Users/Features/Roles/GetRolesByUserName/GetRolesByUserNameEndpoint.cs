namespace Auth.Users.Features.Roles.GetRolesByUserName;

public record GetRolesByUserNameRequest(string UserName);
public record GetRolesByUserNameResponse(List<RoleDto> RoleList);
public class GetRolesByUserNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/roles/GetByUserName/{userName}", async (string userName, ISender sender) =>
        {
            var result = await sender.Send(new GetRolesByUserNameQuery(userName));
            return Results.Ok(result.Adapt<GetRolesByUserNameResponse>());
        })
            .WithName("GetRolesByUserName")
            .Produces<GetRolesByUserNameResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetRolesByUserName")
            .WithDescription("GetRolesByUserName")
            .RequireAuthorization(PermissionList.RolesPermissions.View);
    }
}
