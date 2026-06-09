using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeeAttendanceProfile;

public class GetEmployeeAttendanceProfileHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetEmployeeAttendanceProfileQuery, GetEmployeeAttendanceProfileResult>,
      IQueryHandler<GetEmployeeAttendanceProfileByCodeQuery, GetEmployeeAttendanceProfileResult>
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
                e.IsActive,
                e.Code,
                e.Email,
                e.FullName))
            .FirstOrDefaultAsync(cancellationToken);

        return employee ?? throw new NotFoundException(nameof(EmployeeModule.Employees.Models.Employee), request.EmployeeId);
    }

    public async Task<GetEmployeeAttendanceProfileResult> Handle(
        GetEmployeeAttendanceProfileByCodeQuery request,
        CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToLower();
        var employee = await dbContext.Employees
            .AsNoTracking()
            .Where(e => e.Code.ToLower() == normalizedCode && !e.IsDeleted)
            .Select(e => new GetEmployeeAttendanceProfileResult(
                e.Id,
                e.CompanyId,
                e.BranchId,
                e.AdministrationId,
                e.DepartmentId,
                e.AttendanceType,
                e.IsActive,
                e.Code,
                e.Email,
                e.FullName))
            .FirstOrDefaultAsync(cancellationToken);

        return employee ?? throw new NotFoundException(nameof(EmployeeModule.Employees.Models.Employee), request.Code);
    }
}
