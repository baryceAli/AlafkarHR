using Auth.Helpers;
using Shared.Contracts.Messaging;

namespace Auth.Users.Features.Authentication.GenerateResetPasswordOTP;


public record GenerateResetPasswordOTPCommand(string UserIdentifier, bool IsEmail) : ICommand<GenerateResetPasswordOTPResult>;
public record GenerateResetPasswordOTPResult(bool IsSuccess);

public class GenerateResetPasswordOTPCommandValidator : AbstractValidator<GenerateResetPasswordOTPCommand>
{
    public GenerateResetPasswordOTPCommandValidator()
    {
        RuleFor(x => x.UserIdentifier).NotEmpty().WithMessage("UserIdentifier is required");
    }
}
public class GenerateResetPasswordOTPHanlder(AuthDbContext dbContext, UserManager<ApplicationUser> userManager, IOptions<OTPOptions> oTPOptions, IMessageSender emailSender)
    : ICommandHandler<GenerateResetPasswordOTPCommand, GenerateResetPasswordOTPResult>
{
    public async Task<GenerateResetPasswordOTPResult> Handle(GenerateResetPasswordOTPCommand command, CancellationToken cancellationToken)
    {

        var user = command.IsEmail ?
            await userManager.FindByEmailAsync(command.UserIdentifier)
            : await dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == command.UserIdentifier);

        if (user is null)
            throw new Exception($"User not found: {command.UserIdentifier}");

        var otp = GenerateOTP.Generate(oTPOptions.Value.Length);
        user.UpdateOtp(otp, OTPType.ResetPassword, DateTime.UtcNow.AddMinutes(oTPOptions.Value.ExpirationMinutes), false);


        if (command.IsEmail)
        {
            await emailSender.SendAsync(user!.Email!, "Password Reset OTP", $"{otp}");

        }
        else
        {
            await emailSender.SendAsync(user!.PhoneNumber!, $"{otp}");

        }

        return new GenerateResetPasswordOTPResult(true);

    }
}
