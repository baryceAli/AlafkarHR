using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using Organization.Organizations.Features.Departments.GetDepartments;

namespace Organization.Organizations.Features.Departments.GetDepartmentsByAdministrationId;


public record GetDepartmentsByAdministrationIdQuery(Guid AdministrationId,PaginationRequest PaginationRequest) : IQuery<GetDepartmentsByAdministrationIdResult>;
public record GetDepartmentsByAdministrationIdResult(PaginatedResult<DepartmentDto> DepartmentList);
public class GetDepartmentsByAdministrationIdHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetDepartmentsByAdministrationIdQuery, GetDepartmentsByAdministrationIdResult>
{
    public async Task<GetDepartmentsByAdministrationIdResult> Handle(GetDepartmentsByAdministrationIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Departments.AsNoTracking().AsQueryable();
        query = query.Where(ad => ad.AdministrationId == request.AdministrationId);
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
        var departments = await query
            .OrderBy(b => b.Name) // default sorting (important!)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetDepartmentsByAdministrationIdResult(
            new PaginatedResult<DepartmentDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                departments.Adapt<List<DepartmentDto>>()
            )
        );
    }
}
