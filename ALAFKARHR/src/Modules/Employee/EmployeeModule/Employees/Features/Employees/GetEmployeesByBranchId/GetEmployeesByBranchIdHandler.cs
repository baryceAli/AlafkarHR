using EmployeeModule.Employees.Features.Employees.GetEmployeesByAdministrationId;
using Shared.Contracts.CQRS;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeesByBranchId;


public record GetEmployeesByBranchIdQuery(Guid BranchId, PaginationRequest PaginationRequest) : IQuery<GetEmployeesByBranchIdResult>;
public record GetEmployeesByBranchIdResult(PaginatedResult<EmployeeDto> EmployeeList);
public class GetEmployeesByBranchIdHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetEmployeesByBranchIdQuery, GetEmployeesByBranchIdResult>
{
    public async Task<GetEmployeesByBranchIdResult> Handle(GetEmployeesByBranchIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Employees.AsNoTracking().AsQueryable();

        query = query.Where(e => e.BranchId == request.BranchId);
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

        return new GetEmployeesByBranchIdResult(
            new PaginatedResult<EmployeeDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                branches.Adapt<List<EmployeeDto>>()
            )
        );
    }
}
