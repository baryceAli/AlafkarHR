namespace Auth.Users.Features.Authentication.ValidateRefreshToken;

public record RefreshTokenRequest(string RefreshToken);
public record RefreshTokenResponse(string AccessToken,string RefreshToken);
public class RefreshTokenEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/refresh-token", async (RefreshTokenRequest request, ISender sender) =>
        {
            try
            {
                var command = request.Adapt<RefreshTokenCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<RefreshTokenResponse>();
                return Results.Ok(response);
            }
            catch
            {
                return Results.Unauthorized();
            }
        })
            .WithName("RefreshToken")
            .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Refresh Token")
            .WithDescription("Refresh Token")
            .AllowAnonymous();
    }
}
