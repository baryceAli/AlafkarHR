namespace Auth.Users.Features.ConfirmOTP;

public record ConfirmOTPRequest(string UserIdentifier, string OTP);
public record ConfirmOTPResponse(bool IsConfirmed);
public class ConfirmOTPEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/confirm-otp", async (ConfirmOTPRequest request, ISender sender) =>
        {
            var command = new ConfirmOTPCommand(request.UserIdentifier, request.OTP, request.UserIdentifier.Contains("@"));
            var result = await sender.Send(command);
            return Results.Ok(new ConfirmOTPResponse(result.IsConfirmed));
        })
            .WithName("ConfirmOTP")
            .Produces<ConfirmOTPResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Confirm OTP")
            .WithDescription("Confirm OTP");
    }
}
