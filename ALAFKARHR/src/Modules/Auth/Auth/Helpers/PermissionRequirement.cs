using Microsoft.AspNetCore.Authorization;

namespace Auth.Helpers;

public class PermissionRequirement :IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
