using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;
using Shared.Pagination;
using Shared.SaveImages;

namespace AttendanceDomain.Attendance.Features.Reports;

public record GetAttendanceReportQuery(AttendanceReportFilterDto Filter) : IQuery<GetAttendanceReportResult>;
public record GetAttendanceReportResult(AttendanceReportDto Report);

public class GetAttendanceReportHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetAttendanceReportQuery, GetAttendanceReportResult>
{
    public async Task<GetAttendanceReportResult> Handle(GetAttendanceReportQuery request, CancellationToken cancellationToken)
    {
        var fromDate = UtcDateTime.Normalize(request.Filter.FromDate).Date;
        var toDate = UtcDateTime.Normalize(request.Filter.ToDate).Date;
        if (toDate < fromDate)
        {
            throw new BadRequestException("Report end date must be on or after start date.");
        }

        var configuration = await dbContext.AttendanceConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.Filter.CompanyId, cancellationToken);
        var configurationDto = configuration?.ToDto() ?? AttendanceConfiguration.DefaultDto(request.Filter.CompanyId);

        var rows = new List<AttendanceReportRowDto>();

        if (MatchesCategory(request.Filter.Category, "Attendance")
            || MatchesCategory(request.Filter.Category, "AttendanceSummary")
            || MatchesCategory(request.Filter.Category, "LateArrival")
            || MatchesCategory(request.Filter.Category, "EarlyLeave")
            || MatchesCategory(request.Filter.Category, "Break")
            || MatchesCategory(request.Filter.Category, "Absence"))
        {
            var sessions = dbContext.AttendanceSessions.AsNoTracking()
                .Where(x => x.CompanyId == request.Filter.CompanyId
                    && x.ShiftStart.Date >= fromDate
                    && x.ShiftStart.Date <= toDate);

            if (request.Filter.EmployeeId.HasValue)
            {
                sessions = sessions.Where(x => x.EmployeeId == request.Filter.EmployeeId.Value);
            }

            var sessionRows = await sessions.Select(x => new AttendanceReportRowDto
            {
                Date = x.ShiftStart.Date,
                EmployeeId = x.EmployeeId,
                Category = "Attendance",
                Status = null,
                SessionStatus = x.Status,
                NormalizationStatus = x.NormalizationStatus,
                ShiftStartUtc = x.ShiftStart,
                ShiftEndUtc = x.ShiftEnd,
                CheckInUtc = x.ActualStartTime,
                CheckOutUtc = x.ActualEndTime,
                TotalWorkingHours = x.TotalHours,
                NetWorkingHours = x.TotalHours,
                NormalizationNote = x.NormalizationNote
            }).ToListAsync(cancellationToken);

            if (MatchesCategory(request.Filter.Category, "LateArrival"))
            {
                rows.AddRange(sessionRows
                    .Where(x => x.CheckInUtc.HasValue && x.ShiftStartUtc.HasValue && x.CheckInUtc.Value > x.ShiftStartUtc.Value)
                    .Select(x => { x.Category = "LateArrival"; return x; }));
            }
            else if (MatchesCategory(request.Filter.Category, "EarlyLeave"))
            {
                rows.AddRange(sessionRows
                    .Where(x => x.CheckOutUtc.HasValue && x.ShiftEndUtc.HasValue && x.CheckOutUtc.Value < x.ShiftEndUtc.Value)
                    .Select(x => { x.Category = "EarlyLeave"; return x; }));
            }
            else if (MatchesCategory(request.Filter.Category, "Break"))
            {
                rows.AddRange(sessionRows
                    .Where(x => x.SessionStatus == AttendanceSessionStatus.OnBreak)
                    .Select(x => { x.Category = "Break"; return x; }));
            }
            else if (MatchesCategory(request.Filter.Category, "Absence") && request.Filter.EmployeeId.HasValue)
            {
                var presentDates = sessionRows
                    .Where(x => x.NormalizationStatus != AttendanceNormalizationStatus.MarkedAbsent)
                    .Select(x => x.Date.Date)
                    .ToHashSet();
                var holidayDates = await GetCompanyHolidayDatesAsync(
                    request.Filter.CompanyId,
                    fromDate,
                    toDate,
                    cancellationToken);

                for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                {
                    var schedule = configurationDto.DaySchedules.First(x => x.DayOfWeek == date.DayOfWeek);
                    if (schedule.IsWorkingDay
                        && !configurationDto.WeekendDays.Contains(date.DayOfWeek)
                        && !holidayDates.Contains(date)
                        && !presentDates.Contains(date))
                    {
                        rows.Add(new AttendanceReportRowDto
                        {
                            Date = date,
                            EmployeeId = request.Filter.EmployeeId,
                            Category = "Absence",
                            Reason = "No attendance session found for configured working day."
                        });
                    }
                }
            }
            else
            {
                rows.AddRange(sessionRows);
            }
        }

