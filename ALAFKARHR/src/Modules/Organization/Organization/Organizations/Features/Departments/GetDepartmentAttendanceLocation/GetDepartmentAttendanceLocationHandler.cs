using Organization.Contracts.Departments.Features.GetDepartmentAttendanceLocation;

namespace Organization.Organizations.Features.Departments.GetDepartmentAttendanceLocation;

public class GetDepartmentAttendanceLocationHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetDepartmentAttendanceLocationQuery, GetDepartmentAttendanceLocationResult>
{
    public async Task<GetDepartmentAttendanceLocationResult> Handle(
        GetDepartmentAttendanceLocationQuery request,
        CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments
            .AsNoTracking()
            .Where(d => d.Id == request.DepartmentId && !d.IsDeleted)
            .Select(d => new GetDepartmentAttendanceLocationResult(
                d.Id,
                d.CompanyId,
                d.Name,
                d.Latitude,
                d.Longitude,
                d.AllowedRadiusMeters,
                d.IsActive))
            .FirstOrDefaultAsync(cancellationToken);

        return department ?? throw new NotFoundException(nameof(Department), request.DepartmentId);
    }
}
