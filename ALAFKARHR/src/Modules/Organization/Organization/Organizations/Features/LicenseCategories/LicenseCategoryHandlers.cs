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
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Notes).MaximumLength(1000);
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

        return new GetLicenseCategoriesResult(categories.Select(ToDto).ToList());
    }

    public static LicenseCategoryDto ToDto(LicenseCategory category) => new()
    {
        Id = category.Id,
        Key = category.Key,
        Name = category.Name,
        MaxUsers = category.MaxUsers,
        MaxChildCompanies = category.MaxChildCompanies,
        MaxBranches = category.MaxBranches,
        MonthlyPrice = category.MonthlyPrice,
        YearlyPrice = category.YearlyPrice,
        CurrencyCode = category.CurrencyCode,
        IsActive = category.IsActive,
        Notes = category.Notes
    };
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
            request.Category.CurrencyCode,
            request.Category.Notes,
            GetUserId());

        await dbContext.LicenseCategories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateLicenseCategoryResult(LicenseCategoryQueryHandler.ToDto(category));
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
            request.Category.CurrencyCode,
            request.Category.Notes,
            GetUserId());

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

    private string GetUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not authenticated");
}
