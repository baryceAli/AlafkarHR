using AttendanceDomain.Attendance.Models;
using AttendanceDomain.Data;
using FluentValidation;
using LeaveManagement.Data;
using LeaveManagement.Leave.Models;

namespace LeaveManagement.Leave.Features.LeaveBalances;

public record GetEmployeeLeaveBalancesQuery(Guid CompanyId, int Year, Guid? EmployeeId)
    : IQuery<GetEmployeeLeaveBalancesResult>;
public record GetEmployeeLeaveBalancesResult(List<EmployeeLeaveBalanceDto> BalanceList);
public record UpsertEmployeeLeaveBalanceCommand(UpsertEmployeeLeaveBalanceDto Balance, string? ModifiedBy)
    : ICommand<UpsertEmployeeLeaveBalanceResult>;
public record UpsertEmployeeLeaveBalanceResult(EmployeeLeaveBalanceDto Balance);
public record GetLeaveReportQuery(LeaveReportFilterDto Filter) : IQuery<GetLeaveReportResult>;
public record GetLeaveReportResult(LeaveReportDto Report);

public class UpsertEmployeeLeaveBalanceValidator : AbstractValidator<UpsertEmployeeLeaveBalanceCommand>
{
    public UpsertEmployeeLeaveBalanceValidator()
    {
        RuleFor(x => x.Balance.EmployeeId).NotEmpty();
        RuleFor(x => x.Balance.CompanyId).NotEmpty();
        RuleFor(x => x.Balance.Year).InclusiveBetween(2000, 2100);
        RuleFor(x => x.Balance.AnnualLeaveDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Balance.MaxCarryForwardDays).GreaterThanOrEqualTo(0);
    }
}

public class GetEmployeeLeaveBalancesHandler(LeaveDbContext leaveDbContext)
    : IQueryHandler<GetEmployeeLeaveBalancesQuery, GetEmployeeLeaveBalancesResult>
{
    public async Task<GetEmployeeLeaveBalancesResult> Handle(GetEmployeeLeaveBalancesQuery request, CancellationToken cancellationToken)
    {
        var query = leaveDbContext.EmployeeLeaveBalances.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.Year == request.Year);

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        var balanceEntities = await query
            .OrderBy(x => x.EmployeeId)
            .ToListAsync(cancellationToken);

        return new GetEmployeeLeaveBalancesResult(balanceEntities.Select(ToDto).ToList());
    }

    private static EmployeeLeaveBalanceDto ToDto(EmployeeLeaveBalance balance)
        => new()
        {
            Id = balance.Id,
            EmployeeId = balance.EmployeeId,
            CompanyId = balance.CompanyId,
            Year = balance.Year,
            AnnualLeaveDays = balance.AnnualLeaveDays,
            AllowCarryForward = balance.AllowCarryForward,
            MaxCarryForwardDays = balance.MaxCarryForwardDays,
            CarriedForwardDays = balance.CarriedForwardDays,
            TakenDays = balance.TakenDays,
            AvailableDays = balance.AvailableDays,
            RemainingDays = balance.RemainingDays
        };
}

