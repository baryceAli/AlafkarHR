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

        if (await IsProtectedAdmin(user))
            throw new BadRequestException("Admin role assignments cannot be changed from this page.");

        if (IsCompanyAdminRole(user, request.UserRole.RoleName))
            throw new BadRequestException("Admin role assignments cannot be changed from this page.");

        var isExist = await userManager.IsInRoleAsync(user, request.UserRole.RoleName);
        if (isExist)
            throw new BadRequestException($"Role ({request.UserRole.RoleName}) is already assigned to user ({userName}).");

        var result = await userManager.AddToRoleAsync(user, request.UserRole.RoleName);

        return new AssignRoleToUserResult(result.Succeeded);
    }

    private async Task<bool> IsProtectedAdmin(ApplicationUser user)
    {
        var normalizedUserName = UserNameKeyNormalizer.Normalize(user.UserName ?? string.Empty);
        if (string.Equals(normalizedUserName, "admin", StringComparison.OrdinalIgnoreCase))
            return true;

        var companyAdminRoleName = $"SystemAdmin-{user.CompanyId:N}";
        return await userManager.IsInRoleAsync(user, companyAdminRoleName);
    }

    private static bool IsCompanyAdminRole(ApplicationUser user, string roleName)
    {
        var companyAdminRoleName = $"SystemAdmin-{user.CompanyId:N}";
        return string.Equals(roleName, companyAdminRoleName, StringComparison.OrdinalIgnoreCase);
    }
}
