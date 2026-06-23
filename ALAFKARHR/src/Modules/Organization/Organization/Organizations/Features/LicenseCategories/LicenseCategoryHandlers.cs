using Organization.Organizations.Features.BusinessLines;

namespace Organization.Organizations.Features.LicenseCategories;

public record GetLicenseCategoriesQuery(bool IncludeInactive) : IQuery<GetLicenseCategoriesResult>;
public record GetLicenseCategoriesResult(List<LicenseCategoryDto> Categories);
public record CreateLicenseCategoryCommand(LicenseCategoryDto Category) : ICommand<CreateLicenseCategoryResult>;
public record CreateLicenseCategoryResult(LicenseCategoryDto Category);
public record UpdateLicenseCategoryCommand(LicenseCategoryDto Category) : ICommand<UpdateLicenseCategoryResult>;
public record UpdateLicenseCategoryResult(bool IsSuccess);
public record SetLicenseCategoryStatusCommand(Guid Id, bool IsActive) : ICommand<SetLicenseCategoryStatusResult>;
public record SetLicenseCategoryStatusResult(bool IsSuccess);

public class LicenseCategoryValidator : AbstractValidator<LicenseCategoryDto>
{
    public LicenseCategoryValidator()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MaxUsers).GreaterThan(0);
        RuleFor(x => x.MaxChildCompanies).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxBranches).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MonthlyPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.YearlyPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyId).NotEmpty();
        RuleFor(x => x.CurrencyCode).MaximumLength(10);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleForEach(x => x.BusinessLines).ChildRules(line =>
        {
            line.RuleFor(x => x.BusinessLineId).NotEmpty();
            line.RuleFor(x => x.ActivationLimit).GreaterThan(0);
        });
    }
}

public class CreateLicenseCategoryCommandValidator : AbstractValidator<CreateLicenseCategoryCommand>
{
    public CreateLicenseCategoryCommandValidator()
    {
        RuleFor(x => x.Category).SetValidator(new LicenseCategoryValidator());
    }
}

public class UpdateLicenseCategoryCommandValidator : AbstractValidator<UpdateLicenseCategoryCommand>
{
    public UpdateLicenseCategoryCommandValidator()
    {
        RuleFor(x => x.Category.Id).NotEmpty();
        RuleFor(x => x.Category).SetValidator(new LicenseCategoryValidator());
    }
}

public class LicenseCategoryQueryHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetLicenseCategoriesQuery, GetLicenseCategoriesResult>
{
    public async Task<GetLicenseCategoriesResult> Handle(GetLicenseCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.LicenseCategories.AsNoTracking();
        if (!request.IncludeInactive)
            query = query.Where(x => x.IsActive);

        var categories = await query
            .OrderBy(x => x.MonthlyPrice)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var businessLinesByCategory = await GetBusinessLinesByCategoryAsync(
            dbContext,
            categories.Select(category => category.Id).ToList(),
            cancellationToken);

        return new GetLicenseCategoriesResult(categories.Select(category => ToDto(category, businessLinesByCategory)).ToList());
    }

    public static LicenseCategoryDto ToDto(LicenseCategory category, IReadOnlyDictionary<Guid, List<LicensedBusinessLineDto>>? businessLinesByCategory = null) => new()
    {
        Id = category.Id,
        Key = category.Key,
        Name = category.Name,
        MaxUsers = category.MaxUsers,
        MaxChildCompanies = category.MaxChildCompanies,
        MaxBranches = category.MaxBranches,
        MonthlyPrice = category.MonthlyPrice,
        YearlyPrice = category.YearlyPrice,
        CurrencyId = category.CurrencyId,
        CurrencyCode = category.CurrencyCode,
        IsActive = category.IsActive,
        Notes = category.Notes,
        BusinessLines = businessLinesByCategory is not null && businessLinesByCategory.TryGetValue(category.Id, out var businessLines)
            ? businessLines
            : []
    };

    public static async Task<Dictionary<Guid, List<LicensedBusinessLineDto>>> GetBusinessLinesByCategoryAsync(
        OrganizationDbContext dbContext,
        IReadOnlyCollection<Guid> categoryIds,
        CancellationToken cancellationToken)
    {
        if (categoryIds.Count == 0)
            return [];

        var rows = await dbContext.LicenseCategoryBusinessLines
            .AsNoTracking()
            .Where(x => categoryIds.Contains(x.LicenseCategoryId))
            .Select(x => new { x.LicenseCategoryId, x.BusinessLine, x.ActivationLimit })
            .OrderBy(x => x.BusinessLine.DisplayOrder)
            .ThenBy(x => x.BusinessLine.Name)
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => x.LicenseCategoryId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x =>
                {
                    var dto = BusinessLineQueryHandler.ToLicensedDto(x.BusinessLine);
                    dto.ActivationLimit = x.ActivationLimit;
                    return dto;
                }).ToList());
    }
}

