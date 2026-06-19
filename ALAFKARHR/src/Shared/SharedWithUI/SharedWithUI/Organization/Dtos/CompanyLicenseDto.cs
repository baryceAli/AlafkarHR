using SharedWithUI.Organization.Enums;

namespace SharedWithUI.Organization.Dtos;

public class CompanyLicenseDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public CompanyLicenseStatus Status { get; set; } = CompanyLicenseStatus.Active;
    public string PlanKey { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddYears(1);
    public int MaxUsers { get; set; } = 25;
    public int MaxChildCompanies { get; set; } = 5;
    public int MaxBranches { get; set; } = 10;
    public string? Notes { get; set; }
}
