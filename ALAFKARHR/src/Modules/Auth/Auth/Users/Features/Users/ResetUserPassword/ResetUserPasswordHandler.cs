using System.Security.Claims;
using Auth.Users;

namespace Auth.Users.Features.Users.ResetUserPassword;

public record ResetUserPasswordCommand(string UserName, string TemporaryPassword, ClaimsPrincipal User) : ICommand<ResetUserPasswordResult>;
public record ResetUserPasswordResult(bool IsSuccess);

public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.TemporaryPassword).NotEmpty();
    }
}

public class ResetUserPasswordHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<ResetUserPasswordCommand, ResetUserPasswordResult>
{
    public async Task<ResetUserPasswordResult> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var currentCompanyId = GetCurrentCompanyId(request.User);
        var userName = UserNameKeyNormalizer.Normalize(request.UserName);
        if (string.IsNullOrWhiteSpace(userName))
            throw new BadRequestException("User name is required.");

        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
            throw new NotFoundException($"User not found: {userName}");

        if (!user.CompanyId.HasValue || user.CompanyId.Value != currentCompanyId)
            throw new BadRequestException("Cannot reset password for a user outside your company.");

        if (await IsProtectedAdminAsync(user))
            throw new BadRequestException("Admin users cannot be reset from this page.");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, request.TemporaryPassword);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join("Reset user password, ", result.Errors.Select(e => e.Description)));

        return new ResetUserPasswordResult(true);
    }

    private static Guid GetCurrentCompanyId(ClaimsPrincipal user)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(c => c.Type == "company_id")?.Value, out var companyId))
            throw new BadRequestException("Company claim is missing.");

        return companyId;
    }

    private async Task<bool> IsProtectedAdminAsync(ApplicationUser user)
    {
        var normalizedUserName = UserNameKeyNormalizer.Normalize(user.UserName ?? string.Empty);
        if (string.Equals(normalizedUserName, "admin", StringComparison.OrdinalIgnoreCase))
            return true;

        if (!user.CompanyId.HasValue)
            return true;

        var companyAdminRoleName = CompanyRoleTemplates.BuildSystemAdminRoleName(user.CompanyId.Value);
        return await userManager.IsInRoleAsync(user, companyAdminRoleName);
    }
}
