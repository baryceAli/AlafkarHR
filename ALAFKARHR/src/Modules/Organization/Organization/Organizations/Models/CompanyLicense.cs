using Shared.DDD;
using SharedWithUI.Organization.Enums;

namespace Organization.Organizations.Models;

public class CompanyLicense : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = default!;
    public Guid? LicenseCategoryId { get; private set; }
    public LicenseCategory? LicenseCategory { get; private set; }
    public CompanyLicenseStatus Status { get; private set; }
    public string PlanKey { get; private set; } = string.Empty;
    public string PlanName { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int MaxUsers { get; private set; }
    public int MaxChildCompanies { get; private set; }
    public int MaxBranches { get; private set; }
    public string? Notes { get; private set; }
    public string EffectivePlanKey => LicenseCategory?.Key ?? PlanKey;
    public string EffectivePlanName => LicenseCategory?.Name ?? PlanName;
    public int EffectiveMaxUsers => LicenseCategory?.MaxUsers ?? MaxUsers;
    public int EffectiveMaxChildCompanies => LicenseCategory?.MaxChildCompanies ?? MaxChildCompanies;
    public int EffectiveMaxBranches => LicenseCategory?.MaxBranches ?? MaxBranches;
    public decimal EffectiveMonthlyPrice => LicenseCategory?.MonthlyPrice ?? 0;
    public decimal EffectiveYearlyPrice => LicenseCategory?.YearlyPrice ?? 0;
    public string EffectiveCurrencyCode => LicenseCategory?.CurrencyCode ?? "SAR";

    private CompanyLicense() { }

    public static CompanyLicense Create(
        Guid id,
        Guid companyId,
        CompanyLicenseStatus status,
        string planKey,
        string planName,
        DateTime startDate,
        DateTime endDate,
        int maxUsers,
        int maxChildCompanies,
        int maxBranches,
        string? notes,
        string createdBy,
        Guid? licenseCategoryId = null)
    {
        ValidateLimits(maxUsers, maxChildCompanies, maxBranches);
        if (endDate.Date < startDate.Date)
            throw new ArgumentException("License end date must be after start date");

        return new CompanyLicense
        {
            Id = id,
            CompanyId = companyId,
            LicenseCategoryId = licenseCategoryId,
            Status = status,
            PlanKey = planKey.Trim(),
            PlanName = planName.Trim(),
            StartDate = startDate.Date,
            EndDate = endDate.Date,
            MaxUsers = maxUsers,
            MaxChildCompanies = maxChildCompanies,
            MaxBranches = maxBranches,
            Notes = notes?.Trim(),
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        CompanyLicenseStatus status,
        string planKey,
        string planName,
        DateTime startDate,
        DateTime endDate,
        int maxUsers,
        int maxChildCompanies,
        int maxBranches,
        string? notes,
        string modifiedBy,
        Guid? licenseCategoryId = null)
    {
        ValidateLimits(maxUsers, maxChildCompanies, maxBranches);
        if (endDate.Date < startDate.Date)
            throw new ArgumentException("License end date must be after start date");

        Status = status;
        LicenseCategoryId = licenseCategoryId;
        PlanKey = planKey.Trim();
        PlanName = planName.Trim();
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        MaxUsers = maxUsers;
        MaxChildCompanies = maxChildCompanies;
        MaxBranches = maxBranches;
        Notes = notes?.Trim();
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public bool AllowsAccess(DateTime utcNow) =>
        Status == CompanyLicenseStatus.Active &&
        StartDate.Date <= utcNow.Date &&
        EndDate.Date >= utcNow.Date;

    private static void ValidateLimits(int maxUsers, int maxChildCompanies, int maxBranches)
    {
        if (maxUsers < 1)
            throw new ArgumentOutOfRangeException(nameof(maxUsers), "Max users must be at least 1");
        if (maxChildCompanies < 0)
            throw new ArgumentOutOfRangeException(nameof(maxChildCompanies), "Max child companies cannot be negative");
        if (maxBranches < 0)
            throw new ArgumentOutOfRangeException(nameof(maxBranches), "Max branches cannot be negative");
    }
}
