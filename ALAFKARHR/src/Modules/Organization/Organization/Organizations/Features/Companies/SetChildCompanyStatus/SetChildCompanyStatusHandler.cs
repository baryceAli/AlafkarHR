namespace Organization.Organizations.Features.Companies.SetChildCompanyStatus;

public record SetChildCompanyStatusCommand(Guid CompanyId, bool IsActive) : ICommand<SetChildCompanyStatusResult>;
public record SetChildCompanyStatusResult(bool IsSuccess);

public class SetChildCompanyStatusHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<SetChildCompanyStatusCommand, SetChildCompanyStatusResult>
{
    public async Task<SetChildCompanyStatusResult> Handle(SetChildCompanyStatusCommand request, CancellationToken cancellationToken)
    {
        var parentCompanyId = await ResolveParentCompanyIdAsync(cancellationToken);
        var child = await dbContext.Companies
            .FirstOrDefaultAsync(x => x.Id == request.CompanyId && x.ParentCompanyId == parentCompanyId, cancellationToken)
            ?? throw new NotFoundException($"Child company not found: {request.CompanyId}");

        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");

        child.SetActive(request.IsActive, userId);
        await dbContext.SaveChangesAsync();
        return new SetChildCompanyStatusResult(true);
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
