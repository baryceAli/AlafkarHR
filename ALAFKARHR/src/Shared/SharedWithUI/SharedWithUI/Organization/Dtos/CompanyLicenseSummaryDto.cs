using SharedWithUI.Organization.Enums;

namespace SharedWithUI.Organization.Dtos;

public class CompanyLicenseSummaryDto
{
    public Guid CompanyId { get; set; }
    public Guid? LicenseCategoryId { get; set; }
    public CompanyLicenseStatus Status { get; set; } = CompanyLicenseStatus.Active;
    public CompanyLicenseStatus EffectiveStatus { get; set; } = CompanyLicenseStatus.Active;
    public string PlanKey { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddYears(1);
    public int MaxUsers { get; set; }
    public int MaxChildCompanies { get; set; }
    public int MaxBranches { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public Guid? CurrencyId { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsExpired { get; set; }
}
