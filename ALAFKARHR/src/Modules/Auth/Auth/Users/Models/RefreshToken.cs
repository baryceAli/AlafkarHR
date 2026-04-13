namespace Auth.Users.Models;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public DateTime Created { get; private set; }
    public string CreatedByIp { get; private set; }

    public DateTime? Revoked { get; private set; }
    public string? RevokedByIp { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public bool IsActive => Revoked == null && !IsExpired;

    private RefreshToken() { } // EF

    public static RefreshToken Create(Guid userId,string token, DateTime expiryDate, string createdBy)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId=userId,
            Token = token,
            CreatedByIp= createdBy,
            Created=DateTime.UtcNow,
            ExpiryDate = expiryDate
        };
    }
    public void Revoke(string? tokenOwner)
    {
        Revoked = DateTime.UtcNow;
        RevokedByIp = tokenOwner;
    }
    
}
