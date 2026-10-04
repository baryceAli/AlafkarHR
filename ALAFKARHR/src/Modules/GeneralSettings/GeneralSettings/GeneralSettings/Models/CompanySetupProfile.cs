using Shared.DDD;

namespace GeneralSettings.GeneralSettings.Models;

public class CompanySetupProfile : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public int SchemaVersion { get; private set; }
    public string CurrentStepKey { get; private set; } = string.Empty;
    public bool RemindersDismissed { get; private set; }
    public string SelectedTrackKeys { get; private set; } = "core|compliance";
    public string SkippedStepKeys { get; private set; } = string.Empty;
    public DateTime? CompletedAt { get; private set; }

    private CompanySetupProfile()
    {
    }

    public static CompanySetupProfile Create(Guid companyId, string user)
        => new()
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            SchemaVersion = 2,
            CurrentStepKey = "company-profile",
            SelectedTrackKeys = "core|compliance",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user
        };

    public void Update(
        string? currentStepKey,
        bool remindersDismissed,
        IEnumerable<string> selectedTrackKeys,
        IEnumerable<string> skippedStepKeys,
        bool isComplete,
        string user)
    {
        SchemaVersion = Math.Max(SchemaVersion, 2);
        CurrentStepKey = string.IsNullOrWhiteSpace(currentStepKey) ? CurrentStepKey : currentStepKey.Trim();
        RemindersDismissed = remindersDismissed;
        SelectedTrackKeys = SerializeKeys(selectedTrackKeys.Append("core"));
        SkippedStepKeys = SerializeKeys(skippedStepKeys);
        CompletedAt = isComplete ? CompletedAt ?? DateTime.UtcNow : CompletedAt;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    public IReadOnlyList<string> GetSelectedTrackKeys() => ParseKeys(SelectedTrackKeys);
    public IReadOnlyList<string> GetSkippedStepKeys() => ParseKeys(SkippedStepKeys);

    private static string SerializeKeys(IEnumerable<string> keys)
        => string.Join('|', keys.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase));

    private static IReadOnlyList<string> ParseKeys(string value)
        => value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
