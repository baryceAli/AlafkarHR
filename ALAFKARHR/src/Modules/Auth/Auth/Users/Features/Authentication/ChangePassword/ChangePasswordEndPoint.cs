namespace Auth.Users.Features.Authentication.ChangePassword;

public record ChangePasswordRequest(
    string UserIdentifier, 
    string CurrentPassword, 
    string NewPassword, 
    string ConfirmNewPassword);
public record ChangePasswordResponse(bool IsSuccess);
public class ChangePasswordEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/change-password", async (ChangePasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<ChangePasswordCommand>();
            var result = await sender.Send(command);
            return Results.Ok(new ChangePasswordResponse(result.IsSuccess));
        })
            .WithName("ChangePassword")
            .Produces<ChangePasswordResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Change Password")
            .WithDescription("Allows a user to change their password by providing their current password and a new password.");
    }
}
