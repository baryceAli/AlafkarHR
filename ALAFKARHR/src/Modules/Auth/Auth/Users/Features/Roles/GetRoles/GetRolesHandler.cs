using Shared.Pagination;

namespace Auth.Users.Features.Roles.GetRoles;

public record GetRolesQuery(Guid companyId):IQuery<GetRolesResult>;
public record GetRolesResult(List<RoleDto> RoleList);
public class GetRolesHandler(RoleManager<ApplicationRole> roleManager)
    : IQueryHandler<GetRolesQuery, GetRolesResult>
{
    public async Task<GetRolesResult> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        await CompanyRoleTemplates.EnsurePublicWebsiteRolesAsync(roleManager, request.companyId);

        var roles = await roleManager.Roles
            .Where(r => r.CompanyId == request.companyId)
            .ToListAsync(cancellationToken);
        List<RoleDto> rolesResult = new List<RoleDto>();
        foreach (var role in roles)
        {
            rolesResult.Add(new RoleDto
            {
                RoleName = role.Name ?? string.Empty,
                DisplayName = string.IsNullOrWhiteSpace(role.DisplayName) ? role.Name ?? string.Empty : role.DisplayName,
                CompanyId = role.CompanyId,
                TemplateKey = role.TemplateKey
            });
        }

        return new GetRolesResult(rolesResult);
    }
}
