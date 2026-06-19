using Auth.Helpers;
using Organization.Contracts.Companies.Features.GetCompanyAccessStatus;

namespace Auth.Users.Features.Authentication.ValidateRefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResult>;
public record RefreshTokenResult(string AccessToken, string RefreshToken);

public class RefreshTokenHandler(
    AuthDbContext dbContext,
    IJwtTokenGenerator jwtTokenGenerator,
    ISender sender)
    : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            throw new Exception("Invalid token");

        var refreshTokenHash = RefreshTokenGenerator.Hash(command.RefreshToken);

        var tokenOwner = await dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(
                u => u.RefreshTokens.Any(rt => rt.Token == refreshTokenHash),
                cancellationToken);

        if (tokenOwner is null)
            throw new Exception("Invalid token");

        var refreshToken = tokenOwner.RefreshTokens
            .FirstOrDefault(x => x.Token == refreshTokenHash);

        if (refreshToken is null)
            throw new Exception("Invalid token");

        var tokenOwnerName = tokenOwner.Email ?? tokenOwner.UserName ?? tokenOwner.Id.ToString();

        if (!refreshToken.IsActive)
        {
            foreach (var activeToken in tokenOwner.RefreshTokens.Where(x => x.IsActive))
            {
                activeToken.Revoke(tokenOwnerName);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            throw new Exception("Token reuse detected");
        }

        if (tokenOwner.CompanyId.HasValue)
        {
            var accessStatus = await sender.Send(new GetCompanyAccessStatusQuery(tokenOwner.CompanyId.Value), cancellationToken);
            if (!accessStatus.CanLogin)
                throw new Exception("Company is disabled");
        }

        var rawNewRefreshToken = RefreshTokenGenerator.Generate();
        var newRefreshTokenHash = RefreshTokenGenerator.Hash(rawNewRefreshToken);

        var newRefreshToken = tokenOwner.RotateRefreshToken(
            refreshTokenHash,
            newRefreshTokenHash,
            DateTime.UtcNow.AddDays(7),
            tokenOwnerName);

        var accessToken = await jwtTokenGenerator.GenerateTokenAsync(tokenOwner);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResult(accessToken, rawNewRefreshToken);
    }
}
