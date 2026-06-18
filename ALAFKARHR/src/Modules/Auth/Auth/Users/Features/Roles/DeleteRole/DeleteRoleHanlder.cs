using Shared.Exceptions;

namespace Auth.Users.Features.Roles.DeleteRole;

public record DeleteRoleCommand(string RoleName, Guid CompanyId):ICommand<DeleteRoleResult>;
public record DeleteRoleResult(bool IsSuccess);
public class DeleteRoleHanlder(RoleManager<ApplicationRole> roleManager)
    : ICommandHandler<DeleteRoleCommand, DeleteRoleResult>
{
    public async Task<DeleteRoleResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.FindByNameAsync(request.RoleName);

        if (existingRole is null)
            throw new NotFoundException($"Role not found: {request.RoleName}");
        if (existingRole.CompanyId != request.CompanyId)
            throw new BadRequestException("Role does not belong to the selected company.");
        if (!string.IsNullOrWhiteSpace(existingRole.TemplateKey))
            throw new BadRequestException("Managed default roles cannot be deleted.");

        var claims = await roleManager.GetClaimsAsync(existingRole);
        foreach(var claim in claims)
        {
            await roleManager.RemoveClaimAsync(existingRole,claim);
        }

        await roleManager.DeleteAsync(existingRole);


        return new DeleteRoleResult(true);
    }
}
