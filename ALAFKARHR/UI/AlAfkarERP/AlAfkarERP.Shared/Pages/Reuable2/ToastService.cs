namespace AlAfkarERP.Shared.Pages.Reuable2;

public class ToastService
{
    public event Action<ToastMessage>? OnShow;

    public void Show(ToastMessage message)
    {
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
}
