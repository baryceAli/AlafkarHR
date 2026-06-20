using Auth.Contracts.Features.CountUsersByCompanyIds;
using Auth.Contracts.Features.CreateCompanyAdmin;
using Auth.Contracts.Features.GetCompanyAdmin;
using Auth.Contracts.Features.ResetCompanyAdminPassword;
using Shared.Contracts.GeneralSettings.Currencies;
using SharedWithUI.Organization.Enums;

namespace Organization.Organizations.Features.ParentCompanies;

public record GetParentCompaniesQuery(PaginationRequest PaginationRequest) : IQuery<GetParentCompaniesResult>;
public record GetParentCompaniesResult(PaginatedResult<ParentCompanyDto> CompanyList);
public record GetParentCompanyByIdQuery(Guid Id) : IQuery<GetParentCompanyByIdResult>;
public record GetParentCompanyByIdResult(ParentCompanyDto Company);
public record CreateParentCompanyCommand(ParentCompanyDto Company) : ICommand<CreateParentCompanyResult>;
public record CreateParentCompanyResult(ParentCompanyDto CreatedCompany);
public record UpdateParentCompanyCommand(ParentCompanyDto Company) : ICommand<UpdateParentCompanyResult>;
public record UpdateParentCompanyResult(bool IsSuccess);
public record UpdateParentCompanyLicenseCommand(Guid CompanyId, CompanyLicenseDto License) : ICommand<UpdateParentCompanyLicenseResult>;
public record UpdateParentCompanyLicenseResult(bool IsSuccess);
public record SetParentCompanyStatusCommand(Guid CompanyId, bool IsActive) : ICommand<SetParentCompanyStatusResult>;
public record SetParentCompanyStatusResult(bool IsSuccess);
public record ResetParentCompanyAdminPasswordCommand(Guid CompanyId, string TemporaryPassword) : ICommand<ResetParentCompanyAdminPasswordResult>;
public record ResetParentCompanyAdminPasswordResult(bool IsSuccess);
public record DeleteParentCompanyCommand(Guid CompanyId) : ICommand<DeleteParentCompanyResult>;
public record DeleteParentCompanyResult(bool IsSuccess);

public class ParentCompanyValidator : AbstractValidator<ParentCompanyDto>
{
    public ParentCompanyValidator() : this(false)
    {
    }

    public ParentCompanyValidator(bool requireAdmin)
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.NameEng).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.HqLocation).NotEmpty();
        RuleFor(x => x.VatNo).NotEmpty();
        RuleFor(x => x.License.LicenseCategoryId).NotEmpty();
        RuleFor(x => x.License.EndDate).GreaterThanOrEqualTo(x => x.License.StartDate);

        if (requireAdmin)
        {
            RuleFor(x => x.AdminUserName).NotEmpty();
            RuleFor(x => x.AdminEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.AdminPhoneNumber).NotEmpty();
            RuleFor(x => x.AdminTemporaryPassword).NotEmpty();
        }
    }
}

public class CreateParentCompanyCommandValidator : AbstractValidator<CreateParentCompanyCommand>
{
    public CreateParentCompanyCommandValidator()
    {
        RuleFor(x => x.Company).SetValidator(new ParentCompanyValidator(true));
    }
}

public class UpdateParentCompanyCommandValidator : AbstractValidator<UpdateParentCompanyCommand>
{
    public UpdateParentCompanyCommandValidator()
    {
        RuleFor(x => x.Company.Id).NotEmpty();
        RuleFor(x => x.Company.Name).NotEmpty();
        RuleFor(x => x.Company.NameEng).NotEmpty();
        RuleFor(x => x.Company.Code).NotEmpty();
        RuleFor(x => x.Company.HqLocation).NotEmpty();
        RuleFor(x => x.Company.VatNo).NotEmpty();
    }
}

public class UpdateParentCompanyLicenseCommandValidator : AbstractValidator<UpdateParentCompanyLicenseCommand>
{
    public UpdateParentCompanyLicenseCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.License.LicenseCategoryId).NotEmpty();
        RuleFor(x => x.License.EndDate).GreaterThanOrEqualTo(x => x.License.StartDate);
    }
}

