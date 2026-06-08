namespace Organization.Organizations.Features.Branches.UpdateBranch;


public record UpdateBranchCommand(BranchDto Branch) : ICommand<UpdateBranchResult>;
public record UpdateBranchResult(bool IsSuccess);
public class UpdateBranchHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateBranchCommand, UpdateBranchResult>
{
    public async Task<UpdateBranchResult> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(x => x.Id == request.Branch.Id, cancellationToken);

        if (branch is null)
            throw new NotFoundException($"Branch not found: {request.Branch.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        if (request.Branch.IsMainBranch)
        {
            await ClearOtherMainBranchesAsync(branch.CompanyId, branch.Id, userId, cancellationToken);
        }

        branch.Update(
            request.Branch.Name,
            request.Branch.NameEng,
            request.Branch.Location,
            request.Branch.Longitude,
            request.Branch.Latitude,
            request.Branch.Code,
            request.Branch.Phone,
            request.Branch.Email,
            request.Branch.IsMainBranch,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateBranchResult(true);
    }

    private async Task ClearOtherMainBranchesAsync(Guid companyId, Guid currentBranchId, string userId, CancellationToken cancellationToken)
    {
        var mainBranches = await dbContext.Branches
            .Where(branch =>
                branch.CompanyId == companyId &&
                branch.Id != currentBranchId &&
                branch.IsMainBranch)
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
}
