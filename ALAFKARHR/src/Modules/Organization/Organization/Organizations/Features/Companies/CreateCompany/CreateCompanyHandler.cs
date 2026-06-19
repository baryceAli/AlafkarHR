using Auth.Contracts.Features.CreateCompanyAdmin;

namespace Organization.Organizations.Features.Companies.CreateCompany;

public record CreateCompanyCommand(CompanyDto Company) : ICommand<CreateCompanyResult>;
public record CreateCompanyResult(CompanyDto CreatedCompany);

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Company.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Company.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.Company.Code).NotEmpty().WithMessage("Code is required");
        RuleFor(x => x.Company.CurrencyId).NotEmpty().WithMessage("Currency is required");
        RuleFor(x => x.Company.HqLocation).NotEmpty().WithMessage("HqLocation is required");
        RuleFor(x => x.Company.VatNo).NotEmpty().WithMessage("VatNo is required");
        RuleFor(x => x.Company.AdminUserName).NotEmpty().WithMessage("AdminUserName is required");
        RuleFor(x => x.Company.AdminEmail).NotEmpty().EmailAddress().WithMessage("AdminEmail is required");
        RuleFor(x => x.Company.AdminPhoneNumber).NotEmpty().WithMessage("AdminPhoneNumber is required");
        RuleFor(x => x.Company.AdminTemporaryPassword).NotEmpty().WithMessage("AdminTemporaryPassword is required");
    }
}

public class CreateCompanyHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateCompanyCommand, CreateCompanyResult>
{
    public async Task<CreateCompanyResult> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var currentCompanyIdValue = httpContextAccessor.HttpContext?
            .User?
            .FindFirst("company_id")?
            .Value;

        if (!Guid.TryParse(currentCompanyIdValue, out var currentCompanyId))
            throw new UnauthorizedAccessException("Current user is not linked to a company");

        var currentCompany = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentCompanyId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Current user's company was not found");

        if (currentCompany.ParentCompanyId.HasValue)
            throw new UnauthorizedAccessException("Child companies are not allowed to create child companies");

        if (request.Company.ParentCompanyId.HasValue)
        {
            if (request.Company.ParentCompanyId.Value != currentCompanyId)
                throw new UnauthorizedAccessException("Child companies can only be created under the current parent company");

            var parentExists = await dbContext.Companies
                .AnyAsync(x => x.Id == request.Company.ParentCompanyId.Value, cancellationToken);

            if (!parentExists)
                throw new NotFoundException($"Parent company not found: {request.Company.ParentCompanyId.Value}");

            await EnsureChildCompanyLimitAsync(request.Company.ParentCompanyId.Value, cancellationToken);
        }

        var company = Models.Company.Create(
            Guid.NewGuid(),
            request.Company.ParentCompanyId,
            request.Company.Name,
            request.Company.NameEng,
            request.Company.Logo,
            request.Company.HqLocation,
            request.Company.HqLongitude,
            request.Company.HqLatitude,
            request.Company.VatNo,
            request.Company.Code,
            request.Company.CurrencyId!.Value,
            request.Company.Email,
            request.Company.Phone,
            request.Company.TimeZone,
            userId);

        await dbContext.Companies.AddAsync(company, cancellationToken);
        await dbContext.SaveChangesAsync();

        try
        {
            await sender.Send(
                new CreateCompanyAdminCommand(
                    company.Id,
                    company.Code,
                    request.Company.AdminUserName!,
                    request.Company.AdminEmail!,
                    request.Company.AdminPhoneNumber!,
                    request.Company.AdminTemporaryPassword!),
                cancellationToken);
        }
        catch
        {
            dbContext.Companies.Remove(company);
            await dbContext.SaveChangesAsync();
            throw;
        }

        return new CreateCompanyResult(company.Adapt<CompanyDto>());
    }

    private async Task EnsureChildCompanyLimitAsync(Guid parentCompanyId, CancellationToken cancellationToken)
    {
        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken);

        if (license is null)
            return;

        if (!license.AllowsAccess(DateTime.UtcNow))
            throw new UnauthorizedAccessException("Parent company license is not active");

        var childCompaniesCount = await dbContext.Companies
            .AsNoTracking()
            .CountAsync(x => x.ParentCompanyId == parentCompanyId, cancellationToken);

        if (childCompaniesCount >= license.MaxChildCompanies)
            throw new InvalidOperationException("Parent company child-company license limit has been reached");
    }
}
