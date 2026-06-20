using Shared.DDD;

namespace Organization.Organizations.Models;

public class LicenseCategory : Aggregate<Guid>
{
    public string Key { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int MaxUsers { get; private set; }
    public int MaxChildCompanies { get; private set; }
    public int MaxBranches { get; private set; }
    public decimal MonthlyPrice { get; private set; }
    public decimal YearlyPrice { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public string? CurrencyCode { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }

    private LicenseCategory()
    {
    }

    public static LicenseCategory Create(
        Guid id,
        string key,
        string name,
        int maxUsers,
        int maxChildCompanies,
        int maxBranches,
        decimal monthlyPrice,
        decimal yearlyPrice,
        Guid currencyId,
        string? currencyCode,
        string? notes,
        string createdBy)
    {
        Validate(key, name, maxUsers, maxChildCompanies, maxBranches, monthlyPrice, yearlyPrice, currencyId);

        return new LicenseCategory
        {
            Id = id,
            Key = NormalizeKey(key),
            Name = name.Trim(),
            MaxUsers = maxUsers,
            MaxChildCompanies = maxChildCompanies,
            MaxBranches = maxBranches,
            MonthlyPrice = monthlyPrice,
            YearlyPrice = yearlyPrice,
            CurrencyId = currencyId,
            CurrencyCode = currencyCode?.Trim().ToUpperInvariant(),
            IsActive = true,
            Notes = notes?.Trim(),
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string key,
        string name,
        int maxUsers,
        int maxChildCompanies,
        int maxBranches,
        decimal monthlyPrice,
        decimal yearlyPrice,
        Guid currencyId,
        string? currencyCode,
        string? notes,
        string modifiedBy)
    {
        Validate(key, name, maxUsers, maxChildCompanies, maxBranches, monthlyPrice, yearlyPrice, currencyId);

        Key = NormalizeKey(key);
        Name = name.Trim();
        MaxUsers = maxUsers;
        MaxChildCompanies = maxChildCompanies;
        MaxBranches = maxBranches;
        MonthlyPrice = monthlyPrice;
        YearlyPrice = yearlyPrice;
        CurrencyId = currencyId;
        CurrencyCode = currencyCode?.Trim().ToUpperInvariant();
        Notes = notes?.Trim();
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void SetActive(bool isActive, string modifiedBy)
    {
        IsActive = isActive;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    private static string NormalizeKey(string key) => key.Trim().ToLowerInvariant();

    private static void Validate(
        string key,
        string name,
        int maxUsers,
        int maxChildCompanies,
        int maxBranches,
        decimal monthlyPrice,
        decimal yearlyPrice,
        Guid currencyId)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("License category key is required", nameof(key));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("License category name is required", nameof(name));
        if (maxUsers < 1)
            throw new ArgumentOutOfRangeException(nameof(maxUsers), "Max users must be at least 1");
        if (maxChildCompanies < 0)
            throw new ArgumentOutOfRangeException(nameof(maxChildCompanies), "Max child companies cannot be negative");
        if (maxBranches < 0)
            throw new ArgumentOutOfRangeException(nameof(maxBranches), "Max branches cannot be negative");
        if (monthlyPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(monthlyPrice), "Monthly price cannot be negative");
        if (yearlyPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(yearlyPrice), "Yearly price cannot be negative");
        if (currencyId == Guid.Empty)
            throw new ArgumentException("Currency is required", nameof(currencyId));
    }
}
