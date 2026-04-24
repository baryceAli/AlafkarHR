using System.Data;
using System.Security;

namespace Auth.Users.Features.Roles.CreateRole;

public record CreateRoleCommand(RoleDto Role) : ICommand<CreateRoleResult>;
public record CreateRoleResult(RoleDto CreatedRole);
public class CreateRoleHanlder(RoleManager<ApplicationRole> roleManager)
    : ICommandHandler<CreateRoleCommand, CreateRoleResult>
{
    public async Task<CreateRoleResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.RoleExistsAsync(request.Role.RoleName);
        //= await roleManager.FindByNameAsync(request.Role.RoleName);
        if (existingRole)
            throw new Exception($"Role ({request.Role.RoleName}) is already exist");


        var identityRole =
            new ApplicationRole() { Name=request.Role.RoleName, CompanyId=request.Role.CompanyId.Value};

        var result = await roleManager.CreateAsync(identityRole);
        ApplicationRole? createdRole;
        if (result.Succeeded)
        {
            createdRole=await roleManager.FindByNameAsync(request.Role.RoleName);
            if (createdRole != null)
            {
                // add claims
                foreach (var perm in request.Role.Permissions)
                {
                    await roleManager.AddClaimAsync(createdRole, new Claim("Permission", perm));

                }
            }
        }
        else
        {
            throw new Exception($"Couldn't create role: {request.Role.RoleName}");
        }

        var claims =await roleManager.GetClaimsAsync(identityRole);

        List<string> permissions = [];
        foreach (var claim in claims)
        {
            permissions.Add(claim.Value);
        }

        


        return new CreateRoleResult(new RoleDto
        {
            RoleName=createdRole.Name,
            Permissions = permissions
        });


    }
}
