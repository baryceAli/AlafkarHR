namespace AlAfkarERP.Shared.Layout;

public static class NavigationMenuResolver
{
    public static IReadOnlyList<MenuItem> GetActivePath(string currentUri, string baseUri)
    {
        var currentPath = NormalizePath(ToRelativePath(currentUri, baseUri));
        var bestMatch = new List<MenuItem>();

        foreach (var item in MenuItem.Menu)
        {
            var path = FindBestPath(item, currentPath);
            if (path.Count > bestMatch.Count)
            {
                bestMatch = path;
            }
        }

        return bestMatch;
    }

    public static string NormalizePath(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return "/";
        }

        var path = url.Split('?', '#')[0].Trim();

        if (Uri.TryCreate(path, UriKind.Absolute, out var absoluteUri))
        {
            path = absoluteUri.AbsolutePath;
        }

        path = "/" + path.Trim('/').ToLowerInvariant();
        return path == "/" ? "/" : path.TrimEnd('/');
    }

    public static string GetStorageKey(MenuItem item)
        => !string.IsNullOrWhiteSpace(item.Url)
            ? NormalizePath(item.Url)
            : $"{item.TextEn}|{item.TextAr}";

    public static IEnumerable<MenuItem> Flatten(IEnumerable<MenuItem> items)
    {
        foreach (var item in items)
        {
            yield return item;

            foreach (var child in Flatten(item.Children))
            {
                yield return child;
            }
        }
    }

    private static List<MenuItem> FindBestPath(MenuItem item, string currentPath)
    {
        var ownPath = NormalizePath(item.Url);
        var isOwnMatch = !string.IsNullOrWhiteSpace(item.Url)
                         && (currentPath == ownPath
                             || ownPath != "/" && currentPath.StartsWith($"{ownPath}/", StringComparison.OrdinalIgnoreCase));

        var bestChildPath = new List<MenuItem>();
        foreach (var child in item.Children)
        {
            var childPath = FindBestPath(child, currentPath);
            if (childPath.Count > bestChildPath.Count)
            {
                bestChildPath = childPath;
            }
        }

        if (isOwnMatch || bestChildPath.Count > 0)
        {
            var result = new List<MenuItem> { item };
            result.AddRange(bestChildPath);
            return result;
        }

        return new List<MenuItem>();
    }

    private static string ToRelativePath(string currentUri, string baseUri)
    {
        if (Uri.TryCreate(currentUri, UriKind.Absolute, out var absoluteCurrent)
            && Uri.TryCreate(baseUri, UriKind.Absolute, out var absoluteBase)
            && absoluteBase.IsBaseOf(absoluteCurrent))
        {
            var relative = absoluteBase.MakeRelativeUri(absoluteCurrent).ToString();
            return Uri.UnescapeDataString(relative);
        }

        return currentUri;
    }
}
