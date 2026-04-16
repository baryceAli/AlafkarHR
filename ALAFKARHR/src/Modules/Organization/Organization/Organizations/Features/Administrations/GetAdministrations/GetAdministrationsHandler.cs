using Auth.Contracts.Features.GetUserById;
using Organization.Organizations.Features.Branches.GetBranches;

namespace Organization.Organizations.Features.Administrations.GetAdministrations;


public record GetAdministrationsQuery(PaginationRequest PaginationRequest):IQuery<GetAdministrationsResult>;
public record GetAdministrationsResult(PaginatedResult<AdministrationDto> AdministrationList);
public class GetAdministrationsHandler(OrganizationDbContext dbContext, ISender sender)
    : IQueryHandler<GetAdministrationsQuery, GetAdministrationsResult>
{
    public async Task<GetAdministrationsResult> Handle(GetAdministrationsQuery request, CancellationToken cancellationToken)
    {

        var query = dbContext.Administrations.AsNoTracking().AsQueryable();

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

        return new GetAdministrationsResult(
            new PaginatedResult<AdministrationDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                administrations.Adapt<List<AdministrationDto>>()
            )
        );


    }
}
