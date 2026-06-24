using EmployeeModule.Contracts.Employees.Features.GetCompanyEmployeeRosterProfiles;
using Shared.Contracts.CQRS;

namespace EmployeeModule.Employees.Features.Employees.GetCompanyEmployeeRosterProfiles;

public class GetCompanyEmployeeRosterProfilesHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetCompanyEmployeeRosterProfilesQuery, GetCompanyEmployeeRosterProfilesResult>
{
    public async Task<GetCompanyEmployeeRosterProfilesResult> Handle(
        GetCompanyEmployeeRosterProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await dbContext.Employees
            .AsNoTracking()
            .Include(x => x.Position)
            .Where(x => x.CompanyId == request.CompanyId && x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.FirstNameEng)
            .ThenBy(x => x.FirstName)
            .Select(x => new EmployeeRosterProfileDto(
                x.Id,
                x.CompanyId,
                x.BranchId,
                x.AdministrationId,
                x.DepartmentId,
                x.PositionId,
                x.AttendanceType,
                x.IsActive,
                x.EmployeeNo,
                x.Code,
                x.Email,
                x.FullName,
                x.FullNameEng,
                x.Position != null ? x.Position.Title : null,
                x.Position != null ? x.Position.TitleEng : null))
            .ToListAsync(cancellationToken);

        return new GetCompanyEmployeeRosterProfilesResult(employees);
    }
}
