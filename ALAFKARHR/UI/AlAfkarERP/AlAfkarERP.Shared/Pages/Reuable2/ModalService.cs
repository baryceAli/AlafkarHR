namespace AlAfkarERP.Shared.Pages.Reuable2;

public class ModalService
{
    public event Func<ModalOptions, Task>? OnShow;
    public event Func<Task>? OnHide;

    public async Task ShowAsync(ModalOptions options)
    {
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

    public string OkText { get; set; } = "OK";
    public string CancelText { get; set; } = "Cancel";

    public bool ShowCancel { get; set; } = false;

    public Func<Task>? OnOk { get; set; }
    public Func<Task>? OnCancel { get; set; }
}
