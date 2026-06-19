using Auth.Contracts.Features.ResetCompanyAdminPassword;

namespace Auth.Users.Features.CompanyAdmins.ResetCompanyAdminPassword;

public class ResetCompanyAdminPasswordCommandValidator : AbstractValidator<ResetCompanyAdminPasswordCommand>
{
    public ResetCompanyAdminPasswordCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.TemporaryPassword).NotEmpty();
    }
}

public class ResetCompanyAdminPasswordHandler(UserManager<ApplicationUser> userManager)
    : ICommandHandler<ResetCompanyAdminPasswordCommand, ResetCompanyAdminPasswordResult>
{
    public async Task<ResetCompanyAdminPasswordResult> Handle(ResetCompanyAdminPasswordCommand request, CancellationToken cancellationToken)
    {
        var roleName = CompanyRoleTemplates.BuildSystemAdminRoleName(request.CompanyId);
        var users = await userManager.GetUsersInRoleAsync(roleName);
        var admin = users.FirstOrDefault(x => x.CompanyId == request.CompanyId)
            ?? throw new NotFoundException($"Admin user not found for company: {request.CompanyId}");

        var token = await userManager.GeneratePasswordResetTokenAsync(admin);
        var result = await userManager.ResetPasswordAsync(admin, token, request.TemporaryPassword);

        if (!result.Succeeded)
            throw new Exception(string.Join("Reset company admin password, ", result.Errors.Select(e => e.Description)));

        return new ResetCompanyAdminPasswordResult(true);
    }
}
