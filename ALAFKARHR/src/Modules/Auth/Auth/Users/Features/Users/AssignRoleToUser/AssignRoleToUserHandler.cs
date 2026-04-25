using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Exceptions;

namespace Auth.Users.Features.Users.AssignRoleToUser;


public record AssignRoleToUserCommand(UserRoleDto UserRole) : ICommand<AssignRoleToUserResult>;
public record AssignRoleToUserResult(bool IsSuccess);
public class AssignRoleToUserHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<AssignRoleToUserCommand, AssignRoleToUserResult>
{
    public async Task<AssignRoleToUserResult> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        //get user
        var user = await userManager.FindByNameAsync(request.UserRole.UserName);
        if (user is null)
            throw new NotFoundException($"User not found: {request.UserRole.UserName}");

        //make sure role is not already exist
        var isExist = await userManager.IsInRoleAsync(user, request.UserRole.RoleName);
        if (isExist)
            throw new Exception($"Role ({request.UserRole}) is already exist for user ({request.UserRole.UserName})");

        //assing role to user

        var result=await userManager.AddToRoleAsync(user,request.UserRole.RoleName);

        return new AssignRoleToUserResult(result.Succeeded);
    }
}