        if (MatchesCategory(request.Filter.Category, "Holiday") || MatchesCategory(request.Filter.Category, "HolidayWeekend"))
        {
            var holidays = dbContext.AttendanceHolidays.AsNoTracking()
                .Where(x => x.CompanyId == request.Filter.CompanyId
                    && x.IsActive
                    && !x.IsDeleted
                    && (x.EndDate >= fromDate || x.IsRecurringYearly)
                    && (x.StartDate <= toDate || x.IsRecurringYearly));

            foreach (var holiday in await holidays.ToListAsync(cancellationToken))
            {
                rows.AddRange(BuildHolidayRows(holiday, fromDate, toDate, request.Filter.EmployeeId));
            }
        }

        if (MatchesCategory(request.Filter.Category, "Weekend") || MatchesCategory(request.Filter.Category, "HolidayWeekend"))
        {
            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                if (configurationDto.WeekendDays.Contains(date.DayOfWeek))
                {
                    rows.Add(new AttendanceReportRowDto
                    {
                        Date = date,
                        EmployeeId = request.Filter.EmployeeId,
                        Category = "Weekend",
                        Reason = date.DayOfWeek.ToString()
                    });
                }
            }
        }

        if (MatchesCategory(request.Filter.Category, "MidDayPermission"))
        {
            var permissions = dbContext.MidDayPermissionRequests.AsNoTracking()
                .Where(x => x.CompanyId == request.Filter.CompanyId
                    && x.Date >= fromDate
                    && x.Date <= toDate);

            if (request.Filter.EmployeeId.HasValue)
            {
                permissions = permissions.Where(x => x.EmployeeId == request.Filter.EmployeeId.Value);
            }

            if (request.Filter.Status.HasValue)
            {
                permissions = permissions.Where(x => x.Status == request.Filter.Status.Value);
            }

            rows.AddRange(await permissions.Select(x => new AttendanceReportRowDto
            {
                Date = x.Date,
                EmployeeId = x.EmployeeId,
                Category = "MidDayPermission",
                Status = x.Status,
                RequestedStartUtc = x.RequestedStartUtc,
                RequestedEndUtc = x.RequestedEndUtc,
                ApprovedStartUtc = x.ApprovedStartUtc,
                ApprovedEndUtc = x.ApprovedEndUtc,
                Reason = x.Reason,
                ApproverComment = x.ApproverComment
            }).ToListAsync(cancellationToken));
        }

        var orderedRows = rows
            .OrderBy(x => x.Date)
            .ThenBy(x => x.EmployeeId)
            .ThenBy(x => x.Category)
            .ToList();

        return new GetAttendanceReportResult(new AttendanceReportDto
        {
            FirstDayOfWeek = configurationDto.FirstDayOfWeek,
            WeekendDays = configurationDto.WeekendDays,
            Rows = orderedRows
        });
    }

    private static bool MatchesCategory(string? filter, string category)
        => string.IsNullOrWhiteSpace(filter) || string.Equals(filter, category, StringComparison.OrdinalIgnoreCase);

    private async Task<HashSet<DateTime>> GetCompanyHolidayDatesAsync(
        Guid companyId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken)
    {
        var holidays = await dbContext.AttendanceHolidays.AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && x.IsActive
                && !x.IsDeleted
                && (x.EndDate >= fromDate || x.IsRecurringYearly)
                && (x.StartDate <= toDate || x.IsRecurringYearly))
            .ToListAsync(cancellationToken);

        return holidays
            .SelectMany(x => BuildHolidayDates(x, fromDate, toDate))
            .ToHashSet();
    }

    private static IEnumerable<AttendanceReportRowDto> BuildHolidayRows(
        AttendanceHoliday holiday,
        DateTime fromDate,
        DateTime toDate,
        Guid? employeeId)
        => BuildHolidayDates(holiday, fromDate, toDate).Select(date => new AttendanceReportRowDto
        {
            Date = date,
            EmployeeId = employeeId,
            Category = "Holiday",
            Reason = holiday.Name ?? holiday.Description
        });

    private static IEnumerable<DateTime> BuildHolidayDates(AttendanceHoliday holiday, DateTime fromDate, DateTime toDate)
    {
        if (!holiday.IsRecurringYearly)
        {
            for (var date = holiday.StartDate.Date > fromDate ? holiday.StartDate.Date : fromDate;
                 date <= (holiday.EndDate.Date < toDate ? holiday.EndDate.Date : toDate);
                 date = date.AddDays(1))
            {
                yield return date;
            }

            yield break;
        }

        for (var year = fromDate.Year - 1; year <= toDate.Year; year++)
        {
            var start = BuildRecurringDate(holiday.StartDate.Date, year);
            var end = BuildRecurringDate(holiday.EndDate.Date, year);
            if (end < start)
            {
                end = end.AddYears(1);
            }

            for (var date = start > fromDate ? start : fromDate;
                 date <= (end < toDate ? end : toDate);
                 date = date.AddDays(1))
            {
                yield return date;
            }
        }
    }

    private static bool HolidayMatchesDate(AttendanceHoliday holiday, DateTime date)
        => BuildHolidayDates(holiday, date.Date, date.Date).Any();

    private static DateTime BuildRecurringDate(DateTime date, int year)
    {
        var day = Math.Min(date.Day, DateTime.DaysInMonth(year, date.Month));
        return new DateTime(year, date.Month, day);
    }
}
