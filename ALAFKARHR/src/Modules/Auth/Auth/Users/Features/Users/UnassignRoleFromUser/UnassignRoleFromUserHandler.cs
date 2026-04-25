

namespace Auth.Users.Features.Users.UnassignRoleFromUser;


public record UnassignRoleFromUserCommand(UserRoleDto UserRole) : ICommand<UnassignRoleFromUserResult>;
public record UnassignRoleFromUserResult(bool IsSuccess);
public class UnassignRoleFromUserHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<UnassignRoleFromUserCommand, UnassignRoleFromUserResult>
{
    public async Task<UnassignRoleFromUserResult> Handle(UnassignRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        // make sure user is exist
        var user =await userManager.FindByNameAsync(request.UserRole.UserName);
        if (user is null)
            throw new NotFoundException($"User not found: {request.UserRole.UserName}");


        // make sure rule is exist
        var role =await userManager.IsInRoleAsync(user, request.UserRole.RoleName);
        if (user is null)
            throw new NotFoundException($"Role not found: {request.UserRole.RoleName}");

        //remove role
        var result=await userManager.RemoveFromRoleAsync(user, request.UserRole.RoleName);
        //return success
        return new UnassignRoleFromUserResult(result.Succeeded);
    }
}
