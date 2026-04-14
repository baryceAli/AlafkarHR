namespace Organization.Organizations.Features.Branches.GetBranches;


public record GetBranchesQuery(PaginationRequest PaginationRequest) : IQuery<GetBranchesRersult>;
public record GetBranchesRersult(PaginatedResult<BranchDto> BranchList);
public class GetBranchesHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetBranchesQuery, GetBranchesRersult>
{
    public async Task<GetBranchesRersult> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        long count = await dbContext.Branches.LongCountAsync();
        var branches = await dbContext.Branches
                        .AsNoTracking()
                        .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                        .Take(request.PaginationRequest.PageSize)
                        .ToListAsync();

        return new GetBranchesRersult(
            new PaginatedResult<BranchDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                branches.Adapt<List<BranchDto>>()));

    }
}
