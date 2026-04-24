using Shared.Pagination;

namespace Auth.Users.Features.Roles.GetRoles;

public record GetRolesQuery(Guid companyId):IQuery<GetRolesResult>;
public record GetRolesResult(List<RoleDto> RoleList);
public class GetRolesHandler(RoleManager<ApplicationRole> roleManager)
    : IQueryHandler<GetRolesQuery, GetRolesResult>
{
    public async Task<GetRolesResult> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var rolesDb= await roleManager.Roles.ToListAsync(cancellationToken);
        var roles= rolesDb.Where(r => r.CompanyId==request.companyId).ToList();
        List<RoleDto> rolesResult = new List<RoleDto>();
        foreach (var role in roles)
        {
            rolesResult.Add(new RoleDto { RoleName = role.Name });
        }

        return new GetRolesResult(rolesResult);
    }
}
