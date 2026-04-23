//using Auth.Users.Features.ResetPassword;


//using Auth.Users.Features.ResetPassword;


namespace Auth.Users.Features.Authentication.GenerateResetPasswordOTP;

public record GenerateResetPasswordOTPRequest(string UserIdentifier);
public record GenerateResetPasswordOTPResponse(bool IsSuccess);
public class GenerateResetPasswordOTPEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/reset-password", async (GenerateResetPasswordOTPRequest request, ISender sender) =>
        {
            var command = new GenerateResetPasswordOTPCommand(request.UserIdentifier, request.UserIdentifier.Contains("@"));
            var result = await sender.Send(command);
            return Results.Ok(new GenerateResetPasswordOTPResponse(result.IsSuccess));
        })
            .WithName("GenerateResetPassword")
            .Produces<GenerateResetPasswordOTPResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Generates a reset password OTP and sends it to the user")
            .WithDescription("This endpoint generates a one-time password (OTP) for resetting the user's password and sends it to the user. The user can then use this OTP to reset their password.");


        //    app.MapPost("/api/v1/catalog/auth/reset-password-phone", async (GenerateResetPasswordOTPRequest request, ISender sender) =>
        //    {
        //        var command = new GenerateResetPasswordOTPCommand(request.UserIdentifier,false);
        //        var result = await sender.Send(command);
        //        return Results.Ok(new GenerateResetPasswordOTPResponse(result.IsSuccess));
        //    })
        //        .WithName("GenerateResetPasswordByPhone")
        //        .Produces<GenerateResetPasswordOTPResponse>(StatusCodes.Status200OK)
        //        .ProducesProblem(StatusCodes.Status400BadRequest)
        //        .ProducesProblem(StatusCodes.Status404NotFound)
        //        .WithSummary("Generates a reset password OTP and sends it to the user's phone.")
        //        .WithDescription("This endpoint generates a one-time password (OTP) for resetting the user's password and sends it to the user's registered phone address. The user can then use this OTP to reset their password.");
    }
}
