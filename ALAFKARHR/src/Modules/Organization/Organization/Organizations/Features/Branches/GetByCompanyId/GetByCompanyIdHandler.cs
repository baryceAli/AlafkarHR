using Organization.Organizations.Features.Branches.GetBranches;

namespace Organization.Organizations.Features.Branches.GetByCompanyId;

//public record GetByCompanyIdQuery(Guid companyId) : IQuery<GetByCompanyIdResult>;
public record GetByCompanyIdQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetByCompanyIdResult>;

public record GetByCompanyIdResult(PaginatedResult<BranchDto> BranchList);
public class GetByCompanyIdHandler(OrganizationDbContext dbContext, ISender sender)
    : IQueryHandler<GetByCompanyIdQuery, GetByCompanyIdResult>
{
    public async Task<GetByCompanyIdResult> Handle(GetByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Branches.AsNoTracking().AsQueryable();
        query=query.Where(b=> b.CompanyId==request.CompanyId);

        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (!branchAccess.CanViewAllBranches)
            query = query.Where(b => branchAccess.BranchIds.Contains(b.Id));

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.Trim();

            query = query.Where(b =>
                b.Name.Contains(search) ||
                b.NameEng.Contains(search) ||
                b.Code.Contains(search) ||
                (b.Email != null && b.Email.Contains(search)) ||
                (b.Phone != null && b.Phone.Contains(search)) ||
                (b.Location != null && b.Location.Contains(search))
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

        return new GetByCompanyIdResult(
            new PaginatedResult<BranchDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                branches.Adapt<List<BranchDto>>()
            )
        );
        //return new GetByCompanyIdResult(companies.Adapt<List<BranchDto>>());

    }
}
