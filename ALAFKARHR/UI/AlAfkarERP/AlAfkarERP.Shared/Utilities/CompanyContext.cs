using System.Security.Claims;

namespace AlAfkarERP.Shared.Utilities;

public static class CompanyContext
{
    public const string CompanyIdClaimType = "company_id";
    private const string ParentCompanyPermissionPrefix = "Organization.ParentCompany.";

    public static bool TryGetCompanyId(ClaimsPrincipal? user, out Guid companyId)
        => Guid.TryParse(
            user?.Claims.FirstOrDefault(claim => claim.Type == CompanyIdClaimType)?.Value,
            out companyId);

    public static bool HasCompanyContext(ClaimsPrincipal? user)
        => TryGetCompanyId(user, out _);

    public static bool IsPlatformScoped(ClaimsPrincipal? user)
        => user?.Identity?.IsAuthenticated == true && !HasCompanyContext(user);

    public static bool IsPlatformPermission(string? permission)
        => !string.IsNullOrWhiteSpace(permission)
           && permission.StartsWith(ParentCompanyPermissionPrefix, StringComparison.Ordinal);

    public static bool HasPermission(ClaimsPrincipal? user, string? permission)
    {
        if (user is null || string.IsNullOrWhiteSpace(permission))
        {
            return false;
        }

        if (IsPlatformScoped(user) && !IsPlatformPermission(permission))
        {
            return false;
        }

        return user.Claims.Any(claim => claim.Value == permission);
    }
}
