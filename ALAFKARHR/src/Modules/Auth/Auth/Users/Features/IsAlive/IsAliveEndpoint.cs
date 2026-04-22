namespace Auth.Users.Features.IsAlive;

public class IsAliveEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async () =>
        {
            return Results.Ok("Alafkar API is Alive");
        })
            .WithName("IsAlive")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("IsAlive")
            .WithDescription("IsAlive");
    }
}
