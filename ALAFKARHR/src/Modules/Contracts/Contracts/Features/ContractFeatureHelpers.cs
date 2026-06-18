namespace Contracts.Contracts.Features;

public static class ContractFeatureHelpers
{
    public static string CurrentUserId(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? httpContextAccessor.HttpContext?.User.FindFirstValue("sub")
        ?? "system";

    public static Guid CurrentUserGuid(IHttpContextAccessor httpContextAccessor) =>
        Guid.TryParse(CurrentUserId(httpContextAccessor), out var userId) ? userId : Guid.Empty;

    public static IQueryable<Contract> IncludeDetails(this IQueryable<Contract> query) =>
        query
            .Include(x => x.Renewals)
            .Include(x => x.Attachments)
            .Include(x => x.StatusHistory);

    public static async Task<string> GenerateNumberAsync(ContractsDbContext dbContext, Guid companyId, string type, CancellationToken cancellationToken)
    {
        var normalizedType = NormalizeCode(type);
        var prefix = $"{normalizedType}-{DateTime.UtcNow:yyyy}";
        var count = await dbContext.Contracts.IgnoreQueryFilters()
            .CountAsync(x => x.CompanyId == companyId && x.Type == type && x.Number.StartsWith(prefix), cancellationToken);

        return $"{prefix}-{count + 1:0000}";
    }

    public static async Task<string> SaveFileAsync(IFormFile file, IWebHostEnvironment environment, string moduleFolder, Guid ownerId, IReadOnlyCollection<string> allowedContentTypes, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            throw new BadRequestException("File is empty.");
        if (file.Length > 10 * 1024 * 1024)
            throw new BadRequestException("File size cannot exceed 10 MB.");
        if (!allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            throw new BadRequestException("File type is not allowed.");

        var uploadRoot = Path.Combine(environment.WebRootPath ?? "wwwroot", "Images", "Contracts", moduleFolder, ownerId.ToString());
        Directory.CreateDirectory(uploadRoot);
        var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var physicalPath = Path.Combine(uploadRoot, safeFileName);
        await using var stream = File.Create(physicalPath);
        await file.CopyToAsync(stream, cancellationToken);
        return $"/Images/Contracts/{moduleFolder}/{ownerId}/{safeFileName}";
    }

    public static HashSet<string> AllowedDocumentContentTypes() =>
        new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "image/jpeg",
            "image/png",
            "image/webp",
            "text/plain",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };

    private static string NormalizeCode(string value)
    {
        var chars = value.Where(char.IsLetterOrDigit).Take(6).ToArray();
        return chars.Length == 0 ? "CON" : new string(chars).ToUpperInvariant();
    }
}
