using Auth.Helpers;

namespace Auth.Users.Features.Authentication.ValidateRefreshToken;

public record RefreshTokenCommand(string AccessToken,string RefreshToken) : ICommand<RefreshTokenResult>;
public record RefreshTokenResult(string NewAccessToken,string NewRefreshToken);
public class RefreshTokenHandler (
    AuthDbContext dbContext, 
    IJwtTokenGenerator jwtTokenGenerator, 
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration)
    : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {

        //1️⃣ Validate token
        var jwtKey = configuration["JwtOptions:SecretKey"]!;
        var principal = GetPrincipalFromExpiredToken(command.AccessToken, jwtKey);
        //principal.Identity.
        var userId  =principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
          ?? principal.FindFirst("sub")?.Value;

        var userIdGuid= Guid.TryParse(userId, out var parsedGuid) ? parsedGuid : (Guid?)null;
        if (userId is null)
            throw new Exception("Invalid token");

        var userGuid = Guid.Parse(userId);

        var dbUser = await dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstAsync(u => u.Id == userIdGuid);
        
        //var user = await userManager.FindByIdAsync(userGuid.ToString());

        if (dbUser == null)
            throw new Exception("User not found");
        //2️⃣ Generate new JWT
        var roles = await userManager.GetRolesAsync(dbUser);
        //var accessToken=jwtTokenGenerator.GenerateToken(dbUser,roles);
        var accessToken = await jwtTokenGenerator.GenerateTokenAsync(dbUser);
        //3️⃣ Generate new refresh token
        var refreshToken = dbUser.RefreshTokens.FirstOrDefault(x => x.Token == command.RefreshToken);
        
        if (refreshToken == null || refreshToken.ExpiryDate < DateTime.UtcNow)
            throw new Exception("Token not found");

        //4️⃣ Revoke old refresh token

        // ❌ Reuse detection (CRITICAL SECURITY)
        if (!refreshToken.IsActive)
        {
            // Token was revoked → possible attack
            dbUser.RevokeRefreshToken(command.RefreshToken, dbUser.Email);

            await dbContext.SaveChangesAsync();

            throw new Exception("Token reuse detected");
        }


        // 🔁 ROTATION
        //var newRefreshToken = dbUser.AddRefreshToken(dbUser.Id, Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(3),dbUser.Email);

        //var token = Guid.NewGuid().ToString();
        //dbUser.RotateRefreshToken(
        //    command.RefreshToken,
        //    token,
        //    DateTime.UtcNow.AddDays(3),
        //    dbUser.Email
        //);

        
        //await dbContext.SaveChangesAsync();

        return new RefreshTokenResult(accessToken, refreshToken.Token);
    }


    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string key)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // 🔥 IGNORE EXPIRATION
            ValidIssuer = "SimpleEShop",
            ValidAudience = "SimpleEShopUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out SecurityToken securityToken);

        var jwtToken = securityToken as JwtSecurityToken;

        if (jwtToken == null ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
