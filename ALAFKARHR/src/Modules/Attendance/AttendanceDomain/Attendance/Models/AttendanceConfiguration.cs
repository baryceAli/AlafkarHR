using Shared.DDD;

namespace AttendanceDomain.Attendance.Models;

public class AttendanceConfiguration : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public DayOfWeek FirstDayOfWeek { get; private set; }
    public bool SundayIsWorkingDay { get; private set; }
    public TimeSpan? SundayStartTime { get; private set; }
    public TimeSpan? SundayEndTime { get; private set; }
    public bool MondayIsWorkingDay { get; private set; }
    public TimeSpan? MondayStartTime { get; private set; }
    public TimeSpan? MondayEndTime { get; private set; }
    public bool TuesdayIsWorkingDay { get; private set; }
    public TimeSpan? TuesdayStartTime { get; private set; }
    public TimeSpan? TuesdayEndTime { get; private set; }
    public bool WednesdayIsWorkingDay { get; private set; }
    public TimeSpan? WednesdayStartTime { get; private set; }
    public TimeSpan? WednesdayEndTime { get; private set; }
    public bool ThursdayIsWorkingDay { get; private set; }
    public TimeSpan? ThursdayStartTime { get; private set; }
    public TimeSpan? ThursdayEndTime { get; private set; }
    public bool FridayIsWorkingDay { get; private set; }
    public TimeSpan? FridayStartTime { get; private set; }
    public TimeSpan? FridayEndTime { get; private set; }
    public bool SaturdayIsWorkingDay { get; private set; }
    public TimeSpan? SaturdayStartTime { get; private set; }
    public TimeSpan? SaturdayEndTime { get; private set; }
    public string WeekendDays { get; private set; } = "Friday,Saturday";

    private AttendanceConfiguration() { }

    public static AttendanceConfiguration Create(Guid id, UpsertAttendanceConfigurationDto dto)
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

    public void Update(UpsertAttendanceConfigurationDto dto, string? modifiedBy)
    {
        Apply(dto);
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public List<AttendanceDayScheduleDto> GetDaySchedules()
        => new()
        {
            Build(DayOfWeek.Sunday, SundayIsWorkingDay, SundayStartTime, SundayEndTime),
            Build(DayOfWeek.Monday, MondayIsWorkingDay, MondayStartTime, MondayEndTime),
            Build(DayOfWeek.Tuesday, TuesdayIsWorkingDay, TuesdayStartTime, TuesdayEndTime),
            Build(DayOfWeek.Wednesday, WednesdayIsWorkingDay, WednesdayStartTime, WednesdayEndTime),
            Build(DayOfWeek.Thursday, ThursdayIsWorkingDay, ThursdayStartTime, ThursdayEndTime),
            Build(DayOfWeek.Friday, FridayIsWorkingDay, FridayStartTime, FridayEndTime),
            Build(DayOfWeek.Saturday, SaturdayIsWorkingDay, SaturdayStartTime, SaturdayEndTime)
        };

    public List<DayOfWeek> GetWeekendDays()
        => WeekendDays.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => Enum.TryParse<DayOfWeek>(x, out var day) ? (DayOfWeek?)day : null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

    public AttendanceDayScheduleDto GetSchedule(DateTime date)
        => GetDaySchedules().First(x => x.DayOfWeek == date.DayOfWeek);

    public bool IsWeekend(DateTime date) => GetWeekendDays().Contains(date.DayOfWeek);

    public static AttendanceConfigurationDto DefaultDto(Guid companyId)
        => new()
        {
            CompanyId = companyId,
            FirstDayOfWeek = DayOfWeek.Saturday,
            DaySchedules = AttendanceDayScheduleDto.DefaultWeek(),
            WeekendDays = [DayOfWeek.Friday, DayOfWeek.Saturday]
        };

    public AttendanceConfigurationDto ToDto()
        => new()
        {
            Id = Id,
            CompanyId = CompanyId,
            FirstDayOfWeek = FirstDayOfWeek,
            DaySchedules = GetDaySchedules(),
            WeekendDays = GetWeekendDays()
        };

    private void Apply(UpsertAttendanceConfigurationDto dto)
    {
        Validate(dto);

        FirstDayOfWeek = dto.FirstDayOfWeek;
        WeekendDays = string.Join(",", dto.WeekendDays.Distinct().OrderBy(x => x));

        var schedules = AttendanceDayScheduleDto.DefaultWeek()
            .ToDictionary(x => x.DayOfWeek);

        foreach (var schedule in dto.DaySchedules)
        {
            schedules[schedule.DayOfWeek] = schedule;
        }

        ApplyDay(schedules[DayOfWeek.Sunday], out var sundayWorking, out var sundayStart, out var sundayEnd);
        SundayIsWorkingDay = sundayWorking;
        SundayStartTime = sundayStart;
        SundayEndTime = sundayEnd;

        ApplyDay(schedules[DayOfWeek.Monday], out var mondayWorking, out var mondayStart, out var mondayEnd);
        MondayIsWorkingDay = mondayWorking;
        MondayStartTime = mondayStart;
        MondayEndTime = mondayEnd;

        ApplyDay(schedules[DayOfWeek.Tuesday], out var tuesdayWorking, out var tuesdayStart, out var tuesdayEnd);
        TuesdayIsWorkingDay = tuesdayWorking;
        TuesdayStartTime = tuesdayStart;
        TuesdayEndTime = tuesdayEnd;

        ApplyDay(schedules[DayOfWeek.Wednesday], out var wednesdayWorking, out var wednesdayStart, out var wednesdayEnd);
        WednesdayIsWorkingDay = wednesdayWorking;
        WednesdayStartTime = wednesdayStart;
        WednesdayEndTime = wednesdayEnd;

        ApplyDay(schedules[DayOfWeek.Thursday], out var thursdayWorking, out var thursdayStart, out var thursdayEnd);
        ThursdayIsWorkingDay = thursdayWorking;
        ThursdayStartTime = thursdayStart;
        ThursdayEndTime = thursdayEnd;

        ApplyDay(schedules[DayOfWeek.Friday], out var fridayWorking, out var fridayStart, out var fridayEnd);
        FridayIsWorkingDay = fridayWorking;
        FridayStartTime = fridayStart;
        FridayEndTime = fridayEnd;

        ApplyDay(schedules[DayOfWeek.Saturday], out var saturdayWorking, out var saturdayStart, out var saturdayEnd);
        SaturdayIsWorkingDay = saturdayWorking;
        SaturdayStartTime = saturdayStart;
        SaturdayEndTime = saturdayEnd;
    }

    private static void Validate(UpsertAttendanceConfigurationDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required for attendance configuration.");
        }

        if (dto.WeekendDays.Count == 0)
        {
            throw new BadRequestException("At least one weekend day is required.");
        }

        var weekendDays = dto.WeekendDays.ToHashSet();
        foreach (var schedule in dto.DaySchedules)
        {
            if (!schedule.IsWorkingDay)
            {
                continue;
            }

            if (weekendDays.Contains(schedule.DayOfWeek))
            {
                throw new BadRequestException($"{schedule.DayOfWeek} cannot be both a working day and a weekend day.");
            }

            if (!schedule.StartTime.HasValue || !schedule.EndTime.HasValue)
            {
                throw new BadRequestException($"{schedule.DayOfWeek} start and end times are required for working days.");
            }

            if (schedule.EndTime <= schedule.StartTime)
            {
                throw new BadRequestException($"{schedule.DayOfWeek} end time must be after start time.");
            }
        }
    }

    private static AttendanceDayScheduleDto Build(DayOfWeek day, bool isWorkingDay, TimeSpan? start, TimeSpan? end)
        => new()
        {
            DayOfWeek = day,
            IsWorkingDay = isWorkingDay,
            StartTime = start,
            EndTime = end
        };

    private static void ApplyDay(AttendanceDayScheduleDto schedule, out bool isWorkingDay, out TimeSpan? start, out TimeSpan? end)
    {
        isWorkingDay = schedule.IsWorkingDay;
        start = schedule.IsWorkingDay ? schedule.StartTime : null;
        end = schedule.IsWorkingDay ? schedule.EndTime : null;
    }
}
