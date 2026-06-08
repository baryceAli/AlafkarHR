using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeeAttendanceProfile;

public class GetEmployeeAttendanceProfileHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetEmployeeAttendanceProfileQuery, GetEmployeeAttendanceProfileResult>
{
    public async Task<GetEmployeeAttendanceProfileResult> Handle(
        GetEmployeeAttendanceProfileQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .Where(e => e.Id == request.EmployeeId && !e.IsDeleted)
            .Select(e => new GetEmployeeAttendanceProfileResult(
                e.Id,
                e.CompanyId,
                e.BranchId,
                e.AdministrationId,
                e.DepartmentId,
                e.AttendanceType,
                e.IsActive))
            .FirstOrDefaultAsync(cancellationToken);

        return employee ?? throw new NotFoundException(nameof(EmployeeModule.Employees.Models.Employee), request.EmployeeId);
    }
}
