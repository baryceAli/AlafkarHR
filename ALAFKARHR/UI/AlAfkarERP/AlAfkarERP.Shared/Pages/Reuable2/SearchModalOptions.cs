namespace AlAfkarERP.Shared.Pages.Reuable2;
public class SearchModalOptions<T>
{
    public string Title { get; set; } = "Search";

    // 🔥 EITHER remote OR local
    public Func<string, int, int, Task<PagedResult<T>>>? SearchFunc { get; set; }

    // 🔥 LOCAL DATA
    public IEnumerable<T>? Data { get; set; }

    // 🔥 SEARCH FIELDS
    public List<Func<T, string>> SearchSelectors { get; set; } = new();

    public List<SearchColumn<T>> Columns { get; set; } = new();

    public int PageSize { get; set; } = 20;

    public bool MultiSelect { get; set; }
}
public class SearchColumn<T>
{
    public string Header { get; set; } = "";
    public Func<T, string> Value { get; set; } = default!;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
}