using AlAfkarERP.Shared.Utilities;
using Microsoft.AspNetCore.Components;

namespace AlAfkarERP.Shared.Pages.PublicSite;

public abstract class SiteComponentBase : ComponentBase, IDisposable
{
    [Inject] protected SharedDataService Language { get; set; } = default!;
    protected string T(string en, string ar) => Language.SelectViewLang(en, ar);
    protected string T(SiteText text) => T(text.En, text.Ar);
    protected override void OnInitialized() => Language.OnChange1 += Changed;
    private Task Changed() => InvokeAsync(StateHasChanged);
    public virtual void Dispose() => Language.OnChange1 -= Changed;
}
