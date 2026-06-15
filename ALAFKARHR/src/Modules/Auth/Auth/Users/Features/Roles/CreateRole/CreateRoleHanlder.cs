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
        if (request.Role.CompanyId is null || request.Role.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");

        var knownPermissions = PermissionList.GetAll().ToHashSet(StringComparer.Ordinal);
        var requestedPermissions = (request.Role.Permissions ?? [])
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var invalidPermissions = requestedPermissions
            .Where(p => !knownPermissions.Contains(p))
            .ToList();

        if (invalidPermissions.Count > 0)
            throw new BadRequestException($"Unknown permission(s): {string.Join(", ", invalidPermissions)}");

        var existingRole = await roleManager.RoleExistsAsync(request.Role.RoleName);
        //= await roleManager.FindByNameAsync(request.Role.RoleName);
        if (!existingRole)
        {
            //Create role
            var identityRole =
                new ApplicationRole() { Name = request.Role.RoleName, CompanyId = request.Role.CompanyId.Value };
            var result = await roleManager.CreateAsync(identityRole);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

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
            foreach (var c in claims.Where(c => c.Type == "Permission"))
            {
                await roleManager.RemoveClaimAsync(createdRole, c);
            }

            // add claims
            foreach (var perm in requestedPermissions)
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
