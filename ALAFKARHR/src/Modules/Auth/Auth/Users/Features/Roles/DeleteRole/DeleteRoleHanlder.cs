using Shared.Exceptions;

namespace Auth.Users.Features.Roles.DeleteRole;

public record DeleteRoleCommand(string RoleName):ICommand<DeleteRoleResult>;
public record DeleteRoleResult(bool IsSuccess);
public class DeleteRoleHanlder(RoleManager<ApplicationRole> roleManager)
    : ICommandHandler<DeleteRoleCommand, DeleteRoleResult>
{
    public async Task<DeleteRoleResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.FindByNameAsync(request.RoleName);

        if (existingRole is null)
            throw new NotFoundException($"Role not found: {request.RoleName}");

        var claims = await roleManager.GetClaimsAsync(existingRole);
        foreach(var claim in claims)
        {
            await roleManager.RemoveClaimAsync(existingRole,claim);
        }

        await roleManager.DeleteAsync(existingRole);


        return new DeleteRoleResult(true);
    }
}
