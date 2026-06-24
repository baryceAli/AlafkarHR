using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceConfiguration : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public DayOfWeek FirstDayOfWeek { get; private set; }
    public string WeekendDays { get; private set; } = "Friday,Saturday";

    private AttendanceConfiguration() { }

    public static AttendanceConfiguration Create(Guid id, UpsertAttendanceCalendarSettingsDto dto)
    {
        var configuration = new AttendanceConfiguration
        {
            Id = id,
            CompanyId = dto.CompanyId,
            CreatedAt = DateTime.UtcNow
        };

        configuration.Apply(dto);
        return configuration;
    }

    public void Update(UpsertAttendanceCalendarSettingsDto dto, string? modifiedBy)
    {
        Apply(dto);
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public List<DayOfWeek> GetWeekendDays()
        => WeekendDays.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => Enum.TryParse<DayOfWeek>(x, out var day) ? (DayOfWeek?)day : null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

    public bool IsWeekend(DateTime date) => GetWeekendDays().Contains(date.DayOfWeek);

    public static AttendanceCalendarSettingsDto DefaultDto(Guid companyId)
        => new()
        {
            CompanyId = companyId,
            FirstDayOfWeek = DayOfWeek.Saturday,
            WeekendDays = [DayOfWeek.Friday, DayOfWeek.Saturday]
        };

    public AttendanceCalendarSettingsDto ToDto()
        => new()
        {
            Id = Id,
            CompanyId = CompanyId,
            FirstDayOfWeek = FirstDayOfWeek,
            WeekendDays = GetWeekendDays()
        };

    private void Apply(UpsertAttendanceCalendarSettingsDto dto)
    {
        Validate(dto);

        FirstDayOfWeek = dto.FirstDayOfWeek;
        WeekendDays = string.Join(",", dto.WeekendDays.Distinct().OrderBy(x => x));
    }

    private static void Validate(UpsertAttendanceCalendarSettingsDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required for attendance calendar settings.");
        }

        if (dto.WeekendDays.Count == 0)
        {
            throw new BadRequestException("At least one weekend day is required.");
        }
    }
}
