namespace Organization.Organizations.Features.Branches.DeleteBranch;

public record DeleteBranchCommand(Guid Id) : ICommand<DeleteBranchResult>;
public record DeleteBranchResult(bool IsSuccess);
public class DeleteBranchHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteBranchCommand, DeleteBranchResult>
{
    public async Task<DeleteBranchResult> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (branch is null)
            throw new NotFoundException($"Branch not found: {request.Id}");


        var userId = httpContextAccessor.HttpContext?
                        .User
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        branch.Remove(userId);
        await dbContext.SaveChangesAsync();

        return new DeleteBranchResult(true);
    
    }
}
