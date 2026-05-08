using SharedWithUI.Catalog.Dtos;

namespace SharedWithUI.Catalog.SKUGenerator;

//1. 
public record SkuBuildContext(
    Guid ProductId,
    Guid BrandId,
    Guid? PackageId,
    List<(Guid VariantId, Guid ValueId)> Variants
);
public static class ProductSkuGenerator
{
    //2. 
    public static string BuildSkuKey(SkuBuildContext ctx)
    {
        var variantPart = ctx.Variants
            .OrderBy(v => v.VariantId)
            .ThenBy(v => v.ValueId)
            .Select(v => $"{v.VariantId}:{v.ValueId}");

        return string.Join("|",
            ctx.ProductId,
            ctx.BrandId,
            ctx.PackageId?.ToString() ?? "NONE",
            string.Join(",", variantPart));
    }

    //3. 
    public static string GenerateSkuCode(SkuBuildContext ctx,
    Dictionary<Guid, string> variantNames,
    Dictionary<Guid, string> valueNames,
    //List<VariantDto> variants,
    string productName,
    string brandName)
    {


        var parts = new List<string>
        {
            productName[..Math.Min(5, productName.Length)].ToUpper(),
            brandName[..Math.Min(5, brandName.Length)].ToUpper()
        };

        if (ctx.PackageId.HasValue)
            parts.Add("PKG");

        parts.AddRange(ctx.Variants
            .OrderBy(v => v.VariantId)
            .Select(v =>
            {
                var vName = valueNames[v.ValueId];
                return vName[..Math.Min(5, vName.Length)].ToUpper();
            }));

        return string.Join("-", parts);
    }



}