
namespace Auth.Users.Models;

public class ApplicationUser
    : IdentityUser<Guid>, IAggregateRoot
{
    //public Guid Id { get; private set; }
    private readonly List<RefreshToken> _refreshTokens = new();
    
    
    public UserType UserType { get; private set; }//SystemUser, Customer, Driver
    public string? Otp { get; private set; }
    public bool? IsOtpConfirmed { get; set; }
    public OTPType OtpType { get; set; }
    public DateTime OtpExpiration { get; set; }
    public Guid CompanyId { get; set; }
    //public Guid? EmployeeId { get; set; }
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private ApplicationUser() { }
    public static ApplicationUser Create(
        Guid id, 
        string userName, 
        string email, 
        string phoneNumber, 
        UserType userType, 
        string otp, 
        OTPType otpType, 
        DateTime otpExpiration,
        Guid companyId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userName);
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(phoneNumber);
        //ArgumentException.ThrowIfNullOrEmpty(userType);

        ApplicationUser applicationUser = new ApplicationUser()
        {
            Id = id,
            UserName = userName,
            Email = email,
            PhoneNumber = phoneNumber,
            UserType = userType,
            Otp = otp,
            OtpType = otpType,
            OtpExpiration = otpExpiration,
            CompanyId= companyId,
            //EmployeeId= employeeId
        };
        applicationUser.AddDomainEvent(new UserRegistredEvent(applicationUser));
        return applicationUser;
    }
    public void Update(UserType userType)
    {
        UserType = userType;

    }
    public void UpdateOtp(string otp, OTPType otpType, DateTime otpExpiration, bool isOtpConfirmed)
    {

        ArgumentException.ThrowIfNullOrEmpty(otp);
        ArgumentOutOfRangeException.ThrowIfLessThan(otpExpiration, DateTime.UtcNow);

        Otp = otp;
        OtpType = otpType;
        OtpExpiration = otpExpiration;
        IsOtpConfirmed = isOtpConfirmed;
        AddDomainEvent(new OTPChangedEvent(this));
    }

    public void ConfirmOtp(string otp)
    {
        if (string.IsNullOrEmpty(Otp) || 
            Otp != otp || 
            OtpExpiration < DateTime.UtcNow || 
            IsOtpConfirmed==null || 
            IsOtpConfirmed==true)
                throw new Exception("Invalid or expired OTP");

        
        IsOtpConfirmed = true;

        //AddDomainEvent(new OTPConfirmedEvent(this));
    }
    // ✅ Add Refresh Token
    public RefreshToken AddRefreshToken(Guid userId,string token, DateTime expiryDate, string createdByIp)
    {
        var refreshToken =  RefreshToken.Create (userId,token, expiryDate, createdByIp);

        _refreshTokens.Add(refreshToken);

        return refreshToken;
    }

    // ✅ Revoke Token
    public void RevokeRefreshToken(string token, string? revokedByIp)
    {
        var existingToken = _refreshTokens
            .SingleOrDefault(x => x.Token == token);

        if (existingToken == null)
            throw new Exception("Invalid token");

        existingToken.Revoke(revokedByIp);
    }

    // ✅ Get Active Token
    public RefreshToken? GetActiveRefreshToken(string token)
    {
        var reftokens = _refreshTokens;
        return _refreshTokens
            .SingleOrDefault(x => x.Token == token && x.IsActive);
    }
    public RefreshToken RotateRefreshToken(string oldToken, string newToken, DateTime expiry, string ip)
    {
        var existing = _refreshTokens
            .SingleOrDefault(x => x.Token == oldToken );

        if (existing == null)
            throw new Exception("Invalid token");

        if (!existing.IsActive)
            throw new Exception("Token is not valid");


        existing.Revoke(ip);

        return AddRefreshToken(Id,newToken, expiry, ip);
    }
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    public IDomainEvent[] ClearDomainEvents()
    {
        IDomainEvent[] dequeuedEvents = _domainEvents.ToArray();
        _domainEvents.Clear();
        return dequeuedEvents;
    }
}
