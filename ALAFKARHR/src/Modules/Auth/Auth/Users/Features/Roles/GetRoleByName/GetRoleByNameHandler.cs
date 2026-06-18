using Shared.Exceptions;

namespace Auth.Users.Features.Roles.GetRoleByName;

public record GetRoleByNameQuery(string RoleName, Guid CompanyId):IQuery<GetRoleByNameResult>;
public record GetRoleByNameResult(RoleDto Role);
public class GetRoleByNameHandler(RoleManager<ApplicationRole> roleManager)
    : IQueryHandler<GetRoleByNameQuery, GetRoleByNameResult>
{
    public async Task<GetRoleByNameResult> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
    {
        var role= await roleManager.FindByNameAsync(request.RoleName);
        if (role is null)
            throw new NotFoundException($"Role not found: {request.RoleName}");
        if (role.CompanyId != request.CompanyId)
            throw new BadRequestException("Role does not belong to the selected company.");

        var claims = await roleManager.GetClaimsAsync(role);
        List<string> permissions = [];
        foreach(var claim in claims)
        {
            permissions.Add(claim.Value);
        }

        return new GetRoleByNameResult(new RoleDto
        {
            RoleName = role.Name ?? string.Empty,
            DisplayName = string.IsNullOrWhiteSpace(role.DisplayName) ? role.Name ?? string.Empty : role.DisplayName,
            CompanyId = role.CompanyId,
            Permissions = permissions
        });
    }
}
