using Auth.Users;

namespace Auth.Users.Features.Roles.GetRolesByUserName;

public record GetRolesByUserNameQuery(string UserName) : IQuery<GetRolesByUserNameResult>;
public record GetRolesByUserNameResult(List<RoleDto> RoleList);
public class GetRolesByUserNameHandler(AuthDbContext dbContext, UserManager<ApplicationUser> userManager)
    : IQueryHandler<GetRolesByUserNameQuery, GetRolesByUserNameResult>
{
    public async Task<GetRolesByUserNameResult> Handle(GetRolesByUserNameQuery request, CancellationToken cancellationToken)
    {
        var userName = UserNameKeyNormalizer.Normalize(request.UserName);
        if (string.IsNullOrWhiteSpace(userName))
            throw new BadRequestException("User name is required.");

        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByNameAsync(userName);
        cancellationToken.ThrowIfCancellationRequested();
        if (user is null)
            throw new NotFoundException($"User not found: {userName}");

        var roles = await (
            from userRole in dbContext.UserRoles.AsNoTracking()
            join role in dbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
            where userRole.UserId == user.Id && role.CompanyId == user.CompanyId
            select new { role.Id, role.Name, role.DisplayName })
            .ToListAsync(cancellationToken);

        if (roles.Count == 0)
            return new GetRolesByUserNameResult([]);

        var roleIds = roles.Select(role => role.Id).ToList();
        var claims = await dbContext.RoleClaims.AsNoTracking()
            .Where(claim => roleIds.Contains(claim.RoleId))
            .Select(claim => new { claim.RoleId, claim.ClaimValue })
            .ToListAsync(cancellationToken);

        var claimsByRoleId = claims.ToLookup(claim => claim.RoleId, claim => claim.ClaimValue ?? string.Empty);

        var roleList = roles
            .Select(role => new RoleDto
            {
                CompanyId = user.CompanyId,
                RoleName = role.Name ?? string.Empty,
                DisplayName = string.IsNullOrWhiteSpace(role.DisplayName) ? role.Name ?? string.Empty : role.DisplayName,
                Permissions = claimsByRoleId[role.Id].ToList()
            })
            .ToList();

        return new GetRolesByUserNameResult(roleList);
    }
}
