namespace Auth.Users.Features.Roles.GetRolesByEmployeeId;

public record GetRolesByEmployeeIdQuery(Guid EmployeeId) : IQuery<GetRolesByEmployeeIdResult>;
public record GetRolesByEmployeeIdResult(List<RoleDto> RoleList);
public class GetRolesByEmployeeIdHandler(RoleManager<ApplicationRole> roleManager)
    : IQueryHandler<GetRolesByEmployeeIdQuery, GetRolesByEmployeeIdResult>
{
    public Task<GetRolesByEmployeeIdResult> Handle(GetRolesByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
