namespace AlAfkarERP.Shared.Pages.Reuable2;

public class ModalService
{
    public event Func<ModalOptions, Task>? OnShow;
    public event Func<Task>? OnHide;

    public async Task ShowAsync(ModalOptions options)
    {
        options.SanitizeForDisplay();

        if (OnShow != null)
            await OnShow.Invoke(options);
    }

    public async Task HideAsync()
    {
        if (OnHide != null)
            await OnHide.Invoke();
    }
}

public class ModalOptions
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public string Details { get; set; } = "";
    public bool IsError { get; set; }
    public ModalVariant Variant { get; set; } = ModalVariant.Default;
    public string? IconCssClass { get; set; }

    public string OkText { get; set; } = "OK";
    public string CancelText { get; set; } = "Cancel";

    public bool ShowCancel { get; set; } = false;

    public Func<Task>? OnOk { get; set; }
    public Func<Task>? OnCancel { get; set; }

    public void SanitizeForDisplay()
    {
        Title = SanitizeField(Title, "Notice");
        Message = SanitizeField(Message, "The request could not be completed. Please try again.");
        Details = SanitizeField(Details, "");
    }

    private static string SanitizeField(string? value, string fallback)
    {
        var sanitized = global::AlAfkarERP.Shared.Utilities.ApiErrorFormatter.SanitizePublicMessage(value);

        return string.IsNullOrWhiteSpace(sanitized) || global::AlAfkarERP.Shared.Utilities.ApiErrorFormatter.HasInternalDetails(sanitized)
            ? fallback
            : sanitized;
    }
}

public enum ModalVariant
{
    Default,
    Info,
    Success,
    Warning,
    Danger
}
