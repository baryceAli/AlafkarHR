using System.Security.Claims;
using FluentValidation;
using GeneralSettings.Data;
using GeneralSettings.GeneralSettings.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.CQRS;
using Shared.SaveImages;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.HomePageTemplates;

public record GetPublicHomePageTemplateQuery(Guid CompanyId) : IQuery<GetHomePageTemplateResult>;
public record GetCompanyHomePageTemplateQuery(Guid CompanyId) : IQuery<GetHomePageTemplateResult>;
public record UpdateHomePageActiveTemplateCommand(Guid CompanyId, string ActiveTemplateKey) : ICommand<GetHomePageTemplateResult>;
public record UpdateHomePageTemplateContentCommand(Guid CompanyId, string TemplateKey, List<HomePageContentItemDto> ContentItems) : ICommand<GetHomePageTemplateResult>;
public record GetHomePageTemplateResult(HomePageTemplateDto HomePage);

public class GetPublicHomePageTemplateHandler(GeneralSettingsDbContext dbContext)
    : IQueryHandler<GetPublicHomePageTemplateQuery, GetHomePageTemplateResult>
{
    public async Task<GetHomePageTemplateResult> Handle(GetPublicHomePageTemplateQuery request, CancellationToken cancellationToken)
    {
        var activeTemplateKey = await ResolveActiveTemplateKeyAsync(dbContext, request.CompanyId, cancellationToken);
        var items = await HomePageTemplateStore.GetContentAsync(dbContext, request.CompanyId, activeTemplateKey, cancellationToken);

        if (items.Count == 0)
        {
            items = HomePageTemplateDefaults.GetDefaultContent(request.CompanyId, activeTemplateKey)
                .Select(seed => HomePageTemplateStore.FromSeed(seed, activeTemplateKey))
                .ToList();
        }

        return new GetHomePageTemplateResult(new HomePageTemplateDto
        {
            CompanyId = request.CompanyId,
            ActiveTemplateKey = activeTemplateKey,
            Templates = HomePageTemplateDefaults.Templates.ToList(),
            ContentItems = items.OrderBy(x => x.SortOrder).ToList()
        });
    }

    private static async Task<string> ResolveActiveTemplateKeyAsync(GeneralSettingsDbContext dbContext, Guid companyId, CancellationToken cancellationToken)
    {
        var selection = await dbContext.HomePageTemplateSelections
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);

        return HomePageTemplateKeys.IsValid(selection?.ActiveTemplateKey)
            ? selection!.ActiveTemplateKey
            : HomePageTemplateKeys.CurrentStorefront;
    }
}

public class GetCompanyHomePageTemplateHandler(GeneralSettingsDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetCompanyHomePageTemplateQuery, GetHomePageTemplateResult>
{
    public async Task<GetHomePageTemplateResult> Handle(GetCompanyHomePageTemplateQuery request, CancellationToken cancellationToken)
    {
        var userId = HomePageTemplateStore.GetUserId(httpContextAccessor);
        await HomePageTemplateStore.EnsureCompanyTemplateRowsAsync(dbContext, request.CompanyId, userId, cancellationToken);

        var activeTemplateKey = await dbContext.HomePageTemplateSelections
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .Select(x => x.ActiveTemplateKey)
            .FirstAsync(cancellationToken);

        var contentItems = new List<HomePageContentItemDto>();
        foreach (var templateKey in HomePageTemplateKeys.All)
        {
            contentItems.AddRange(await HomePageTemplateStore.GetContentAsync(dbContext, request.CompanyId, templateKey, cancellationToken));
        }

        return new GetHomePageTemplateResult(new HomePageTemplateDto
        {
            CompanyId = request.CompanyId,
            ActiveTemplateKey = activeTemplateKey,
            Templates = HomePageTemplateDefaults.Templates.ToList(),
            ContentItems = contentItems.OrderBy(x => x.TemplateKey).ThenBy(x => x.SortOrder).ToList()
        });
    }
}

public class UpdateHomePageActiveTemplateCommandValidator : AbstractValidator<UpdateHomePageActiveTemplateCommand>
{
    public UpdateHomePageActiveTemplateCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.ActiveTemplateKey)
            .Must(HomePageTemplateKeys.IsValid)
            .WithMessage("Invalid home page template key.");
    }
}

