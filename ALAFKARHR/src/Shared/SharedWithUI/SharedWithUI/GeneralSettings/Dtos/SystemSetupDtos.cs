namespace SharedWithUI.GeneralSettings.Dtos;

public enum SetupStepStatus
{
    Incomplete,
    Complete,
    AttentionRequired,
    Unavailable,
    Optional,
    NotApplicable
}

public class SetupStepDto
{
    public string Key { get; set; } = string.Empty;
    public string TrackKey { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-check2-circle";
    public string? Route { get; set; }
    public string? RequiredPermission { get; set; }
    public int Order { get; set; }
    public bool IsOptional { get; set; }
    public bool IsBlocking { get; set; }
    public bool IsComplete { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsSkipped { get; set; }
    public SetupStepStatus Status { get; set; }
    public string? DetailEn { get; set; }
    public string? DetailAr { get; set; }
    public List<string> DependencyStepKeys { get; set; } = [];
    public string? AvailabilityReasonEn { get; set; }
    public string? AvailabilityReasonAr { get; set; }
}

public class SetupTrackDto
{
    public string Key { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-sliders2";
    public int Order { get; set; }
    public bool IsOptional { get; set; }
    public bool IsSelected { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsComplete { get; set; }
    public int CompletionPercent { get; set; }
    public string? RequiredBusinessLineKey { get; set; }
    public List<string> DependencyTrackKeys { get; set; } = [];
    public string? AvailabilityReasonEn { get; set; }
    public string? AvailabilityReasonAr { get; set; }
    public List<SetupStepDto> Steps { get; set; } = [];
}

public class SetupSummaryDto
{
    public int SchemaVersion { get; set; }
    public string CurrentStepKey { get; set; } = string.Empty;
    public bool RemindersDismissed { get; set; }
    public bool IsComplete { get; set; }
    public int CompletionPercent { get; set; }
    public bool IsCoreComplete { get; set; }
    public int CoreCompletionPercent { get; set; }
    public bool HasAttentionRequired { get; set; }
    public DateTime ReadinessRefreshedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<string> SelectedTrackKeys { get; set; } = [];
    public List<string> SkippedStepKeys { get; set; } = [];
    public List<SetupTrackDto> Tracks { get; set; } = [];
}

public class UpdateSetupProgressDto
{
    public string? CurrentStepKey { get; set; }
    public bool RemindersDismissed { get; set; }
    public List<string> SelectedTrackKeys { get; set; } = [];
    public List<string> SkippedStepKeys { get; set; } = [];
}
