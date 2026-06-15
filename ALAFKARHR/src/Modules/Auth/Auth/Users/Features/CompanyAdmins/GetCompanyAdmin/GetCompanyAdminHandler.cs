using Auth.Contracts.Features.GetCompanyAdmin;

namespace Auth.Users.Features.CompanyAdmins.GetCompanyAdmin;

public class GetCompanyAdminHandler(UserManager<ApplicationUser> userManager)
    : IQueryHandler<GetCompanyAdminQuery, GetCompanyAdminResult>
{
    public async Task<GetCompanyAdminResult> Handle(GetCompanyAdminQuery request, CancellationToken cancellationToken)
    {
        var roleName = $"SystemAdmin-{request.CompanyId:N}";
        var users = await userManager.GetUsersInRoleAsync(roleName);
        var admin = users.FirstOrDefault(x => x.CompanyId == request.CompanyId)
            ?? throw new NotFoundException($"Admin user not found for company: {request.CompanyId}");

        return new GetCompanyAdminResult(admin.Id, admin.UserName ?? string.Empty, admin.Email, admin.PhoneNumber);
    }
}
