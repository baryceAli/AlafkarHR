namespace AlAfkarERP.Shared.Pages.Reuable;

public class ToastService
{
    public event Action<ToastMessage>? OnShow;

    public void Show(ToastMessage message)
    {
        OnShow?.Invoke(message);
    }
}

public class ToastMessage
{
    public string Message { get; set; } = "";
    public string Type { get; set; } = "success"; // success, error, warning
}