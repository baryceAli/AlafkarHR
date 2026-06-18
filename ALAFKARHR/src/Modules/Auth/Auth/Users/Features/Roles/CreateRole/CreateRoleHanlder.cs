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

        var companyId = request.Role.CompanyId.Value;
        var submittedRoleName = request.Role.RoleName?.Trim();
        var displayName = !string.IsNullOrWhiteSpace(request.Role.DisplayName)
            ? request.Role.DisplayName.Trim()
            : submittedRoleName;

        if (string.IsNullOrWhiteSpace(displayName))
            throw new BadRequestException("Role name is required.");

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

        var isEdit = !string.IsNullOrWhiteSpace(submittedRoleName);
        var role = isEdit
            ? await roleManager.FindByNameAsync(submittedRoleName!)
            : null;

        if (role is not null && role.CompanyId != companyId)
            throw new BadRequestException("Role does not belong to the selected company.");

        var duplicateDisplayName = await roleManager.Roles
            .AnyAsync(r => r.CompanyId == companyId
                && r.DisplayName == displayName
                && (role == null || r.Id != role.Id), cancellationToken);

        if (duplicateDisplayName)
            throw new BadRequestException($"Role ({displayName}) already exists for this company.");

        if (role is null)
        {
            var internalName = await BuildUniqueRoleName(companyId, displayName, cancellationToken);
            role = new ApplicationRole
            {
                Name = internalName,
                DisplayName = displayName,
                CompanyId = companyId
            };

            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            role = await roleManager.FindByNameAsync(internalName)
                ?? throw new Exception($"Couldn't find the role: {internalName}");
        }
        else if (!string.Equals(role.DisplayName, displayName, StringComparison.Ordinal))
        {
            role.DisplayName = displayName;
            var updateResult = await roleManager.UpdateAsync(role);
            if (!updateResult.Succeeded)
                throw new BadRequestException(string.Join(", ", updateResult.Errors.Select(e => e.Description)));
        }

        await CompanyRoleTemplates.SyncPermissionClaimsAsync(roleManager, role, requestedPermissions, removeObsolete: true);

        return new CreateRoleResult(role.Id);
    }

    private async Task<string> BuildUniqueRoleName(Guid companyId, string displayName, CancellationToken cancellationToken)
    {
        var slug = CompanyRoleTemplates.NormalizeSlug(displayName);
        var roleName = CompanyRoleTemplates.BuildCompanyRoleName(companyId, slug);
        var suffix = 2;

        while (await roleManager.Roles.AnyAsync(r => r.Name == roleName, cancellationToken))
        {
            roleName = CompanyRoleTemplates.BuildCompanyRoleName(companyId, $"{slug}-{suffix}");
            suffix++;
        }

        return roleName;
    }
}
