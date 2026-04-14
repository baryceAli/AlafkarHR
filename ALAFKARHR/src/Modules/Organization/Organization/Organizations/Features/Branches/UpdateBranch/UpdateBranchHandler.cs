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

        await dbContext.SaveChangesAsync();

        return new UpdateBranchResult(true);
    }
}
