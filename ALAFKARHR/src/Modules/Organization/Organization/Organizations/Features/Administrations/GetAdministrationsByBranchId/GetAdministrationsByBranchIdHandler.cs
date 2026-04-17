namespace Organization.Organizations.Features.Administrations.GetAdministrationsByBranchId;

public record GetAdministrationsByBranchIdQuery(Guid BranchId, PaginationRequest PaginationRequest) : IQuery<GetAdministrationsByBranchIdResult>;
public record GetAdministrationsByBranchIdResult(PaginatedResult<AdministrationDto> AdministrationList);
public class GetAdministrationsByBranchIdHandler(OrganizationDbContext dbContext) 
    : IQueryHandler<GetAdministrationsByBranchIdQuery, GetAdministrationsByBranchIdResult>
{
    public async Task<GetAdministrationsByBranchIdResult> Handle(GetAdministrationsByBranchIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Administrations.AsNoTracking().AsQueryable();

        query=query.Where(adm=> adm.BranchId==request.BranchId);
        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();

            query = query.Where(b =>
                b.Name.ToLower().Contains(search)
                || b.NameEng.ToLower().Contains(search)
            //|| (b.Email != null && b.Email.ToLower().Contains(search)) 
            //|| (b.Phone != null && b.Phone.Contains(search))
            );
        }

        // 📊 Total count AFTER filtering
        long count = await query.LongCountAsync(cancellationToken);

        // 📄 Pagination
        var administrations = await query
            .OrderBy(b => b.Name) // default sorting (important!)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetAdministrationsByBranchIdResult(
            new PaginatedResult<AdministrationDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                administrations.Adapt<List<AdministrationDto>>()
            )
        );



    }
}
