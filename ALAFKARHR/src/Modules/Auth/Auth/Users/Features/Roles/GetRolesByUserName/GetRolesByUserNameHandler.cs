using Auth.Users;

namespace Auth.Users.Features.Roles.GetRolesByUserName;

public record GetRolesByUserNameQuery(string UserName) : IQuery<GetRolesByUserNameResult>;
public record GetRolesByUserNameResult(List<RoleDto> RoleList);
public class GetRolesByUserNameHandler(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    : IQueryHandler<GetRolesByUserNameQuery, GetRolesByUserNameResult>
{
    public async Task<GetRolesByUserNameResult> Handle(GetRolesByUserNameQuery request, CancellationToken cancellationToken)
    {
        var userName = UserNameKeyNormalizer.Normalize(request.UserName);
        if (string.IsNullOrWhiteSpace(userName))
            throw new BadRequestException("User name is required.");

        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
            throw new NotFoundException($"User not found: {userName}");

        var roles = await userManager.GetRolesAsync(user);

        var roleList = new List<RoleDto>();

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) continue;
            if (role.CompanyId != user.CompanyId) continue;

            var claims = await roleManager.GetClaimsAsync(role);

            roleList.Add(new RoleDto
            {
                CompanyId = user.CompanyId,
                RoleName = role.Name ?? string.Empty,
                DisplayName = string.IsNullOrWhiteSpace(role.DisplayName) ? role.Name ?? string.Empty : role.DisplayName,
                Permissions = claims.Select(c => c.Value).ToList()
            });
        }

        return new GetRolesByUserNameResult(roleList);
    }
}
