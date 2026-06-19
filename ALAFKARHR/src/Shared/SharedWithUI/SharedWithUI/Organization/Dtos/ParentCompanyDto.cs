using SharedWithUI.Organization.Enums;

namespace SharedWithUI.Organization.Dtos;

public class ParentCompanyDto
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string HqLocation { get; set; } = string.Empty;
    public double HqLongitude { get; set; }
    public double HqLatitude { get; set; }
    public string VatNo { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? CurrencyId { get; set; }
    public string TimeZone { get; set; } = "Asia/Riyadh";
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AdminUserName { get; set; }
    public string? AdminEmail { get; set; }
    public string? AdminPhoneNumber { get; set; }
    public string? AdminTemporaryPassword { get; set; }
    public CompanyLicenseDto License { get; set; } = new();
    public int UsersCount { get; set; }
    public int ChildCompaniesCount { get; set; }
    public int BranchesCount { get; set; }
    public bool IsLicenseExpired => License.EndDate.Date < DateTime.UtcNow.Date;
    public CompanyLicenseStatus EffectiveLicenseStatus => IsLicenseExpired ? CompanyLicenseStatus.Expired : License.Status;
}
