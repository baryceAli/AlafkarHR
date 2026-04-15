namespace AlAfkarERP.Shared.Pages;

public static class TableSearchHelper
{
    public static IEnumerable<T> OrderByDynamic<T>(this IEnumerable<T> data, string property, bool asc)
    {
        var prop = typeof(T).GetProperty(property);
        return asc
            ? data.OrderBy(x => prop.GetValue(x, null))
            : data.OrderByDescending(x => prop.GetValue(x, null));
    }
}
