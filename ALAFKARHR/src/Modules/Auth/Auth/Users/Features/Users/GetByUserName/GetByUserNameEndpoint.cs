using Auth.Contracts.Features.GetByUserName;
using SharedWithUI.Auth.Dtos;

namespace Auth.Users.Features.Users.GetByUserName;

public record GetByUserNameRequest(string UserName);
public record GetByUserNameResponse(UserDto User);

public class GetByUserNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/users/GetByUserName/{userName}", async (string userName, ISender sender) =>
        {
            var result = await sender.Send(new GetByUserNameQuery(userName));
            return Results.Ok(result.Adapt<GetByUserNameResponse>());
        })
            .WithName("GetByUserName")
            .Produces<GetByUserNameResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetByUserName")
            .WithDescription("Get user by user name")
            .RequireAuthorization(PermissionList.UsersPermissions.View);
    }
}
