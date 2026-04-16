using EmployeeModule.Employees.Features.Employees.GetEmployeesByBranchId;
using Shared.Contracts.CQRS;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeesByCompanyId;


public record GetEmployeesByCompanyIdQuery(Guid CompanyId, PaginationRequest PaginationRequest) : IQuery<GetEmployeesByCompanyIdResult>;
public record GetEmployeesByCompanyIdResult(PaginatedResult<EmployeeDto> EmployeeList);
public class GetEmployeesByCompanyIdHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetEmployeesByCompanyIdQuery, GetEmployeesByCompanyIdResult>
{
    public async Task<GetEmployeesByCompanyIdResult> Handle(GetEmployeesByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Employees.AsNoTracking().AsQueryable();

        query = query.Where(e => e.CompanyId == request.CompanyId);
        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();

            query = query.Where(b =>
                b.FirstName.ToLower().Contains(search) ||
                b.MiddleName.ToLower().Contains(search) ||
                b.LastName.ToLower().Contains(search) ||
                (b.Email != null && b.Email.ToLower().Contains(search)) ||
                (b.Phone != null && b.Phone.Contains(search))
            );
        }

        // 📊 Total count AFTER filtering
        long count = await query.LongCountAsync(cancellationToken);

        // 📄 Pagination
        var branches = await query
            .OrderBy(b => b.FullName) // default sorting (important!)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetEmployeesByCompanyIdResult(
            new PaginatedResult<EmployeeDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                branches.Adapt<List<EmployeeDto>>()
            )
        );
    }
}