public class UpdateHomePageActiveTemplateHandler(GeneralSettingsDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateHomePageActiveTemplateCommand, GetHomePageTemplateResult>
{
    public async Task<GetHomePageTemplateResult> Handle(UpdateHomePageActiveTemplateCommand request, CancellationToken cancellationToken)
    {
        var userId = HomePageTemplateStore.GetUserId(httpContextAccessor);
        await HomePageTemplateStore.EnsureCompanyTemplateRowsAsync(dbContext, request.CompanyId, userId, cancellationToken);

        var selection = await dbContext.HomePageTemplateSelections
            .FirstAsync(x => x.CompanyId == request.CompanyId, cancellationToken);

        selection.SetActiveTemplate(request.ActiveTemplateKey, userId);
        await dbContext.SaveChangesAsync(cancellationToken);

        var contentItems = await HomePageTemplateStore.GetContentAsync(dbContext, request.CompanyId, request.ActiveTemplateKey, cancellationToken);

        return new GetHomePageTemplateResult(new HomePageTemplateDto
        {
            CompanyId = request.CompanyId,
            ActiveTemplateKey = selection.ActiveTemplateKey,
            Templates = HomePageTemplateDefaults.Templates.ToList(),
            ContentItems = contentItems.OrderBy(x => x.SortOrder).ToList()
        });
    }
}

public class UpdateHomePageTemplateContentCommandValidator : AbstractValidator<UpdateHomePageTemplateContentCommand>
{
    public UpdateHomePageTemplateContentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.TemplateKey).Must(HomePageTemplateKeys.IsValid).WithMessage("Invalid home page template key.");
        RuleForEach(x => x.ContentItems).ChildRules(item =>
        {
            item.RuleFor(x => x.SectionKey).NotEmpty().MaximumLength(80);
            item.RuleFor(x => x.FieldKey).NotEmpty().MaximumLength(80);
            item.RuleFor(x => x.ContentType).NotEmpty().MaximumLength(20);
            item.RuleFor(x => x.TextEn).MaximumLength(2000);
            item.RuleFor(x => x.TextAr).MaximumLength(2000);
            item.RuleFor(x => x.ImagePath).MaximumLength(5000000);
            item.RuleFor(x => x.AltTextEn).MaximumLength(300);
            item.RuleFor(x => x.AltTextAr).MaximumLength(300);
        });
    }
}

