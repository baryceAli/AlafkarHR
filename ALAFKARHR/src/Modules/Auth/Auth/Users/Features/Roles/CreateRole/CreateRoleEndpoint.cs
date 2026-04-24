namespace Auth.Users.Features.Roles.CreateRole;


public record CreateRoleRequest(RoleDto Role);
public record CreateRoleResponse(Guid Id);
public class CreateRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/roles", async (CreateRoleRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateRoleCommand>());
            return Results.Ok(result.Adapt<CreateRoleResponse>());
        })
            .WithName("CreateRole")
            .Produces<CreateRoleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            //.ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("CreateRole")
            .WithDescription("CreateRole")
            .RequireAuthorization(PermissionList.RolesPermissions.Create);
    }
}
