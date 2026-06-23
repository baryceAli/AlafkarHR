using Shared.DDD;
using SharedWithUI.Organization.Enums;

namespace Organization.Organizations.Models;

public class BusinessLine : Aggregate<Guid>
{
    public string Key { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameAr { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public BusinessLineActivationPolicy ActivationPolicy { get; private set; } = BusinessLineActivationPolicy.SinglePerCompany;

    private BusinessLine()
    {
    }

    public static BusinessLine Create(
        Guid id,
        string key,
        string name,
        string nameAr,
        string icon,
        string? description,
        int displayOrder,
        BusinessLineActivationPolicy activationPolicy,
        string createdBy)
    {
        Validate(key, name, nameAr, icon);

        return new BusinessLine
        {
            Id = id,
            Key = NormalizeKey(key),
            Name = name.Trim(),
            NameAr = nameAr.Trim(),
            Icon = icon.Trim(),
            Description = description?.Trim(),
            DisplayOrder = displayOrder,
            ActivationPolicy = activationPolicy,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string key, string name, string nameAr, string icon, string? description, int displayOrder, BusinessLineActivationPolicy activationPolicy, string modifiedBy)
    {
        Validate(key, name, nameAr, icon);

        Key = NormalizeKey(key);
        Name = name.Trim();
        NameAr = nameAr.Trim();
        Icon = icon.Trim();
        Description = description?.Trim();
        DisplayOrder = displayOrder;
        ActivationPolicy = activationPolicy;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void SetActive(bool isActive, string modifiedBy)
    {
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public static string NormalizeKey(string key) => key.Trim().ToLowerInvariant();

    private static void Validate(string key, string name, string nameAr, string icon)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Business line key is required", nameof(key));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Business line name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new ArgumentException("Business line Arabic name is required", nameof(nameAr));
        if (string.IsNullOrWhiteSpace(icon))
            throw new ArgumentException("Business line icon is required", nameof(icon));
    }
}
