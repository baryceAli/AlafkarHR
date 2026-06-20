using AttendanceDomain.Attendance.Models;
using AttendanceDomain.Data;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;
using LeaveManagement.Data;
using LeaveManagement.Leave.Models;
using Shared.Pagination;
using Shared.SaveImages;

namespace LeaveManagement.Leave.Features.EmergencyLeaves;

public record CreateEmergencyLeaveRequestCommand(CreateEmergencyLeaveRequestDto Request)
    : ICommand<CreateEmergencyLeaveRequestResult>;
public record CreateEmergencyLeaveRequestResult(EmergencyLeaveRequestDto Request);
public record UploadEmergencyLeaveAttachmentCommand(IFormFile File, string UserId)
    : ICommand<UploadEmergencyLeaveAttachmentResult>;
public record UploadEmergencyLeaveAttachmentResult(string AttachmentPath);
public record ReviewEmergencyLeaveRequestCommand(ReviewEmergencyLeaveRequestDto Review, string ReviewedBy, Guid ReviewerEmployeeId)
    : ICommand<ReviewEmergencyLeaveRequestResult>;
public record ReviewEmergencyLeaveRequestResult(EmergencyLeaveRequestDto Request);
public record GetEmergencyLeaveRequestsQuery(
    Guid CompanyId,
    AttendanceExceptionStatus? Status,
    Guid? EmployeeId,
    Guid ReviewerEmployeeId,
    PaginationRequest PaginationRequest)
    : IQuery<GetEmergencyLeaveRequestsResult>;
public record GetEmergencyLeaveRequestsResult(PaginatedResult<EmergencyLeaveRequestDto> RequestList);

public class CreateEmergencyLeaveRequestValidator : AbstractValidator<CreateEmergencyLeaveRequestCommand>
{
    public CreateEmergencyLeaveRequestValidator()
    {
        RuleFor(x => x.Request.EmployeeId).NotEmpty();
        RuleFor(x => x.Request.CompanyId).NotEmpty();
        RuleFor(x => x.Request.Reason).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Request.EndDate.Date)
            .GreaterThanOrEqualTo(x => x.Request.StartDate.Date)
            .WithMessage("Emergency leave end date must be on or after start date.");
    }
}

