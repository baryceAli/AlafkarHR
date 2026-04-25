namespace Catalog.Products.Helpers;

public static class GenerateSKU
{
    public static string Generate(string productName, string brandName,string variantName, string variantValue, string unit, string packageName)
    {
        var parts = new List<string>();

        // Brand
        parts.Add(Normalize(brandName));

        // Product
        parts.Add(Normalize(productName));

        // Variant Name (e.g., "Size", "Color")
        parts.Add(Normalize(variantName));
        // Variants (ordered for consistency)
        parts.Add(Normalize(variantValue));

        parts.Add(Normalize(unit));
        parts.Add(Normalize(packageName));

        return string.Join("-", parts);
    }

    private static string Normalize(string input)
    {
        return input
            .Trim()
            .ToUpper()
            .Replace(" ", "")
            .Replace("-", "");
    }
}