public class LicenseCategoryCommandHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateLicenseCategoryCommand, CreateLicenseCategoryResult>,
      ICommandHandler<UpdateLicenseCategoryCommand, UpdateLicenseCategoryResult>,
      ICommandHandler<SetLicenseCategoryStatusCommand, SetLicenseCategoryStatusResult>
{
    public async Task<CreateLicenseCategoryResult> Handle(CreateLicenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var normalizedKey = request.Category.Key.Trim().ToLowerInvariant();
        var keyExists = await dbContext.LicenseCategories.AnyAsync(x => x.Key == normalizedKey, cancellationToken);
        if (keyExists)
            throw new InvalidOperationException("License category key already exists");

        var category = LicenseCategory.Create(
            Guid.NewGuid(),
            request.Category.Key,
            request.Category.Name,
            request.Category.MaxUsers,
            request.Category.MaxChildCompanies,
            request.Category.MaxBranches,
            request.Category.MonthlyPrice,
            request.Category.YearlyPrice,
            request.Category.CurrencyId!.Value,
            request.Category.CurrencyCode,
            request.Category.Notes,
            GetUserId());

        await dbContext.LicenseCategories.AddAsync(category, cancellationToken);
        await SyncCategoryBusinessLinesAsync(category.Id, request.Category.BusinessLines, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var businessLinesByCategory = await LicenseCategoryQueryHandler.GetBusinessLinesByCategoryAsync(dbContext, [category.Id], cancellationToken);
        return new CreateLicenseCategoryResult(LicenseCategoryQueryHandler.ToDto(category, businessLinesByCategory));
    }

    public async Task<UpdateLicenseCategoryResult> Handle(UpdateLicenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await FindAsync(request.Category.Id, cancellationToken);
        var normalizedKey = request.Category.Key.Trim().ToLowerInvariant();
        var keyExists = await dbContext.LicenseCategories
            .AnyAsync(x => x.Id != request.Category.Id && x.Key == normalizedKey, cancellationToken);
        if (keyExists)
            throw new InvalidOperationException("License category key already exists");

        category.Update(
            request.Category.Key,
            request.Category.Name,
            request.Category.MaxUsers,
            request.Category.MaxChildCompanies,
            request.Category.MaxBranches,
            request.Category.MonthlyPrice,
            request.Category.YearlyPrice,
            request.Category.CurrencyId!.Value,
            request.Category.CurrencyCode,
            request.Category.Notes,
            GetUserId());

        await SyncCategoryBusinessLinesAsync(category.Id, request.Category.BusinessLines, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateLicenseCategoryResult(true);
    }

    public async Task<SetLicenseCategoryStatusResult> Handle(SetLicenseCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        var category = await FindAsync(request.Id, cancellationToken);
        category.SetActive(request.IsActive, GetUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SetLicenseCategoryStatusResult(true);
    }

    private async Task<LicenseCategory> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.LicenseCategories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new NotFoundException($"License category not found: {id}");

    private async Task SyncCategoryBusinessLinesAsync(Guid categoryId, List<LicensedBusinessLineDto> businessLines, CancellationToken cancellationToken)
    {
        var requestedLines = businessLines
            .Where(x => x.BusinessLineId != Guid.Empty)
            .GroupBy(x => x.BusinessLineId)
            .ToDictionary(x => x.Key, x => Math.Max(x.First().ActivationLimit, 1));
        var requestedIds = requestedLines.Keys.ToHashSet();

        if (requestedIds.Count > 0)
        {
            var existingBusinessLineIds = await dbContext.BusinessLines
                .Where(x => requestedIds.Contains(x.Id) && x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (existingBusinessLineIds.Count != requestedIds.Count)
                throw new InvalidOperationException("One or more selected business lines are inactive or unavailable");
        }

        var currentLinks = await dbContext.LicenseCategoryBusinessLines
            .Where(x => x.LicenseCategoryId == categoryId)
            .ToListAsync(cancellationToken);

        var linksToRemove = currentLinks.Where(x => !requestedIds.Contains(x.BusinessLineId)).ToList();
        dbContext.LicenseCategoryBusinessLines.RemoveRange(linksToRemove);

        var currentIds = currentLinks.Select(x => x.BusinessLineId).ToHashSet();
        foreach (var businessLineId in requestedIds.Where(id => !currentIds.Contains(id)))
        {
            await dbContext.LicenseCategoryBusinessLines.AddAsync(
                LicenseCategoryBusinessLine.Create(categoryId, businessLineId, requestedLines[businessLineId]),
                cancellationToken);
        }

        foreach (var link in currentLinks.Where(x => requestedIds.Contains(x.BusinessLineId)))
        {
            link.UpdateActivationLimit(requestedLines[link.BusinessLineId]);
        }
    }

    private string GetUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not authenticated");
}
