using Auth.Contracts.Features.RegisterUser;

namespace Auth.Users.Features.Authentication.RegisterUser;
public record RegisterUserRequest(RegisterDto Register);
public record RegisterUserResponse(Guid Id);
public class RegisterUserEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/register", async (RegisterUserRequest request, ISender sender) =>
        {
            var command= request.Adapt<RegisterUserCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<RegisterUserResponse>();
            return Results.Ok(response);
        })
            .WithName("RegisterUser")
            .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Register User")
            .WithDescription("Register User");
    }
}
