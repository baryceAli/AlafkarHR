namespace Organization.Organizations.Features.Departments.GetDepartments;


public record GetDepartmentsQuery(PaginationRequest PaginationRequest) : IQuery<GetDepartmentsResult>;
public record GetDepartmentsResult(PaginatedResult<DepartmentDto> DepartmentList);
public class GetDepartmentsHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetDepartmentsQuery, GetDepartmentsResult>
{
    public async Task<GetDepartmentsResult> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var count = await dbContext.Departments.LongCountAsync();
        var departments = await dbContext.Departments
                            .AsNoTracking()
                            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
                            .Take(request.PaginationRequest.PageSize)
                            .ToListAsync(cancellationToken);

        return new GetDepartmentsResult(
            new PaginatedResult<DepartmentDto>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                count,
                departments.Adapt<List<DepartmentDto>>()));
    }
}
