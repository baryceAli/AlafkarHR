using Auth.Users;

namespace Auth.Users.Features.Users.AssignRoleToUser;

public record AssignRoleToUserCommand(UserRoleDto UserRole) : ICommand<AssignRoleToUserResult>;
public record AssignRoleToUserResult(bool IsSuccess);
public class AssignRoleToUserHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
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
        if (!user.CompanyId.HasValue)
            throw new BadRequestException("Platform users cannot be assigned tenant roles from this page.");

        if (await IsProtectedAdmin(user))
            throw new BadRequestException("Admin role assignments cannot be changed from this page.");

        var role = await roleManager.FindByNameAsync(request.UserRole.RoleName);
        if (role is null)
            throw new NotFoundException($"Role not found: {request.UserRole.RoleName}");

        if (role.CompanyId != user.CompanyId)
            throw new BadRequestException("Cannot assign a role from another company.");

        if (IsCompanyAdminRole(user, role.Name))
            throw new BadRequestException("Admin role assignments cannot be changed from this page.");

        await CompanyRoleTemplates.SeedDefaultRolesAsync(roleManager, user.CompanyId.Value);

        role = await ResolveRoleAfterTemplateSync(role, user.CompanyId.Value, cancellationToken);
        if (role is null)
            throw new NotFoundException($"Role not found: {request.UserRole.RoleName}");

        var isExist = await userManager.IsInRoleAsync(user, role.Name!);
        if (isExist)
            throw new BadRequestException($"Role ({role.DisplayName}) is already assigned to user ({userName}).");

        var result = await userManager.AddToRoleAsync(user, role.Name!);

        return new AssignRoleToUserResult(result.Succeeded);
    }

    private async Task<ApplicationRole?> ResolveRoleAfterTemplateSync(
        ApplicationRole role,
        Guid companyId,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(role.TemplateKey))
        {
            return await roleManager.Roles
                .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.TemplateKey == role.TemplateKey, cancellationToken);
        }

        return await roleManager.FindByIdAsync(role.Id.ToString());
    }

    private async Task<bool> IsProtectedAdmin(ApplicationUser user)
    {
        var normalizedUserName = UserNameKeyNormalizer.Normalize(user.UserName ?? string.Empty);
        if (string.Equals(normalizedUserName, "admin", StringComparison.OrdinalIgnoreCase))
            return true;

        if (!user.CompanyId.HasValue)
            return true;

        var companyAdminRoleName = CompanyRoleTemplates.BuildSystemAdminRoleName(user.CompanyId.Value);
        return await userManager.IsInRoleAsync(user, companyAdminRoleName);
    }

    private static bool IsCompanyAdminRole(ApplicationUser user, string? roleName)
    {
        if (!user.CompanyId.HasValue)
            return false;

        var companyAdminRoleName = CompanyRoleTemplates.BuildSystemAdminRoleName(user.CompanyId.Value);
        return string.Equals(roleName, companyAdminRoleName, StringComparison.OrdinalIgnoreCase);
    }
}
