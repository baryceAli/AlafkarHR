namespace Catalog.Products.Helpers;

internal static class CatalogUserContext
{
    private const string CompanyIdClaimType = "company_id";

    public static string GetUserId(IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.HttpContext?.User?
                   .FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? throw new UnauthorizedAccessException("User is not authenticated");
    }

    public static Guid GetCompanyId(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User?
            .FindFirst(CompanyIdClaimType)?.Value;

        return Guid.TryParse(value, out var companyId) && companyId != Guid.Empty
            ? companyId
            : throw new UnauthorizedAccessException("Company context is required");
    }
}
