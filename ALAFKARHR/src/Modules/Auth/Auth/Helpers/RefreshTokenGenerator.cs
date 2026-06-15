using System.Security.Cryptography;

namespace Auth.Helpers;

public static class RefreshTokenGenerator
{
    private const int TokenByteLength = 64;

    public static string Generate()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenByteLength));
    }

    public static string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}
