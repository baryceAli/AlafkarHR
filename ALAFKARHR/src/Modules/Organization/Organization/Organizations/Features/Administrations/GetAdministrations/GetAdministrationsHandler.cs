using Auth.Contracts.Features.GetUserById;
using Organization.Organizations.Features.Branches.GetBranches;

namespace Organization.Organizations.Features.Administrations.GetAdministrations;


public record GetAdministrationsQuery(PaginationRequest PaginationRequest):IQuery<GetAdministrationsResult>;
public record GetAdministrationsResult(PaginatedResult<AdministrationDto> AdministrationList);
public class GetAdministrationsHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : IQueryHandler<GetAdministrationsQuery, GetAdministrationsResult>
{
    public async Task<GetAdministrationsResult> Handle(GetAdministrationsQuery request, CancellationToken cancellationToken)
    {

        var query = dbContext.Administrations.AsNoTracking().AsQueryable();

        var companyIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        if (Guid.TryParse(companyIdClaim, out var companyId))
        {
            query = query.Where(x => x.CompanyId == companyId);
        }

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

        var administrationDtos = administrations.Adapt<List<AdministrationDto>>();
        await FillParentNamesAsync(administrationDtos, cancellationToken);

        return new GetAdministrationsResult(
            new PaginatedResult<AdministrationDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                administrationDtos
            )
        );


    }

    private async Task FillParentNamesAsync(List<AdministrationDto> administrations, CancellationToken cancellationToken)
    {
        var parentIds = administrations
            .Where(x => x.ParentAdministrationId.HasValue)
            .Select(x => x.ParentAdministrationId!.Value)
            .Distinct()
            .ToList();

        if (!parentIds.Any())
        {
            return;
        }

        var parents = await dbContext.Administrations
            .AsNoTracking()
            .Where(x => parentIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Name, x.NameEng })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var administration in administrations)
        {
            if (administration.ParentAdministrationId.HasValue
                && parents.TryGetValue(administration.ParentAdministrationId.Value, out var parent))
            {
                administration.ParentAdministrationName = parent.Name;
                administration.ParentAdministrationNameEng = parent.NameEng;
            }
        }
    }
}
