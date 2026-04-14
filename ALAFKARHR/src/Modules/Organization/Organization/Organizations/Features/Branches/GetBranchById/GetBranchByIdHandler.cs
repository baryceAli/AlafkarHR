namespace Organization.Organizations.Features.Branches.GetBranchById;


public record GetBranchByIdQuery(Guid Id) : IQuery<GetBranchByIdResult>;
public record GetBranchByIdResult(BranchDto Branch);
public class GetBranchByIdHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetBranchByIdQuery, GetBranchByIdResult>
{
    public async Task<GetBranchByIdResult> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (branch is null)
            throw new NotFoundException($"Branch not found: {request.Id}");

        return new GetBranchByIdResult(branch.Adapt<BranchDto>());
    }
}
