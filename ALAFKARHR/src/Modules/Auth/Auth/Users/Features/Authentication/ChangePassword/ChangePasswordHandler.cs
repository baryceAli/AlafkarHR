namespace Auth.Users.Features.Authentication.ChangePassword;

public record ChangePasswordCommand(string UserIdentifier, string CurrentPassword, string NewPassword, string ConfirmNewPassword) : ICommand<ChangePasswordResult>;
public record ChangePasswordResult(bool IsSuccess);

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserIdentifier).NotEmpty().WithMessage("UserIdentifier is required");
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("CurrentPassword is required");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("NewPassword is required");
        RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage("ConfirmNewPassword is required")
            .Equal(x => x.NewPassword).WithMessage("ConfirmNewPassword must match NewPassword");
    }
}
public class ChangePasswordHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<ChangePasswordCommand, ChangePasswordResult>
{
    public async Task<ChangePasswordResult> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user= command.UserIdentifier.Contains("@")?
            await userManager.FindByEmailAsync(command.UserIdentifier)
            : await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == command.UserIdentifier, cancellationToken);
        if(user is null)
            throw new Exception($"User not found: {command.UserIdentifier}");

        var result = await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);

        if(result is null || !result.Succeeded) 
            throw new Exception($"Couldn't change password for user: {command.UserIdentifier}");

        return new ChangePasswordResult(result.Succeeded);
    }
}
