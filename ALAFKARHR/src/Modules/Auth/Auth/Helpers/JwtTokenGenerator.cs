using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Users.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Helpers;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public JwtTokenGenerator(
        IOptions<JwtOptions> options,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _options = options.Value;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Typ,user.UserType.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim("company_id", user.CompanyId.ToString()),
            //new Claim("employee_id", user.EmployeeId?.ToString()??"")
        };

        // ✅ Get Roles
        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));

            // ✅ Get Role Permissions (THIS FIXES YOUR 403)
            var roleEntity = await _roleManager.FindByNameAsync(role);
            if (roleEntity is null) continue;

            var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);

            foreach (var rc in roleClaims)
            {
                claims.Add(rc); // 🔥 inject Permission claims into JWT
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}