using System.Data;
using System.Security;

namespace Auth.Users.Features.Roles.CreateRole;

public record CreateRoleCommand(RoleDto Role) : ICommand<CreateRoleResult>;
public record CreateRoleResult(Guid Id);
public class CreateRoleHanlder(RoleManager<ApplicationRole> roleManager)
    : ICommandHandler<CreateRoleCommand, CreateRoleResult>
{
    public async Task<CreateRoleResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.RoleExistsAsync(request.Role.RoleName);
        //= await roleManager.FindByNameAsync(request.Role.RoleName);
        if (!existingRole)
        {
            //Create role
            var identityRole =
                new ApplicationRole() { Name = request.Role.RoleName, CompanyId = request.Role.CompanyId.Value };
            var result = await roleManager.CreateAsync(identityRole);


        }

        ApplicationRole? createdRole;
        createdRole = await roleManager.FindByNameAsync(request.Role.RoleName);

        if (createdRole == null)
        {
            throw new Exception($"Couldn't find the role: {request.Role.RoleName}");
            //delete policies for the role
        }
        else
        {
            var claims = await roleManager.GetClaimsAsync(createdRole);
            foreach (var c in claims)
            {
                await roleManager.RemoveClaimAsync(createdRole, c);
            }

            // add claims
            foreach (var perm in request.Role.Permissions)
            {
                await roleManager.AddClaimAsync(createdRole, new Claim("Permission", perm));

            }
        }

        var currentClaims = await roleManager.GetClaimsAsync(createdRole);

        List<string> permissions = [];
        foreach (var claim in currentClaims)
        {
            permissions.Add(claim.Value);
        }

        return new CreateRoleResult(createdRole.Id);
    }
}
