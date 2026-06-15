namespace Organization.Organizations.Features.Companies.SetChildCompanyStatus;

public record SetChildCompanyStatusCommand(Guid CompanyId, bool IsActive) : ICommand<SetChildCompanyStatusResult>;
public record SetChildCompanyStatusResult(bool IsSuccess);

public class SetChildCompanyStatusHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ICompanyHierarchyContext companyHierarchyContext)
    : ICommandHandler<SetChildCompanyStatusCommand, SetChildCompanyStatusResult>
{
    public async Task<SetChildCompanyStatusResult> Handle(SetChildCompanyStatusCommand request, CancellationToken cancellationToken)
    {
        var parentCompanyId = await companyHierarchyContext.GetCurrentParentCompanyIdAsync(cancellationToken);
        var child = await dbContext.Companies
            .FirstOrDefaultAsync(x => x.Id == request.CompanyId && x.ParentCompanyId == parentCompanyId, cancellationToken)
            ?? throw new NotFoundException($"Child company not found: {request.CompanyId}");

        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");

        child.SetActive(request.IsActive, userId);
        await dbContext.SaveChangesAsync();
        return new SetChildCompanyStatusResult(true);
    }
}
