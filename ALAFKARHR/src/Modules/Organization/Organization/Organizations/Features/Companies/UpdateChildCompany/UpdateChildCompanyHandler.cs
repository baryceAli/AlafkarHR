namespace Organization.Organizations.Features.Companies.UpdateChildCompany;

public record UpdateChildCompanyCommand(CompanyDto Company) : ICommand<UpdateChildCompanyResult>;
public record UpdateChildCompanyResult(bool IsSuccess);

public class UpdateChildCompanyCommandValidator : AbstractValidator<UpdateChildCompanyCommand>
{
    public UpdateChildCompanyCommandValidator()
    {
        RuleFor(x => x.Company.Id).NotEmpty();
        RuleFor(x => x.Company.Name).NotEmpty();
        RuleFor(x => x.Company.NameEng).NotEmpty();
        RuleFor(x => x.Company.Code).NotEmpty();
        RuleFor(x => x.Company.VatNo).NotEmpty();
        RuleFor(x => x.Company.CurrencyId).NotEmpty();
    }
}

public class UpdateChildCompanyHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateChildCompanyCommand, UpdateChildCompanyResult>
{
    public async Task<UpdateChildCompanyResult> Handle(UpdateChildCompanyCommand request, CancellationToken cancellationToken)
    {
        var parentCompanyId = await ResolveParentCompanyIdAsync(cancellationToken);
        var child = await dbContext.Companies
            .FirstOrDefaultAsync(x => x.Id == request.Company.Id && x.ParentCompanyId == parentCompanyId, cancellationToken)
            ?? throw new NotFoundException($"Child company not found: {request.Company.Id}");

        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");

        child.UpdateManagementInfo(
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

        await dbContext.SaveChangesAsync();
        return new UpdateChildCompanyResult(true);
    }

    private async Task<Guid> ResolveParentCompanyIdAsync(CancellationToken cancellationToken)
    {
        var companyIdValue = httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(companyIdValue, out var companyId))
            throw new UnauthorizedAccessException("Current user is not linked to a company");

        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Current user's company was not found");

        if (company.ParentCompanyId.HasValue)
            throw new UnauthorizedAccessException("Child companies cannot manage child companies");

        return companyId;
    }
}
