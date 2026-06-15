using Auth.Users;

namespace Auth.Users.Features.Users.UnassignRoleFromUser;

public record UnassignRoleFromUserCommand(UserRoleDto UserRole) : ICommand<UnassignRoleFromUserResult>;
public record UnassignRoleFromUserResult(bool IsSuccess);
public class UnassignRoleFromUserHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<UnassignRoleFromUserCommand, UnassignRoleFromUserResult>
{
    public async Task<UnassignRoleFromUserResult> Handle(UnassignRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var userName = UserNameKeyNormalizer.Normalize(request.UserRole.UserName);
        if (string.IsNullOrWhiteSpace(userName))
            throw new BadRequestException("User name is required.");

        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
            throw new NotFoundException($"User not found: {userName}");

        var role = await userManager.IsInRoleAsync(user, request.UserRole.RoleName);
        if (!role)
            throw new BadRequestException($"Role ({request.UserRole.RoleName}) is not assigned to user ({userName}).");

        var result = await userManager.RemoveFromRoleAsync(user, request.UserRole.RoleName);

        return new UnassignRoleFromUserResult(result.Succeeded);
    }
}
