namespace Auth.Users.Features.Authentication.Login;

//public record LoginRequest(string Email, string Password);
public record LoginRequest(LoginDto Login);
//public record LoginResponse(string AccessToken, string RefreshToken);
public record LoginResponse(LoginResponseDto Login);
public class LoginEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/login", async (LoginRequest request, ISender sender) =>
        {
            try
            {
                if (request?.Login == null)
                    return Results.BadRequest("Login object is null");

                if (string.IsNullOrWhiteSpace(request.Login.Email))
                    return Results.BadRequest("Email is required");

                if (string.IsNullOrWhiteSpace(request.Login.Password))
                    return Results.BadRequest("Password is required");

                var command = request.Adapt<LoginCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<LoginResponse>();

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.ToString());
            }
        })
            .WithName("Login")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Login")
            .WithDescription("Login");
    }
}
