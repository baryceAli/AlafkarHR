namespace AlAfkarERP.Shared.Pages.Reuable2;

public class ToastService
{
    public event Action<ToastMessage>? OnShow;

    public void Show(ToastMessage message)
    {
        message.SanitizeForDisplay();
        OnShow?.Invoke(message);
    }

    public void ShowSuccess(string message)
        => Show(new ToastMessage { Message = message, Type = "success" });

    public void ShowError(string message)
        => Show(new ToastMessage { Message = message, Type = "error" });

    public void ShowWarning(string message)
        => Show(new ToastMessage { Message = message, Type = "warning" });
}

public class ToastMessage
{
    public string Message { get; set; } = "";
    public string Type { get; set; } = "success"; // success, error, warning

    public void SanitizeForDisplay()
    {
        var sanitized = global::AlAfkarERP.Shared.Utilities.ApiErrorFormatter.SanitizePublicMessage(Message);
        Type = Type.ToLowerInvariant() switch
        {
            "error" => "error",
            "warning" => "warning",
            _ => "success"
        };

        var fallback = Type switch
        {
            "error" => "The request could not be completed. Please try again.",
            "warning" => "Please review the request and try again.",
            _ => "Done."
        };

        Message = string.IsNullOrWhiteSpace(sanitized) || global::AlAfkarERP.Shared.Utilities.ApiErrorFormatter.HasInternalDetails(sanitized)
            ? fallback
            : sanitized;
    }
}
