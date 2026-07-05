using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Data;
using Auth.Users.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Shared.Contracts.Organization;

namespace Auth.Helpers;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AuthDbContext _authDbContext;
    private readonly ISender _sender;

    public JwtTokenGenerator(
        IOptions<JwtOptions> options,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        AuthDbContext authDbContext,
        ISender sender)
    {
        _options = options.Value;
        _userManager = userManager;
        _roleManager = roleManager;
        _authDbContext = authDbContext;
        _sender = sender;
    }

    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Typ,user.UserType.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            //new Claim("employee_id", user.EmployeeId?.ToString()??"")
        };

        if (user.CompanyId.HasValue)
        {
            claims.Add(new Claim("company_id", user.CompanyId.Value.ToString()));
        }

        var existingClaims = claims
            .Select(claim => (claim.Type, claim.Value))
            .ToHashSet();
        var existingClaimValues = claims
            .Select(claim => claim.Value)
            .ToHashSet();

        // ✅ Get Roles
        var roles = await _userManager.GetRolesAsync(user);
        var roleNames = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (user.CompanyId.HasValue)
        {
            await CompanyRoleTemplates.SyncAssignedTemplateRolesAsync(_roleManager, user.CompanyId.Value, roleNames);
        }

        foreach (var role in roleNames)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            existingClaims.Add((ClaimTypes.Role, role));
            existingClaimValues.Add(role);
        }

        var scopedRoleIds = await _authDbContext.Roles
            .AsNoTracking()
            .Where(role => roleNames.Contains(role.Name!) && role.CompanyId == user.CompanyId)
            .Select(role => role.Id)
            .ToListAsync();

        var roleClaims = scopedRoleIds.Count == 0
            ? []
            : await _authDbContext.Set<IdentityRoleClaim<Guid>>()
                .AsNoTracking()
                .Where(roleClaim => scopedRoleIds.Contains(roleClaim.RoleId))
                .Select(roleClaim => new Claim(roleClaim.ClaimType ?? string.Empty, roleClaim.ClaimValue ?? string.Empty))
                .ToListAsync();

        foreach (var rc in roleClaims)
        {
            if (existingClaims.Add((rc.Type, rc.Value)))
            {
                claims.Add(rc);
                existingClaimValues.Add(rc.Value);
            }
        }

        if (user.CompanyId.HasValue)
        {
            var scopedPermissions = await _sender.Send(new GetCurrentUserBranchRolePermissionsQuery(user.CompanyId.Value, user.Id));
            foreach (var permission in scopedPermissions.Permissions)
            {
                if (existingClaimValues.Add(permission))
                {
                    claims.Add(new Claim("ScopedPermission", permission));
                    existingClaims.Add(("ScopedPermission", permission));
                }
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
