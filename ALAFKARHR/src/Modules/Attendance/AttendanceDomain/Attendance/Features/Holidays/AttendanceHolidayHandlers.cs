using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;
using Shared.Pagination;
using Shared.SaveImages;

namespace AttendanceDomain.Attendance.Features.Holidays;

public record GetAttendanceHolidaysQuery(Guid CompanyId, DateTime? FromDate, DateTime? ToDate)
    : IQuery<GetAttendanceHolidaysResult>;
public record GetAttendanceHolidaysResult(List<AttendanceHolidayDto> HolidayList);
public record UpsertAttendanceHolidayCommand(UpsertAttendanceHolidayDto Holiday, string? ModifiedBy)
    : ICommand<UpsertAttendanceHolidayResult>;
public record UpsertAttendanceHolidayResult(AttendanceHolidayDto Holiday);
public record DeleteAttendanceHolidayCommand(Guid HolidayId, string? DeletedBy) : ICommand<DeleteAttendanceHolidayResult>;
public record DeleteAttendanceHolidayResult(bool IsSuccess);

public class UpsertAttendanceHolidayValidator : AbstractValidator<UpsertAttendanceHolidayCommand>
{
    public UpsertAttendanceHolidayValidator()
    {
        RuleFor(x => x.Holiday.CompanyId).NotEmpty();
        RuleFor(x => x.Holiday.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Holiday.Description).MaximumLength(1000);
        RuleFor(x => x.Holiday.EndDate.Date)
            .GreaterThanOrEqualTo(x => x.Holiday.StartDate.Date)
            .WithMessage("Holiday end date must be on or after start date.");
    }
}

public class GetAttendanceHolidaysHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetAttendanceHolidaysQuery, GetAttendanceHolidaysResult>
{
    public async Task<GetAttendanceHolidaysResult> Handle(GetAttendanceHolidaysQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.AttendanceHolidays.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

        if (request.FromDate.HasValue)
        {
            var fromDate = UtcDateTime.Normalize(request.FromDate.Value).Date;
            query = query.Where(x => x.EndDate >= fromDate || x.IsRecurringYearly);
        }

        if (request.ToDate.HasValue)
        {
            var toDate = UtcDateTime.Normalize(request.ToDate.Value).Date;
            query = query.Where(x => x.StartDate <= toDate || x.IsRecurringYearly);
        }

        var holidays = await query
            .OrderByDescending(x => x.StartDate)
            .ProjectToType<AttendanceHolidayDto>()
            .ToListAsync(cancellationToken);

        return new GetAttendanceHolidaysResult(holidays);
    }
}

public class UpsertAttendanceHolidayHandler(AttendanceDbContext dbContext)
    : ICommandHandler<UpsertAttendanceHolidayCommand, UpsertAttendanceHolidayResult>
{
    public async Task<UpsertAttendanceHolidayResult> Handle(UpsertAttendanceHolidayCommand request, CancellationToken cancellationToken)
    {
        AttendanceHoliday holiday;
        if (request.Holiday.Id.HasValue && request.Holiday.Id.Value != Guid.Empty)
        {
            holiday = await dbContext.AttendanceHolidays
                .FirstOrDefaultAsync(x => x.Id == request.Holiday.Id.Value && !x.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("AttendanceHoliday", request.Holiday.Id.Value);

            holiday.Update(
                request.Holiday.HolidayType,
                request.Holiday.StartDate,
                request.Holiday.EndDate,
                request.Holiday.IsRecurringYearly,
                request.Holiday.IsActive,
                request.Holiday.Name,
                request.Holiday.Description,
                request.ModifiedBy);
        }
        else
        {
            holiday = AttendanceHoliday.Create(
                Guid.NewGuid(),
                request.Holiday.CompanyId,
                request.Holiday.HolidayType,
                request.Holiday.StartDate,
                request.Holiday.EndDate,
                request.Holiday.IsRecurringYearly,
                request.Holiday.IsActive,
                request.Holiday.Name,
                request.Holiday.Description);
            await dbContext.AttendanceHolidays.AddAsync(holiday, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAttendanceHolidayResult(holiday.Adapt<AttendanceHolidayDto>());
    }
}

public class DeleteAttendanceHolidayHandler(AttendanceDbContext dbContext)
    : ICommandHandler<DeleteAttendanceHolidayCommand, DeleteAttendanceHolidayResult>
{
    public async Task<DeleteAttendanceHolidayResult> Handle(DeleteAttendanceHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await dbContext.AttendanceHolidays
            .FirstOrDefaultAsync(x => x.Id == request.HolidayId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("AttendanceHoliday", request.HolidayId);

        holiday.Delete(request.DeletedBy);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteAttendanceHolidayResult(true);
    }
}