public class ParentCompanyQueryHandler(OrganizationDbContext dbContext, ISender sender)
    : IQueryHandler<GetParentCompaniesQuery, GetParentCompaniesResult>,
      IQueryHandler<GetParentCompanyByIdQuery, GetParentCompanyByIdResult>
{
    public async Task<GetParentCompaniesResult> Handle(GetParentCompaniesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Companies
            .AsNoTracking()
            .Where(x => x.ParentCompanyId == null);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.Trim();
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.NameEng.Contains(search) ||
                x.Code.Contains(search) ||
                x.VatNo.Contains(search) ||
                x.Email.Contains(search) ||
                x.Phone.Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var companies = await query
            .OrderBy(x => x.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = new List<ParentCompanyDto>();
        foreach (var company in companies)
            dtos.Add(await MapAsync(company, cancellationToken));

        return new GetParentCompaniesResult(new PaginatedResult<ParentCompanyDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            dtos));
    }

    public async Task<GetParentCompanyByIdResult> Handle(GetParentCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.ParentCompanyId == null, cancellationToken)
            ?? throw new NotFoundException($"Parent company not found: {request.Id}");

        return new GetParentCompanyByIdResult(await MapAsync(company, cancellationToken));
    }

    private async Task<ParentCompanyDto> MapAsync(Company company, CancellationToken cancellationToken)
    {
        var childCompanyIds = await dbContext.Companies
            .AsNoTracking()
            .Where(x => x.ParentCompanyId == company.Id)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var hierarchyIds = new List<Guid> { company.Id };
        hierarchyIds.AddRange(childCompanyIds);

        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .Include(x => x.LicenseCategory)
            .FirstOrDefaultAsync(x => x.CompanyId == company.Id, cancellationToken);

        var branchesCount = await dbContext.Branches
            .AsNoTracking()
            .CountAsync(x => hierarchyIds.Contains(x.CompanyId), cancellationToken);

        var usersCount = await sender.Send(new CountUsersByCompanyIdsQuery(hierarchyIds), cancellationToken);
        var dto = ToDto(company, license);
        dto.ChildCompaniesCount = childCompanyIds.Count;
        dto.BranchesCount = branchesCount;
        dto.UsersCount = usersCount.Count;

        try
        {
            var admin = await sender.Send(new GetCompanyAdminQuery(company.Id), cancellationToken);
            dto.AdminUserName = admin.UserName;
            dto.AdminEmail = admin.Email;
            dto.AdminPhoneNumber = admin.PhoneNumber;
        }
        catch
        {
            dto.AdminUserName = string.Empty;
        }

        return dto;
    }

    public static ParentCompanyDto ToDto(Company company, CompanyLicense? license)
    {
        var licenseDto = license is null
            ? new CompanyLicenseDto { CompanyId = company.Id, PlanKey = "legacy", PlanName = "Legacy" }
            : ToLicenseDto(license);

        return new ParentCompanyDto
        {
            Id = company.Id,
            IsActive = company.IsActive,
            Name = company.Name,
            NameEng = company.NameEng,
            Logo = company.Logo,
            HqLocation = company.HqLocation,
            HqLongitude = company.HqLongitude,
            HqLatitude = company.HqLatitude,
            VatNo = company.VatNo,
            Code = company.Code,
            CurrencyId = company.CurrencyId,
            TimeZone = company.TimeZone,
            Phone = company.Phone,
            Email = company.Email,
            License = licenseDto
        };
    }

    private static CompanyLicenseDto ToLicenseDto(CompanyLicense license) => new()
    {
        Id = license.Id,
        CompanyId = license.CompanyId,
        LicenseCategoryId = license.LicenseCategoryId,
        Status = license.Status,
        PlanKey = license.EffectivePlanKey,
        PlanName = license.EffectivePlanName,
        StartDate = license.StartDate,
        EndDate = license.EndDate,
        MaxUsers = license.EffectiveMaxUsers,
        MaxChildCompanies = license.EffectiveMaxChildCompanies,
        MaxBranches = license.EffectiveMaxBranches,
        MonthlyPrice = license.EffectiveMonthlyPrice,
        YearlyPrice = license.EffectiveYearlyPrice,
        CurrencyCode = license.EffectiveCurrencyCode,
        Notes = license.Notes
    };
}

