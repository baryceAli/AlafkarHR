using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;
using Shared.Pagination;
using Shared.SaveImages;

namespace AttendanceDomain.Attendance.Features.Configuration;

public record GetAttendanceCalendarSettingsQuery(Guid CompanyId) : IQuery<GetAttendanceCalendarSettingsResult>;
public record GetAttendanceCalendarSettingsResult(AttendanceCalendarSettingsDto Settings);
public record UpsertAttendanceCalendarSettingsCommand(UpsertAttendanceCalendarSettingsDto Settings, string? ModifiedBy)
    : ICommand<UpsertAttendanceCalendarSettingsResult>;
public record UpsertAttendanceCalendarSettingsResult(AttendanceCalendarSettingsDto Settings);

public class UpsertAttendanceCalendarSettingsValidator : AbstractValidator<UpsertAttendanceCalendarSettingsCommand>
{
    public UpsertAttendanceCalendarSettingsValidator()
    {
        RuleFor(x => x.Settings.CompanyId).NotEmpty();
        RuleFor(x => x.Settings.WeekendDays).NotEmpty().WithMessage("At least one weekend day is required.");
    }
}

public class GetAttendanceCalendarSettingsHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetAttendanceCalendarSettingsQuery, GetAttendanceCalendarSettingsResult>
{
    public async Task<GetAttendanceCalendarSettingsResult> Handle(GetAttendanceCalendarSettingsQuery request, CancellationToken cancellationToken)
    {
        var configuration = await dbContext.AttendanceConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId, cancellationToken);

        return new GetAttendanceCalendarSettingsResult(configuration?.ToDto()
            ?? AttendanceConfiguration.DefaultDto(request.CompanyId));
    }
}

public class UpsertAttendanceCalendarSettingsHandler(AttendanceDbContext dbContext)
    : ICommandHandler<UpsertAttendanceCalendarSettingsCommand, UpsertAttendanceCalendarSettingsResult>
{
    public async Task<UpsertAttendanceCalendarSettingsResult> Handle(
        UpsertAttendanceCalendarSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var configuration = await dbContext.AttendanceConfigurations
            .FirstOrDefaultAsync(x => x.CompanyId == request.Settings.CompanyId, cancellationToken);

        if (configuration is null)
        {
            configuration = AttendanceConfiguration.Create(
                Guid.NewGuid(),
                request.Settings);
            await dbContext.AttendanceConfigurations.AddAsync(configuration, cancellationToken);
        }
        else
        {
            configuration.Update(request.Settings, request.ModifiedBy);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAttendanceCalendarSettingsResult(configuration.ToDto());
    }
}

