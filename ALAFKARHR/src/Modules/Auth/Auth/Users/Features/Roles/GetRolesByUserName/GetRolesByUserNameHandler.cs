namespace Auth.Users.Features.Roles.GetRolesByUserName;

public record GetRolesByUserNameQuery(string UserName) : IQuery<GetRolesByUserNameResult>;
public record GetRolesByUserNameResult(List<RoleDto> RoleList);
public class GetRolesByUserNameHandler(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    : IQueryHandler<GetRolesByUserNameQuery, GetRolesByUserNameResult>
{
    public async Task<GetRolesByUserNameResult> Handle(GetRolesByUserNameQuery request, CancellationToken cancellationToken)
    {
        //find user
        var usr = await userManager.FindByEmailAsync("baryce_1@hotmail.com");
        var res=await userManager.SetUserNameAsync(usr, "Bashir");
        if (res.Succeeded)
        {
            var intval = 0;
        }
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user is null)
            throw new NotFoundException($"User not found: {request.UserName}");


        //find user's roles
        var roles = await userManager.GetRolesAsync(user);

        var roleList = new List<RoleDto>();

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) continue;

            var claims = await roleManager.GetClaimsAsync(role);

            roleList.Add(new RoleDto
            {
                CompanyId = user.CompanyId,
                RoleName = roleName,
                Permissions = claims.Select(c => c.Value).ToList()
            });
        }

        return new GetRolesByUserNameResult(roleList);
        //return value
    }
}