public class CreateEmergencyLeaveRequestHandler(LeaveDbContext leaveDbContext, AttendanceDbContext attendanceDbContext)
    : ICommandHandler<CreateEmergencyLeaveRequestCommand, CreateEmergencyLeaveRequestResult>
{
    public async Task<CreateEmergencyLeaveRequestResult> Handle(CreateEmergencyLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        await EnsureLeaveContainsWorkingDayAsync(request.Request.CompanyId, request.Request.StartDate, request.Request.EndDate, cancellationToken);

        var leave = EmergencyLeaveRequest.Create(Guid.NewGuid(), request.Request);
        await leaveDbContext.EmergencyLeaveRequests.AddAsync(leave, cancellationToken);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new CreateEmergencyLeaveRequestResult(leave.Adapt<EmergencyLeaveRequestDto>());
    }

    private async Task EnsureLeaveContainsWorkingDayAsync(
        Guid companyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var fromDate = UtcDateTime.Normalize(startDate).Date;
        var toDate = UtcDateTime.Normalize(endDate).Date;
        var configuration = await attendanceDbContext.AttendanceConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        var configurationDto = configuration?.ToDto() ?? AttendanceConfiguration.DefaultDto(companyId);
        var holidays = await attendanceDbContext.AttendanceHolidays
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && x.IsActive
                && !x.IsDeleted
                && (x.EndDate >= fromDate || x.IsRecurringYearly)
                && (x.StartDate <= toDate || x.IsRecurringYearly))
            .ToListAsync(cancellationToken);

        for (var date = fromDate; date <= toDate; date = date.AddDays(1))
        {
            var schedule = configurationDto.DaySchedules.First(x => x.DayOfWeek == date.DayOfWeek);
            if (schedule.IsWorkingDay
                && !configurationDto.WeekendDays.Contains(date.DayOfWeek)
                && !holidays.Any(x => HolidayMatchesDate(x, date)))
            {
                return;
            }
        }

        throw new BadRequestException("Leave request must include at least one configured working day.");
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

    private static DateTime BuildRecurringDate(DateTime date, int year)
    {
        var day = Math.Min(date.Day, DateTime.DaysInMonth(year, date.Month));
        return new DateTime(year, date.Month, day);
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
}

public class UploadEmergencyLeaveAttachmentHandler(IWebHostEnvironment environment)
    : ICommandHandler<UploadEmergencyLeaveAttachmentCommand, UploadEmergencyLeaveAttachmentResult>
{
    private static readonly string[] AllowedExtensions =
    [
        ".pdf",
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp",
        ".bmp",
        ".tif",
        ".tiff"
    ];

    private static readonly string[] AllowedContentTypes =
    [
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/bmp",
        "image/tiff"
    ];

    public async Task<UploadEmergencyLeaveAttachmentResult> Handle(
        UploadEmergencyLeaveAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.File.Length == 0)
        {
            throw new BadRequestException("Attachment file is empty.");
        }

        var uploadRoot = Path.Combine(environment.WebRootPath ?? "wwwroot", "attachments", "leaves");
        var fileNameWithoutExtension = $"{request.UserId}-{GetNextSerial(uploadRoot, request.UserId)}";

        try
        {
            var savedUpload = await SaveImages.SaveFormFileAsync(
                request.File,
                fileNameWithoutExtension,
                [uploadRoot],
                "/attachments/leaves",
                AllowedExtensions,
                AllowedContentTypes,
                cancellationToken);

            return new UploadEmergencyLeaveAttachmentResult(savedUpload.PublicPath);
        }
        catch (ArgumentException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    private static int GetNextSerial(string uploadRoot, string userId)
    {
        if (!Directory.Exists(uploadRoot))
        {
            return 1;
        }

        return Directory
            .EnumerateFiles(uploadRoot, $"{userId}-*")
            .Count() + 1;
    }
}

public class ReviewEmergencyLeaveRequestHandler(LeaveDbContext leaveDbContext, AttendanceDbContext attendanceDbContext, ISender sender)
    : ICommandHandler<ReviewEmergencyLeaveRequestCommand, ReviewEmergencyLeaveRequestResult>
{
    public async Task<ReviewEmergencyLeaveRequestResult> Handle(ReviewEmergencyLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leave = await leaveDbContext.EmergencyLeaveRequests
            .FirstOrDefaultAsync(x => x.Id == request.Review.RequestId, cancellationToken)
            ?? throw new NotFoundException("EmergencyLeaveRequest", request.Review.RequestId);

        await EnsureReviewerCanReviewAsync(request.ReviewerEmployeeId, leave.EmployeeId, cancellationToken);

        if (request.Review.IsApproved)
        {
            for (var year = leave.StartDate.Year; year <= leave.EndDate.Year; year++)
            {
                var yearStart = new DateTime(year, 1, 1);
                var yearEnd = new DateTime(year, 12, 31);
                var start = leave.StartDate > yearStart ? leave.StartDate : yearStart;
                var end = leave.EndDate < yearEnd ? leave.EndDate : yearEnd;
                var leaveDays = await CountWorkingLeaveDaysAsync(leave.CompanyId, start, end, cancellationToken);

                if (leaveDays <= 0)
                {
                    continue;
                }

                var balance = await leaveDbContext.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(x => x.CompanyId == leave.CompanyId
                        && x.EmployeeId == leave.EmployeeId
                        && x.Year == year, cancellationToken)
                    ?? throw new BadRequestException($"Employee leave balance must be configured for {year} before approving leave.");

                balance.AddTakenDays(leaveDays, request.ReviewedBy);
            }

            leave.Approve(request.ReviewedBy, request.Review.ApproverComment);
        }
        else
        {
            leave.Reject(request.ReviewedBy, request.Review.ApproverComment);
        }

        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new ReviewEmergencyLeaveRequestResult(leave.Adapt<EmergencyLeaveRequestDto>());
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

    private async Task EnsureReviewerCanReviewAsync(Guid reviewerEmployeeId, Guid employeeId, CancellationToken cancellationToken)
    {
        if (reviewerEmployeeId == employeeId)
        {
            throw new UnauthorizedAccessException("Employees cannot review their own emergency leave requests.");
        }

        var reviewer = await sender.Send(new GetEmployeeAttendanceProfileQuery(reviewerEmployeeId), cancellationToken);
        var employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(employeeId), cancellationToken);

        if (!CanReviewEmergencyLeave(reviewer, employee))
        {
            throw new UnauthorizedAccessException("The signed-in employee cannot review this emergency leave request.");
        }
    }

    private static bool CanReviewEmergencyLeave(
        GetEmployeeAttendanceProfileResult reviewer,
        GetEmployeeAttendanceProfileResult employee)
    {
        if (reviewer.CompanyId != employee.CompanyId)
        {
            return false;
        }

        if (employee.DepartmentId.HasValue && reviewer.DepartmentId == employee.DepartmentId)
        {
            return true;
        }

        return reviewer.AdministrationId == employee.AdministrationId;
    }
}

public class GetEmergencyLeaveRequestsHandler(LeaveDbContext leaveDbContext, ISender sender)
    : IQueryHandler<GetEmergencyLeaveRequestsQuery, GetEmergencyLeaveRequestsResult>
{
    public async Task<GetEmergencyLeaveRequestsResult> Handle(GetEmergencyLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = leaveDbContext.EmergencyLeaveRequests.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        var reviewer = await sender.Send(new GetEmployeeAttendanceProfileQuery(request.ReviewerEmployeeId), cancellationToken);
        var scopedRows = new List<EmergencyLeaveRequestDto>();
        var employeeProfiles = new Dictionary<Guid, GetEmployeeAttendanceProfileResult>();
        var candidateRows = await query
            .OrderByDescending(x => x.StartDate)
            .ProjectToType<EmergencyLeaveRequestDto>()
            .ToListAsync(cancellationToken);

        foreach (var row in candidateRows)
        {
            if (row.EmployeeId == request.ReviewerEmployeeId)
            {
                continue;
            }

            if (!employeeProfiles.TryGetValue(row.EmployeeId, out var employee))
            {
                employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(row.EmployeeId), cancellationToken);
                employeeProfiles[row.EmployeeId] = employee;
            }

            if (CanReviewEmergencyLeave(reviewer, employee))
            {
                scopedRows.Add(row);
            }
        }

        var total = scopedRows.Count;
        var rows = scopedRows
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToList();

        return new GetEmergencyLeaveRequestsResult(new PaginatedResult<EmergencyLeaveRequestDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            total,
            rows));
    }

    private static bool CanReviewEmergencyLeave(
        GetEmployeeAttendanceProfileResult reviewer,
        GetEmployeeAttendanceProfileResult employee)
    {
        if (reviewer.CompanyId != employee.CompanyId)
        {
            return false;
        }

        if (employee.DepartmentId.HasValue && reviewer.DepartmentId == employee.DepartmentId)
        {
            return true;
        }

        return reviewer.AdministrationId == employee.AdministrationId;
    }
}
