namespace AlAfkarERP.Shared.Pages.GuidedAssistant.Models;

public enum GuidedMode
{
    Actions,
    AddEmployee
}

public enum GuidedStepKind
{
    Text,
    Number,
    Date,
    Select,
    Choice
}

public sealed record GuidedWorkspace(string Key, string Title, string Description, string Icon);

public sealed record GuidedArea(string Key, string WorkspaceKey, string Title, string Description, string Icon, bool IsEnabled);

public sealed record GuidedAction(string Key, string Title, string Description, string Icon, bool IsEnabled);

public sealed record GuidedStep(string Key, string Prompt, GuidedStepKind Kind, bool IsRequired, string Placeholder);

public sealed record GuidedChoice(string Key, string Label, bool IsDefault = false);

public sealed record ChatMessage(string Text, bool IsUser);

public sealed record DraftField(string Key, string Label, string Value, bool IsRequired)
{
    public bool IsMissing => string.IsNullOrWhiteSpace(Value);
}
