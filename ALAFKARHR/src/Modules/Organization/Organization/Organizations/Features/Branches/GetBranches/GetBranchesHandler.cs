namespace Organization.Organizations.Features.Branches.GetBranches;


public record GetBranchesQuery(PaginationRequest PaginationRequest) : IQuery<GetBranchesRersult>;
public record GetBranchesRersult(PaginatedResult<BranchDto> BranchList);
public class GetBranchesHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetBranchesQuery, GetBranchesRersult>
{
    public async Task<GetBranchesRersult> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Branches.AsNoTracking().AsQueryable();

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();

            query = query.Where(b =>
                b.Name.ToLower().Contains(search) ||
                b.NameEng.ToLower().Contains(search) ||
                (b.Email != null && b.Email.ToLower().Contains(search)) ||
                (b.Phone != null && b.Phone.Contains(search))
            );
        }

        // 📊 Total count AFTER filtering
        long count = await query.LongCountAsync(cancellationToken);

        // 📄 Pagination
        var branches = await query
            .OrderBy(b => b.Name) // default sorting (important!)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetBranchesRersult(
            new PaginatedResult<BranchDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                branches.Adapt<List<BranchDto>>()
            )
        );
    }
}
