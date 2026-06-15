using Auth.Users;

namespace Auth.Users.Features.Users.AssignRoleToUser;

public record AssignRoleToUserCommand(UserRoleDto UserRole) : ICommand<AssignRoleToUserResult>;
public record AssignRoleToUserResult(bool IsSuccess);
public class AssignRoleToUserHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<AssignRoleToUserCommand, AssignRoleToUserResult>
{
    public async Task<AssignRoleToUserResult> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var userName = UserNameKeyNormalizer.Normalize(request.UserRole.UserName);
        if (string.IsNullOrWhiteSpace(userName))
            throw new BadRequestException("User name is required.");

        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
            throw new NotFoundException($"User not found: {userName}");

        var isExist = await userManager.IsInRoleAsync(user, request.UserRole.RoleName);
        if (isExist)
            throw new BadRequestException($"Role ({request.UserRole.RoleName}) is already assigned to user ({userName}).");

        var result = await userManager.AddToRoleAsync(user, request.UserRole.RoleName);

        return new AssignRoleToUserResult(result.Succeeded);
    }
}