public class UpdateHomePageTemplateContentHandler(GeneralSettingsDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateHomePageTemplateContentCommand, GetHomePageTemplateResult>
{
    public async Task<GetHomePageTemplateResult> Handle(UpdateHomePageTemplateContentCommand request, CancellationToken cancellationToken)
    {
        var userId = HomePageTemplateStore.GetUserId(httpContextAccessor);
        await HomePageTemplateStore.EnsureCompanyTemplateRowsAsync(dbContext, request.CompanyId, userId, cancellationToken);
        await HomePageTemplateStore.UpdateContentAsync(dbContext, request.CompanyId, request.TemplateKey, request.ContentItems, userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var activeTemplateKey = await dbContext.HomePageTemplateSelections
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .Select(x => x.ActiveTemplateKey)
            .FirstAsync(cancellationToken);

        var contentItems = await HomePageTemplateStore.GetContentAsync(dbContext, request.CompanyId, request.TemplateKey, cancellationToken);

        return new GetHomePageTemplateResult(new HomePageTemplateDto
        {
            CompanyId = request.CompanyId,
            ActiveTemplateKey = activeTemplateKey,
            Templates = HomePageTemplateDefaults.Templates.ToList(),
            ContentItems = contentItems.OrderBy(x => x.SortOrder).ToList()
        });
    }
}

internal static class HomePageTemplateStore
{
    public static string GetUserId(IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static async Task EnsureCompanyTemplateRowsAsync(
        GeneralSettingsDbContext dbContext,
        Guid companyId,
        string userId,
        CancellationToken cancellationToken)
    {
        var selectionExists = await dbContext.HomePageTemplateSelections
            .AnyAsync(x => x.CompanyId == companyId, cancellationToken);

        if (!selectionExists)
        {
            await dbContext.HomePageTemplateSelections.AddAsync(
                HomePageTemplateSelection.Create(Guid.NewGuid(), companyId, HomePageTemplateKeys.CurrentStorefront, userId),
                cancellationToken);
        }

        foreach (var templateKey in HomePageTemplateKeys.All)
        {
            var existingKeys = await GetExistingKeysAsync(dbContext, companyId, templateKey, cancellationToken);
            var missingRows = HomePageTemplateDefaults.GetDefaultContent(companyId, templateKey)
                .Where(seed => !existingKeys.Contains(Key(seed.SectionKey, seed.FieldKey)))
                .Select(seed => CreateContent(templateKey, seed, userId))
                .ToList();

            if (missingRows.Count > 0)
            {
                dbContext.AddRange(missingRows);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public static async Task<List<HomePageContentItemDto>> GetContentAsync(
        GeneralSettingsDbContext dbContext,
        Guid companyId,
        string templateKey,
        CancellationToken cancellationToken)
        => templateKey switch
        {
            HomePageTemplateKeys.CorporateShowcase => await QueryContent<CorporateShowcaseHomePageContent>(dbContext, companyId, templateKey, cancellationToken),
            HomePageTemplateKeys.ProductHighlight => await QueryContent<ProductHighlightHomePageContent>(dbContext, companyId, templateKey, cancellationToken),
            HomePageTemplateKeys.CampaignLanding => await QueryContent<CampaignLandingHomePageContent>(dbContext, companyId, templateKey, cancellationToken),
            HomePageTemplateKeys.MinimalCatalog => await QueryContent<MinimalCatalogHomePageContent>(dbContext, companyId, templateKey, cancellationToken),
            _ => await QueryContent<CurrentStorefrontHomePageContent>(dbContext, companyId, templateKey, cancellationToken)
        };

    public static HomePageContentItemDto FromSeed(HomePageContentSeed seed, string templateKey)
        => ToDto(seed, templateKey);

    public static async Task UpdateContentAsync(
        GeneralSettingsDbContext dbContext,
        Guid companyId,
        string templateKey,
        List<HomePageContentItemDto> contentItems,
        string userId,
        CancellationToken cancellationToken)
    {
        switch (templateKey)
        {
            case HomePageTemplateKeys.CorporateShowcase:
                await UpdateContentSetAsync<CorporateShowcaseHomePageContent>(dbContext, companyId, templateKey, contentItems, userId, cancellationToken);
                break;
            case HomePageTemplateKeys.ProductHighlight:
                await UpdateContentSetAsync<ProductHighlightHomePageContent>(dbContext, companyId, templateKey, contentItems, userId, cancellationToken);
                break;
            case HomePageTemplateKeys.CampaignLanding:
                await UpdateContentSetAsync<CampaignLandingHomePageContent>(dbContext, companyId, templateKey, contentItems, userId, cancellationToken);
                break;
            case HomePageTemplateKeys.MinimalCatalog:
                await UpdateContentSetAsync<MinimalCatalogHomePageContent>(dbContext, companyId, templateKey, contentItems, userId, cancellationToken);
                break;
            default:
                await UpdateContentSetAsync<CurrentStorefrontHomePageContent>(dbContext, companyId, templateKey, contentItems, userId, cancellationToken);
                break;
        }
    }

    private static async Task<HashSet<string>> GetExistingKeysAsync(
        GeneralSettingsDbContext dbContext,
        Guid companyId,
        string templateKey,
        CancellationToken cancellationToken)
    {
        var items = await GetContentAsync(dbContext, companyId, templateKey, cancellationToken);
        return items.Select(x => Key(x.SectionKey, x.FieldKey)).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static async Task<List<HomePageContentItemDto>> QueryContent<TEntity>(
        GeneralSettingsDbContext dbContext,
        Guid companyId,
        string templateKey,
        CancellationToken cancellationToken)
        where TEntity : HomePageTemplateContent
    {
        var rows = await dbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        return rows.Select(row => ToDto(row, templateKey)).ToList();
    }

    private static async Task UpdateContentSetAsync<TEntity>(
        GeneralSettingsDbContext dbContext,
        Guid companyId,
        string templateKey,
        List<HomePageContentItemDto> contentItems,
        string userId,
        CancellationToken cancellationToken)
        where TEntity : HomePageTemplateContent
    {
        var rows = await dbContext.Set<TEntity>()
            .Where(x => x.CompanyId == companyId)
            .ToListAsync(cancellationToken);

        var rowByKey = rows.ToDictionary(x => Key(x.SectionKey, x.FieldKey), StringComparer.OrdinalIgnoreCase);

        foreach (var item in contentItems.Where(x => x.CompanyId == Guid.Empty || x.CompanyId == companyId))
        {
            var rowKey = Key(item.SectionKey, item.FieldKey);
            if (!rowByKey.TryGetValue(rowKey, out var row))
                continue;

            var imagePath = NormalizeImagePath(companyId, templateKey, item, row.ImagePath);
            row.Update(
                item.ContentType,
                item.TextEn ?? string.Empty,
                item.TextAr ?? string.Empty,
                imagePath,
                item.AltTextEn ?? string.Empty,
                item.AltTextAr ?? string.Empty,
                item.SortOrder,
                item.IsVisible,
                userId);
        }
    }

    private static string NormalizeImagePath(Guid companyId, string templateKey, HomePageContentItemDto item, string currentImagePath)
    {
        if (!string.Equals(item.ContentType, "Image", StringComparison.OrdinalIgnoreCase))
            return item.ImagePath ?? string.Empty;

        if (string.IsNullOrWhiteSpace(item.ImagePath))
            return string.Empty;

        if (!SaveImages.IsBase64Image(item.ImagePath))
            return item.ImagePath;

        var fileName = SaveImages.SaveBase64Image(
            $"{item.SectionKey}-{item.FieldKey}-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            ["wwwroot", "Images", "HomePage", companyId.ToString("N"), templateKey],
            item.ImagePath);

        return fileName;
    }

    private static HomePageTemplateContent CreateContent(string templateKey, HomePageContentSeed seed, string createdBy)
        => templateKey switch
        {
            HomePageTemplateKeys.CorporateShowcase => CorporateShowcaseHomePageContent.Create(seed, createdBy),
            HomePageTemplateKeys.ProductHighlight => ProductHighlightHomePageContent.Create(seed, createdBy),
            HomePageTemplateKeys.CampaignLanding => CampaignLandingHomePageContent.Create(seed, createdBy),
            HomePageTemplateKeys.MinimalCatalog => MinimalCatalogHomePageContent.Create(seed, createdBy),
            _ => CurrentStorefrontHomePageContent.Create(seed, createdBy)
        };

    private static HomePageContentItemDto ToDto(HomePageTemplateContent row, string templateKey)
        => new()
        {
            Id = row.Id,
            CompanyId = row.CompanyId,
            TemplateKey = templateKey,
            SectionKey = row.SectionKey,
            FieldKey = row.FieldKey,
            ContentType = row.ContentType,
            TextEn = row.TextEn,
            TextAr = row.TextAr,
            ImagePath = row.ImagePath,
            AltTextEn = row.AltTextEn,
            AltTextAr = row.AltTextAr,
            ImageUrl = ResolveImageUrl(row.CompanyId, templateKey, row.ImagePath),
            SortOrder = row.SortOrder,
            IsVisible = row.IsVisible
        };

    private static HomePageContentItemDto ToDto(HomePageContentSeed seed, string templateKey)
        => new()
        {
            Id = Guid.Empty,
            CompanyId = seed.CompanyId,
            TemplateKey = templateKey,
            SectionKey = seed.SectionKey,
            FieldKey = seed.FieldKey,
            ContentType = seed.ContentType,
            TextEn = seed.TextEn,
            TextAr = seed.TextAr,
            ImagePath = seed.ImagePath,
            AltTextEn = seed.AltTextEn,
            AltTextAr = seed.AltTextAr,
            ImageUrl = ResolveImageUrl(seed.CompanyId, templateKey, seed.ImagePath),
            SortOrder = seed.SortOrder,
            IsVisible = seed.IsVisible
        };

    private static string ResolveImageUrl(Guid companyId, string templateKey, string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return string.Empty;

        if (imagePath.StartsWith("_content/", StringComparison.OrdinalIgnoreCase)
            || Uri.TryCreate(imagePath, UriKind.Absolute, out _))
        {
            return imagePath;
        }

        if (imagePath.StartsWith("/Images/", StringComparison.OrdinalIgnoreCase))
            return imagePath;

        return $"/Images/HomePage/{companyId:N}/{templateKey}/{imagePath}";
    }

    private static string Key(string sectionKey, string fieldKey)
        => $"{sectionKey.Trim()}::{fieldKey.Trim()}";
}

