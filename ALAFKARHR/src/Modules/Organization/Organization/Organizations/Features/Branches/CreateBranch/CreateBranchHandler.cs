namespace Organization.Organizations.Features.Branches.CreateBranch;


public record CreateBranchCommand(BranchDto Branch) : ICommand<CreateBranchResult>;
public record CreateBranchResult(BranchDto CreatedBranch);
public class CreateBranchHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ICompanyHierarchyContext companyHierarchyContext, ISender sender)
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
            false,
            request.Branch.Specialization,
            request.Branch.CompanyId,
            userId
            );

        await dbContext.Branches.AddAsync(branch, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await sender.Send(new EnsureBranchAccountingCommand(
            branch.CompanyId,
            branch.Id,
            branch.Code,
            branch.Name,
            branch.NameEng), cancellationToken);
        return new CreateBranchResult(branch.Adapt<BranchDto>());

    }

    private async Task EnsureBranchLimitAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var parentCompanyId = await companyHierarchyContext.GetParentCompanyIdForCompanyAsync(companyId, cancellationToken);
        var license = await dbContext.CompanyLicenses
            .AsNoTracking()
            .Include(x => x.LicenseCategory)
            .FirstOrDefaultAsync(x => x.CompanyId == parentCompanyId, cancellationToken);

        if (license is null)
            return;

        if (!license.AllowsAccess(DateTime.UtcNow))
            throw new UnauthorizedAccessException("Parent company license is not active");

        var hierarchyIds = await companyHierarchyContext.GetCompanyHierarchyIdsAsync(parentCompanyId, cancellationToken);
        var branchesCount = await dbContext.Branches
            .AsNoTracking()
            .CountAsync(x => hierarchyIds.Contains(x.CompanyId), cancellationToken);

        if (branchesCount >= license.EffectiveMaxBranches)
            throw new InvalidOperationException("Parent company branch license limit has been reached");
    }
}
