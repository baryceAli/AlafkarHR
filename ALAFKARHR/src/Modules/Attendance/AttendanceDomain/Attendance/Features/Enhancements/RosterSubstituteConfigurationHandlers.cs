using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetCompanyEmployeeRosterProfiles;

namespace AttendanceDomain.Attendance.Features.Enhancements;

public record GetRosterSubstituteConfigurationsQuery(Guid CompanyId)
    : IQuery<GetRosterSubstituteConfigurationsResult>;

public record GetRosterSubstituteConfigurationsResult(List<AttendanceRosterSubstituteConfigurationDto> ConfigurationList);

public record UpsertRosterSubstituteConfigurationCommand(
    UpsertAttendanceRosterSubstituteConfigurationDto Configuration,
    string? UserId)
    : ICommand<UpsertRosterSubstituteConfigurationResult>;

public record UpsertRosterSubstituteConfigurationResult(AttendanceRosterSubstituteConfigurationDto Configuration);

public class GetRosterSubstituteConfigurationsHandler(
    AttendanceDbContext dbContext,
    ISender sender)
    : IQueryHandler<GetRosterSubstituteConfigurationsQuery, GetRosterSubstituteConfigurationsResult>
{
    public async Task<GetRosterSubstituteConfigurationsResult> Handle(
        GetRosterSubstituteConfigurationsQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await GetEligibleEmployeesAsync(sender, request.CompanyId, cancellationToken);
        var configs = await dbContext.AttendanceRosterSubstituteConfigurations.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .ToDictionaryAsync(x => x.EmployeeId, cancellationToken);

        var rows = employees
            .Select(employee =>
            {
                configs.TryGetValue(employee.EmployeeId, out var config);
                return ToDto(employee, config);
            })
            .OrderBy(x => x.EmployeeNameEng ?? x.EmployeeName)
            .ToList();

        return new GetRosterSubstituteConfigurationsResult(rows);
    }

    internal static async Task<List<EmployeeRosterProfileDto>> GetEligibleEmployeesAsync(
        ISender sender,
        Guid companyId,
        CancellationToken cancellationToken)
    {
        if (companyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required for roster substitute configuration.");
        }

        var employeeResult = await sender.Send(new GetCompanyEmployeeRosterProfilesQuery(companyId), cancellationToken);
        return employeeResult.Employees
            .Where(x => x.IsActive && x.AdministrationId.HasValue && x.AdministrationId.Value != Guid.Empty)
            .ToList();
    }

    internal static AttendanceRosterSubstituteConfigurationDto ToDto(
        EmployeeRosterProfileDto employee,
        AttendanceRosterSubstituteConfiguration? config)
        => new()
        {
            Id = config?.Id,
            CompanyId = employee.CompanyId,
            EmployeeId = employee.EmployeeId,
            EmployeeNo = employee.EmployeeNo,
            EmployeeCode = employee.Code,
            EmployeeName = employee.FullName,
            EmployeeNameEng = employee.FullNameEng,
            AdministrationId = employee.AdministrationId!.Value,
            DepartmentId = employee.DepartmentId,
            PositionId = employee.PositionId,
            PositionName = employee.PositionName,
            PositionNameEng = employee.PositionNameEng,
            IsRosterVisible = config?.IsRosterVisible ?? true,
            IsSubstituteEligible = config?.IsSubstituteEligible ?? true,
            Notes = config?.Notes
        };
}

public class UpsertRosterSubstituteConfigurationHandler(
    AttendanceDbContext dbContext,
    ISender sender)
    : ICommandHandler<UpsertRosterSubstituteConfigurationCommand, UpsertRosterSubstituteConfigurationResult>
{
    public async Task<UpsertRosterSubstituteConfigurationResult> Handle(
        UpsertRosterSubstituteConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Configuration;
        var employees = await GetRosterSubstituteConfigurationsHandler.GetEligibleEmployeesAsync(
            sender,
            dto.CompanyId,
            cancellationToken);
        var employee = employees.FirstOrDefault(x => x.EmployeeId == dto.EmployeeId)
            ?? throw new BadRequestException("Only active employees assigned to an administration can be configured for attendance roster substitution.");

        var config = await dbContext.AttendanceRosterSubstituteConfigurations
            .FirstOrDefaultAsync(x => x.CompanyId == dto.CompanyId
                && x.EmployeeId == dto.EmployeeId
                && !x.IsDeleted,
                cancellationToken);

        if (config is null)
        {
            config = AttendanceRosterSubstituteConfiguration.Create(
                Guid.NewGuid(),
                dto.CompanyId,
                dto.EmployeeId,
                dto.IsRosterVisible,
                dto.IsSubstituteEligible,
                dto.Notes,
                request.UserId);
            dbContext.AttendanceRosterSubstituteConfigurations.Add(config);
        }
        else
        {
            config.Update(
                dto.IsRosterVisible,
                dto.IsSubstituteEligible,
                dto.Notes,
                request.UserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpsertRosterSubstituteConfigurationResult(
            GetRosterSubstituteConfigurationsHandler.ToDto(employee, config));
    }
}
