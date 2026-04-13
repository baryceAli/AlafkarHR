using Shared.Contracts.Messaging;

namespace Auth.Users.Features.ConfirmOTP;

public record ConfirmOTPCommand(string UserIdentifier, string OTP, bool IsEmail) : ICommand<ConfirmOTPResult>;
public record ConfirmOTPResult(bool IsConfirmed);

public class ConfirmOTPCommandValidator : AbstractValidator<ConfirmOTPCommand>
{
    public ConfirmOTPCommandValidator()
    {
        RuleFor(x => x.UserIdentifier)
            .NotEmpty().WithMessage("User identifier is required.");
        RuleFor(x => x.OTP)
            .NotEmpty().WithMessage("OTP is required.");
    }
}
public class ConfirmOTPHandler(UserManager<ApplicationUser> userManager, IMessageSender messageSender)
    : ICommandHandler<ConfirmOTPCommand, ConfirmOTPResult>
{
    public async Task<ConfirmOTPResult> Handle(ConfirmOTPCommand request, CancellationToken cancellationToken)
    {
        var user = request.IsEmail
            ? userManager.Users.FirstOrDefault(u => u.Email == request.UserIdentifier)
            : userManager.Users.FirstOrDefault(u => u.PhoneNumber == request.UserIdentifier);

        if (user is null)
            throw new Exception($"User not found: {request.UserIdentifier}");


        user.ConfirmOtp(request.OTP);

        if (user.OtpType == OTPType.ConfirmEmail && request.IsEmail)
        {
            user.EmailConfirmed = true;
        }
        else if (user.OtpType == OTPType.ConfirmPhone && !request.IsEmail)
        {
            user.PhoneNumberConfirmed = true;
        }
        if (user.OtpType == OTPType.ResetPassword)
        {
            var prt = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, prt, "pass@1234");

            await messageSender.SendAsync(user!.PhoneNumber!, "pass@1234");
            await messageSender.SendAsync(user!.Email!, "New Password", "pass@1234");
            //user.
        }

        await userManager.UpdateAsync(user);
        return new ConfirmOTPResult(user.IsOtpConfirmed ?? false);
    }
}