public class UpsertEmployeeLeaveBalanceHandler(LeaveDbContext leaveDbContext, AttendanceDbContext attendanceDbContext)
    : ICommandHandler<UpsertEmployeeLeaveBalanceCommand, UpsertEmployeeLeaveBalanceResult>
{
    public async Task<UpsertEmployeeLeaveBalanceResult> Handle(UpsertEmployeeLeaveBalanceCommand request, CancellationToken cancellationToken)
    {
        var carriedForwardDays = await CalculateCarryForwardDaysAsync(request.Balance, cancellationToken);
        var takenDays = await CalculateApprovedLeaveDaysAsync(
            request.Balance.CompanyId,
            request.Balance.EmployeeId,
            request.Balance.Year,
            cancellationToken);

        var balance = await leaveDbContext.EmployeeLeaveBalances
            .FirstOrDefaultAsync(x => x.CompanyId == request.Balance.CompanyId
                && x.EmployeeId == request.Balance.EmployeeId
                && x.Year == request.Balance.Year, cancellationToken);

        if (balance is null)
        {
            balance = EmployeeLeaveBalance.Create(Guid.NewGuid(), request.Balance, carriedForwardDays, request.ModifiedBy);
            await leaveDbContext.EmployeeLeaveBalances.AddAsync(balance, cancellationToken);
        }
        else
        {
            balance.Update(request.Balance, carriedForwardDays, request.ModifiedBy);
        }

        balance.RecalculateTakenDays(takenDays, request.ModifiedBy);
        await leaveDbContext.SaveChangesAsync(cancellationToken);

        return new UpsertEmployeeLeaveBalanceResult(ToDto(balance));
    }

    private async Task<decimal> CalculateCarryForwardDaysAsync(UpsertEmployeeLeaveBalanceDto dto, CancellationToken cancellationToken)
    {
        if (!dto.AllowCarryForward)
        {
            return 0;
        }

        var previous = await leaveDbContext.EmployeeLeaveBalances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == dto.CompanyId
                && x.EmployeeId == dto.EmployeeId
                && x.Year == dto.Year - 1, cancellationToken);

        return previous is null
            ? 0
            : Math.Min(previous.RemainingDays, dto.MaxCarryForwardDays);
    }

    private async Task<decimal> CalculateApprovedLeaveDaysAsync(Guid companyId, Guid employeeId, int year, CancellationToken cancellationToken)
    {
        var fromDate = new DateTime(year, 1, 1);
        var toDate = new DateTime(year, 12, 31);
        var leaves = await leaveDbContext.EmergencyLeaveRequests.AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && x.EmployeeId == employeeId
                && x.Status == AttendanceExceptionStatus.Approved
                && x.StartDate <= toDate
                && x.EndDate >= fromDate)
            .ToListAsync(cancellationToken);

        decimal days = 0;
        foreach (var leave in leaves)
        {
            var start = leave.StartDate < fromDate ? fromDate : leave.StartDate;
            var end = leave.EndDate > toDate ? toDate : leave.EndDate;
            days += await CountWorkingLeaveDaysAsync(companyId, start, end, cancellationToken);
        }

        return days;
    }

    private async Task<decimal> CountWorkingLeaveDaysAsync(Guid companyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var configuration = await attendanceDbContext.AttendanceConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);

        var weekendDays = configuration?.ToDto().WeekendDays ?? [DayOfWeek.Friday, DayOfWeek.Saturday];
        var holidays = await attendanceDbContext.AttendanceHolidays
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && x.IsActive
                && !x.IsDeleted
                && (x.EndDate >= startDate.Date || x.IsRecurringYearly)
                && (x.StartDate <= endDate.Date || x.IsRecurringYearly))
            .ToListAsync(cancellationToken);

        decimal days = 0;
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            if (!weekendDays.Contains(date.DayOfWeek)
                && !holidays.Any(x => HolidayMatchesDate(x, date)))
            {
                days++;
            }
        }

        return days;
    }

    private static bool HolidayMatchesDate(AttendanceHoliday holiday, DateTime date)
    {
        if (!holiday.IsRecurringYearly)
        {
            return holiday.StartDate.Date <= date.Date && holiday.EndDate.Date >= date.Date;
        }

        return RecurringHolidayMatchesYear(holiday, date.Date, date.Year)
            || RecurringHolidayMatchesYear(holiday, date.Date, date.Year - 1);
    }

    private static bool RecurringHolidayMatchesYear(AttendanceHoliday holiday, DateTime date, int year)
    {
        var start = BuildRecurringDate(holiday.StartDate.Date, year);
        var end = BuildRecurringDate(holiday.EndDate.Date, year);
        if (end < start)
        {
            end = end.AddYears(1);
        }

        return start <= date && end >= date;
    }

    private static DateTime BuildRecurringDate(DateTime date, int year)
    {
        var day = Math.Min(date.Day, DateTime.DaysInMonth(year, date.Month));
        return new DateTime(year, date.Month, day);
    }

    private static EmployeeLeaveBalanceDto ToDto(EmployeeLeaveBalance balance)
        => new()
        {
            Id = balance.Id,
            EmployeeId = balance.EmployeeId,
            CompanyId = balance.CompanyId,
            Year = balance.Year,
            AnnualLeaveDays = balance.AnnualLeaveDays,
            AllowCarryForward = balance.AllowCarryForward,
            MaxCarryForwardDays = balance.MaxCarryForwardDays,
            CarriedForwardDays = balance.CarriedForwardDays,
            TakenDays = balance.TakenDays,
            AvailableDays = balance.AvailableDays,
            RemainingDays = balance.RemainingDays
        };
}

public class GetLeaveReportHandler(LeaveDbContext leaveDbContext)
    : IQueryHandler<GetLeaveReportQuery, GetLeaveReportResult>
{
    public async Task<GetLeaveReportResult> Handle(GetLeaveReportQuery request, CancellationToken cancellationToken)
    {
        var balances = await leaveDbContext.EmployeeLeaveBalances.AsNoTracking()
            .Where(x => x.CompanyId == request.Filter.CompanyId && x.Year == request.Filter.Year)
            .ToListAsync(cancellationToken);

        if (request.Filter.EmployeeId.HasValue)
        {
            balances = balances.Where(x => x.EmployeeId == request.Filter.EmployeeId.Value).ToList();
        }

        var fromDate = new DateTime(request.Filter.Year, 1, 1);
        var toDate = new DateTime(request.Filter.Year, 12, 31);
        var requests = await leaveDbContext.EmergencyLeaveRequests.AsNoTracking()
            .Where(x => x.CompanyId == request.Filter.CompanyId
                && x.StartDate <= toDate
                && x.EndDate >= fromDate)
            .ToListAsync(cancellationToken);

        if (request.Filter.EmployeeId.HasValue)
        {
            requests = requests.Where(x => x.EmployeeId == request.Filter.EmployeeId.Value).ToList();
        }

        if (request.Filter.Status.HasValue)
        {
            requests = requests.Where(x => x.Status == request.Filter.Status.Value).ToList();
        }

        var rows = balances.Select(balance =>
        {
            var employeeRequests = requests.Where(x => x.EmployeeId == balance.EmployeeId).ToList();
            return new LeaveReportRowDto
            {
                EmployeeId = balance.EmployeeId,
                Year = balance.Year,
                AnnualLeaveDays = balance.AnnualLeaveDays,
                CarriedForwardDays = balance.CarriedForwardDays,
                AvailableDays = balance.AvailableDays,
                TakenDays = balance.TakenDays,
                RemainingDays = balance.RemainingDays,
                PendingRequests = employeeRequests.Count(x => x.Status == AttendanceExceptionStatus.Pending),
                ApprovedRequests = employeeRequests.Count(x => x.Status == AttendanceExceptionStatus.Approved),
                RejectedRequests = employeeRequests.Count(x => x.Status == AttendanceExceptionStatus.Rejected)
            };
        }).ToList();

        return new GetLeaveReportResult(new LeaveReportDto
        {
            Year = request.Filter.Year,
            Rows = rows
        });
    }
}
