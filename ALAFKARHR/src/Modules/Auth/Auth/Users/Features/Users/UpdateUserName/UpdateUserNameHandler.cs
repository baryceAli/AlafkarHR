using Auth.Contracts.Features.UpdateUserName;

namespace Auth.Users.Features.Users.UpdateUserName;

public class UpdateUserNameCommandValidator : AbstractValidator<UpdateUserNameCommand>
{
    public UpdateUserNameCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required");
        RuleFor(x => x.OldUserName).NotEmpty().WithMessage("Old username is required");
        RuleFor(x => x.NewUserName).NotEmpty().WithMessage("New username is required");
    }
}

public class UpdateUserNameHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<UpdateUserNameCommand, UpdateUserNameResult>
{
    public async Task<UpdateUserNameResult> Handle(UpdateUserNameCommand command, CancellationToken cancellationToken)
    {
        var oldUserName = UserNameKeyNormalizer.Normalize(command.OldUserName);
        var newUserName = UserNameKeyNormalizer.Normalize(command.NewUserName);

        if (string.Equals(oldUserName, newUserName, StringComparison.Ordinal))
        {
            return new UpdateUserNameResult(true);
        }

        var user = await userManager.FindByNameAsync(oldUserName)
            ?? throw new InvalidOperationException("Linked user account was not found.");

        if (user.CompanyId != command.CompanyId)
        {
            throw new InvalidOperationException("Linked user account does not belong to this company.");
        }

        var existingUser = await userManager.FindByNameAsync(newUserName);
        if (existingUser is not null && existingUser.Id != user.Id)
        {
            throw new InvalidOperationException("Employee code is already used as a username.");
        }

        var result = await userManager.SetUserNameAsync(user, newUserName);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(error => error.Description)));
        }

        return new UpdateUserNameResult(true);
    }
}
