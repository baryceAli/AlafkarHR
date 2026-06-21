using Shared.DDD;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Models;

public class HomePageTemplateSelection : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public string ActiveTemplateKey { get; private set; } = HomePageTemplateKeys.CurrentStorefront;

    private HomePageTemplateSelection()
    {
    }

    public static HomePageTemplateSelection Create(
        Guid id,
        Guid companyId,
        string activeTemplateKey,
        string createdBy)
    {
        if (!HomePageTemplateKeys.IsValid(activeTemplateKey))
            throw new ArgumentException("Invalid home page template key.", nameof(activeTemplateKey));

        return new HomePageTemplateSelection
        {
            Id = id,
            CompanyId = companyId,
            ActiveTemplateKey = activeTemplateKey,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void SetActiveTemplate(string activeTemplateKey, string modifiedBy)
    {
        if (!HomePageTemplateKeys.IsValid(activeTemplateKey))
            throw new ArgumentException("Invalid home page template key.", nameof(activeTemplateKey));

        ActiveTemplateKey = activeTemplateKey;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}

