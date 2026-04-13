using AlAfkarERP.Shared.Dtos.Auth;

namespace AlAfkarERP.Shared.Utilities;

public class SharedDataService
{
    //public SharedDataService() { }
    //public event Action OnChange;
    public event Func<Task>? OnChange1;
    public string Language { get; set; } = "Ar";
    public AuthTokens Tokens { get; set; }
    public async Task UpdateLanguageAsync()
    {

        Language = Language == "Ar" ? "Eng" : "Ar";
        //await NotifyStateChangedAsync();
        await NotifyStateChangedAsync();
    }
    public string SelectViewLang(string en, string ar)
        => Language == "Ar" ? ar : en;
    public string PageDirection
            => Language == "Ar" ? "rtl" : "ltr";
    public async Task UpdateTokensAsync(AuthTokens? tokens)
    {
        Tokens = tokens;
     await   NotifyStateChangedAsync();
        //await NotifyStateChangedAsync();
    }
    //private async Task NotifyStateChangedAsync()
    //{

    //    // fallback (e.g., if called from UI thread)
    //    OnChange?.Invoke();

    //}
    //private void NotifyStateChangedAsync()
    //{

    //    OnChange?.Invoke();

    //}
    private async Task NotifyStateChangedAsync()
    {
        if (OnChange1 != null)
        {
            foreach (var handler in OnChange1.GetInvocationList())
            {
                await ((Func<Task>)handler)();
            }
        }
    }
}
