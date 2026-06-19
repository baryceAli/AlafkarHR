namespace Organization.Organizations.Features.Branches.CreateBranch;


public record CreateBranchCommand(BranchDto Branch) : ICommand<CreateBranchResult>;
public record CreateBranchResult(BranchDto CreatedBranch);
public class CreateBranchHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ICompanyHierarchyContext companyHierarchyContext)
    : ICommandHandler<CreateBranchCommand, CreateBranchResult>
{
    public async Task<CreateBranchResult> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {

        var company = await dbContext.Companies.FindAsync([request.Branch.CompanyId]);
        if (company is null)
            throw new NotFoundException($"Company not found: {request.Branch.CompanyId}");

        await EnsureBranchLimitAsync(request.Branch.CompanyId, cancellationToken);

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ?? 
                        throw new UnauthorizedAccessException("User is not authenticated");

        if (request.Branch.IsMainBranch)
        {
            await ClearCurrentMainBranchAsync(request.Branch.CompanyId, userId, cancellationToken);
        }
        
        var branch = Branch.Create(
            Guid.NewGuid(),
            request.Branch.Name,
            request.Branch.NameEng,
            request.Branch.Location,
            request.Branch.Longitude,
            request.Branch.Latitude,
            request.Branch.Code,
            request.Branch.Phone,
            request.Branch.Email,
            request.Branch.IsMainBranch,
            request.Branch.CompanyId,
            userId
            );

        await dbContext.Branches.AddAsync(branch, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateBranchResult(branch.Adapt<BranchDto>());

    }

    private async Task ClearCurrentMainBranchAsync(Guid companyId, string userId, CancellationToken cancellationToken)
    {
        var mainBranches = await dbContext.Branches
            .Where(branch => branch.CompanyId == companyId && branch.IsMainBranch)
            .ToListAsync(cancellationToken);

        foreach (var branch in mainBranches)
        {
            branch.Update(
                branch.Name,
                branch.NameEng,
                branch.Location,
                branch.Longitude,
                branch.Latitude,
                branch.Code,
                branch.Phone,
                branch.Email,
                false,
                userId);
        }
    }

    private async Task EnsureBranchLimitAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var parentCompanyId = await companyHierarchyContext.GetParentCompanyIdForCompanyAsync(companyId, cancellationToken);
        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken);

        if (license is null)
            return;

        if (!license.AllowsAccess(DateTime.UtcNow))
            throw new UnauthorizedAccessException("Parent company license is not active");

        var hierarchyIds = await companyHierarchyContext.GetCompanyHierarchyIdsAsync(parentCompanyId, cancellationToken);
        var branchesCount = await dbContext.Branches
            .AsNoTracking()
            .CountAsync(x => hierarchyIds.Contains(x.CompanyId), cancellationToken);

        if (branchesCount >= license.MaxBranches)
            throw new InvalidOperationException("Parent company branch license limit has been reached");
    }
}
