using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;
using Shared.Pagination;
using Shared.SaveImages;

namespace AttendanceDomain.Attendance.Features.Configuration;

public record GetAttendanceConfigurationQuery(Guid CompanyId) : IQuery<GetAttendanceConfigurationResult>;
public record GetAttendanceConfigurationResult(AttendanceConfigurationDto Configuration);
public record UpsertAttendanceConfigurationCommand(UpsertAttendanceConfigurationDto Configuration, string? ModifiedBy)
    : ICommand<UpsertAttendanceConfigurationResult>;
public record UpsertAttendanceConfigurationResult(AttendanceConfigurationDto Configuration);

public class UpsertAttendanceConfigurationValidator : AbstractValidator<UpsertAttendanceConfigurationCommand>
{
    public UpsertAttendanceConfigurationValidator()
    {
        RuleFor(x => x.Configuration.CompanyId).NotEmpty();
        RuleFor(x => x.Configuration.WeekendDays).NotEmpty().WithMessage("At least one weekend day is required.");
        RuleFor(x => x.Configuration)
            .Must(configuration => !configuration.DaySchedules
                .Any(day => day.IsWorkingDay && configuration.WeekendDays.Contains(day.DayOfWeek)))
            .WithMessage("A day cannot be both a working day and a weekend day.");
        RuleForEach(x => x.Configuration.DaySchedules).ChildRules(day =>
        {
            day.When(x => x.IsWorkingDay, () =>
            {
                day.RuleFor(x => x.StartTime).NotNull().WithMessage("Start time is required for working days.");
                day.RuleFor(x => x.EndTime).NotNull().WithMessage("End time is required for working days.");
                day.RuleFor(x => x.EndTime)
                    .GreaterThan(x => x.StartTime)
                    .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
                    .WithMessage("End time must be after start time.");
            });
        });
    }
}

public class GetAttendanceConfigurationHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetAttendanceConfigurationQuery, GetAttendanceConfigurationResult>
{
    public async Task<GetAttendanceConfigurationResult> Handle(GetAttendanceConfigurationQuery request, CancellationToken cancellationToken)
    {
        var configuration = await dbContext.AttendanceConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId, cancellationToken);

        return new GetAttendanceConfigurationResult(configuration?.ToDto()
            ?? AttendanceConfiguration.DefaultDto(request.CompanyId));
    }
}

public class UpsertAttendanceConfigurationHandler(AttendanceDbContext dbContext)
    : ICommandHandler<UpsertAttendanceConfigurationCommand, UpsertAttendanceConfigurationResult>
{
    public async Task<UpsertAttendanceConfigurationResult> Handle(
        UpsertAttendanceConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        var configuration = await dbContext.AttendanceConfigurations
            .FirstOrDefaultAsync(x => x.CompanyId == request.Configuration.CompanyId, cancellationToken);

        if (configuration is null)
        {
            configuration = AttendanceConfiguration.Create(
                Guid.NewGuid(),
                request.Configuration);
            await dbContext.AttendanceConfigurations.AddAsync(configuration, cancellationToken);
        }
        else
        {
            configuration.Update(request.Configuration, request.ModifiedBy);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAttendanceConfigurationResult(configuration.ToDto());
    }
}