public class CreateParentCompanyHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateParentCompanyCommand, CreateParentCompanyResult>
{
    public async Task<CreateParentCompanyResult> Handle(CreateParentCompanyCommand request, CancellationToken cancellationToken)
    {
        var userId = GetUserId(httpContextAccessor);
        var companyId = Guid.NewGuid();
        var currencyResult = await sender.Send(new EnsureCompanyInitialCurrenciesCommand(companyId, userId), cancellationToken);

        var company = Company.Create(
            companyId,
            null,
            request.Company.Name,
            request.Company.NameEng,
            request.Company.Logo,
            request.Company.HqLocation,
            request.Company.HqLongitude,
            request.Company.HqLatitude,
            request.Company.VatNo,
            request.Company.Code,
            currencyResult.DefaultCurrencyId,
            request.Company.Email,
            request.Company.Phone,
            request.Company.TimeZone,
            userId);

        var category = await GetCategoryAsync(request.Company.License.LicenseCategoryId, requireActive: true, cancellationToken);
        var license = BuildLicense(company.Id, request.Company.License, category, userId);
        try
        {
            await dbContext.Companies.AddAsync(company, cancellationToken);
            await dbContext.CompanyLicenses.AddAsync(license, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await sender.Send(
                new CreateCompanyAdminCommand(
                    company.Id,
                    company.Code,
                    request.Company.AdminUserName!,
                    request.Company.AdminEmail!,
                    request.Company.AdminPhoneNumber!,
                    request.Company.AdminTemporaryPassword!,
                    CompanyAdminScope.ParentCompanyAdministration),
                cancellationToken);
        }
        catch
        {
            dbContext.CompanyLicenses.Remove(license);
            dbContext.Companies.Remove(company);
            await dbContext.SaveChangesAsync(cancellationToken);
            await sender.Send(new RemoveCompanyCurrenciesCommand(company.Id), cancellationToken);
            throw;
        }

        return new CreateParentCompanyResult(ParentCompanyQueryHandler.ToDto(company, license));
    }

    private static string GetUserId(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not authenticated");

    private async Task<LicenseCategory> GetCategoryAsync(Guid? categoryId, bool requireActive, CancellationToken cancellationToken)
    {
        if (!categoryId.HasValue)
            throw new ArgumentException("License category is required");

        var category = await dbContext.LicenseCategories
            .FirstOrDefaultAsync(x => x.Id == categoryId.Value, cancellationToken)
            ?? throw new NotFoundException($"License category not found: {categoryId.Value}");

        if (requireActive && !category.IsActive)
            throw new InvalidOperationException("Selected license category is not active");

        return category;
    }

    private static CompanyLicense BuildLicense(Guid companyId, CompanyLicenseDto dto, LicenseCategory category, string userId) =>
        CompanyLicense.Create(
            Guid.NewGuid(),
            companyId,
            dto.Status,
            category.Key,
            category.Name,
            dto.StartDate,
            dto.EndDate,
            category.MaxUsers,
            category.MaxChildCompanies,
            category.MaxBranches,
            dto.Notes,
            userId,
            category.Id);
}

public class UpdateParentCompanyHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpdateParentCompanyCommand, UpdateParentCompanyResult>,
      ICommandHandler<UpdateParentCompanyLicenseCommand, UpdateParentCompanyLicenseResult>,
      ICommandHandler<SetParentCompanyStatusCommand, SetParentCompanyStatusResult>,
      ICommandHandler<DeleteParentCompanyCommand, DeleteParentCompanyResult>
{
    public async Task<UpdateParentCompanyResult> Handle(UpdateParentCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await FindParentCompanyAsync(request.Company.Id, cancellationToken);
        var userId = GetUserId(httpContextAccessor);

        company.UpdateManagementInfo(
            request.Company.Name,
            request.Company.NameEng,
            request.Company.Logo,
            request.Company.HqLocation,
            request.Company.HqLongitude,
            request.Company.HqLatitude,
            request.Company.VatNo,
            request.Company.Code,
            company.CurrencyId,
            request.Company.Email,
            request.Company.Phone,
            request.Company.TimeZone,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateParentCompanyResult(true);
    }

    public async Task<UpdateParentCompanyLicenseResult> Handle(UpdateParentCompanyLicenseCommand request, CancellationToken cancellationToken)
    {
        await FindParentCompanyAsync(request.CompanyId, cancellationToken);
        await UpsertLicenseAsync(request.CompanyId, request.License, GetUserId(httpContextAccessor), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateParentCompanyLicenseResult(true);
    }

    public async Task<SetParentCompanyStatusResult> Handle(SetParentCompanyStatusCommand request, CancellationToken cancellationToken)
    {
        var company = await FindParentCompanyAsync(request.CompanyId, cancellationToken);
        company.SetActive(request.IsActive, GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SetParentCompanyStatusResult(true);
    }

    public async Task<DeleteParentCompanyResult> Handle(DeleteParentCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await FindParentCompanyAsync(request.CompanyId, cancellationToken);
        var childCompanyIds = await dbContext.Companies
            .AsNoTracking()
            .Where(x => x.ParentCompanyId == request.CompanyId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var hierarchyIds = new List<Guid> { request.CompanyId };
        hierarchyIds.AddRange(childCompanyIds);

        var hasChildren = childCompanyIds.Count > 0;
        var hasBranches = await dbContext.Branches.AnyAsync(x => hierarchyIds.Contains(x.CompanyId), cancellationToken);
        var usersCount = await sender.Send(new CountUsersByCompanyIdsQuery(hierarchyIds), cancellationToken);
        if (hasChildren || hasBranches || usersCount.Count > 0)
            throw new InvalidOperationException("Parent company cannot be deleted while child companies, branches, or users exist");

        company.Remove(GetUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteParentCompanyResult(true);
    }

    private async Task<Company> FindParentCompanyAsync(Guid companyId, CancellationToken cancellationToken) =>
        await dbContext.Companies.FirstOrDefaultAsync(x => x.Id == companyId && x.ParentCompanyId == null, cancellationToken)
        ?? throw new NotFoundException($"Parent company not found: {companyId}");

    private async Task UpsertLicenseAsync(Guid companyId, CompanyLicenseDto dto, string userId, CancellationToken cancellationToken)
    {
        var license = await dbContext.CompanyLicenses.FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        var requireActiveCategory = license is null || license.LicenseCategoryId != dto.LicenseCategoryId;
        var category = await GetCategoryAsync(dto.LicenseCategoryId, requireActiveCategory, cancellationToken);
        if (license is null)
        {
            license = CompanyLicense.Create(
                Guid.NewGuid(),
                companyId,
                dto.Status,
                category.Key,
                category.Name,
                dto.StartDate,
                dto.EndDate,
                category.MaxUsers,
                category.MaxChildCompanies,
                category.MaxBranches,
                dto.Notes,
                userId,
                category.Id);
            await dbContext.CompanyLicenses.AddAsync(license, cancellationToken);
            return;
        }

        license.Update(
            dto.Status,
            category.Key,
            category.Name,
            dto.StartDate,
            dto.EndDate,
            category.MaxUsers,
            category.MaxChildCompanies,
            category.MaxBranches,
            dto.Notes,
            userId,
            category.Id);
    }

    private async Task<LicenseCategory> GetCategoryAsync(Guid? categoryId, bool requireActive, CancellationToken cancellationToken)
    {
        if (!categoryId.HasValue)
            throw new ArgumentException("License category is required");

        var category = await dbContext.LicenseCategories
            .FirstOrDefaultAsync(x => x.Id == categoryId.Value, cancellationToken)
            ?? throw new NotFoundException($"License category not found: {categoryId.Value}");

        if (requireActive && !category.IsActive)
            throw new InvalidOperationException("Selected license category is not active");

        return category;
    }

    private static string GetUserId(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not authenticated");
}

public class ResetParentCompanyAdminPasswordHandler(ISender sender)
    : ICommandHandler<ResetParentCompanyAdminPasswordCommand, ResetParentCompanyAdminPasswordResult>
{
    public async Task<ResetParentCompanyAdminPasswordResult> Handle(ResetParentCompanyAdminPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ResetCompanyAdminPasswordCommand(request.CompanyId, request.TemporaryPassword), cancellationToken);
        return new ResetParentCompanyAdminPasswordResult(result.IsSuccess);
    }
}
