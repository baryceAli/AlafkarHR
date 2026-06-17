using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;
using Shared.Pagination;
using Shared.SaveImages;

namespace AttendanceDomain.Attendance.Features.MidDayPermissions;

public record CreateMidDayPermissionRequestCommand(CreateMidDayPermissionRequestDto Request)
    : ICommand<CreateMidDayPermissionRequestResult>;
public record CreateMidDayPermissionRequestResult(MidDayPermissionRequestDto Request);
public record ReviewMidDayPermissionRequestCommand(ReviewMidDayPermissionRequestDto Review, string ReviewedBy, Guid ReviewerEmployeeId)
    : ICommand<ReviewMidDayPermissionRequestResult>;
public record ReviewMidDayPermissionRequestResult(MidDayPermissionRequestDto Request);
public record GetMidDayPermissionRequestsQuery(
    Guid CompanyId,
    AttendanceExceptionStatus? Status,
    Guid? EmployeeId,
    Guid ReviewerEmployeeId,
    PaginationRequest PaginationRequest)
    : IQuery<GetMidDayPermissionRequestsResult>;
public record GetMidDayPermissionRequestsResult(PaginatedResult<MidDayPermissionRequestDto> RequestList);

public class CreateMidDayPermissionRequestValidator : AbstractValidator<CreateMidDayPermissionRequestCommand>
{
    public CreateMidDayPermissionRequestValidator()
    {
        RuleFor(x => x.Request.EmployeeId).NotEmpty();
        RuleFor(x => x.Request.CompanyId).NotEmpty();
        RuleFor(x => x.Request.Reason).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Request.RequestedEndUtc)
            .GreaterThan(x => x.Request.RequestedStartUtc)
            .WithMessage("Permission end time must be after start time.");
    }
}

public class CreateMidDayPermissionRequestHandler(AttendanceDbContext dbContext)
    : ICommandHandler<CreateMidDayPermissionRequestCommand, CreateMidDayPermissionRequestResult>
{
    public async Task<CreateMidDayPermissionRequestResult> Handle(CreateMidDayPermissionRequestCommand request, CancellationToken cancellationToken)
    {
        var configuration = await LoadConfigurationAsync(request.Request.CompanyId, cancellationToken);
        ValidatePermissionWithinWorkingDay(request.Request.Date, request.Request.RequestedStartUtc, request.Request.RequestedEndUtc, configuration);
        await EnsureNotCompanyHolidayAsync(request.Request.CompanyId, request.Request.Date, cancellationToken);

        var permission = MidDayPermissionRequest.Create(Guid.NewGuid(), request.Request);
        await dbContext.MidDayPermissionRequests.AddAsync(permission, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateMidDayPermissionRequestResult(permission.Adapt<MidDayPermissionRequestDto>());
    }

    private async Task EnsureNotCompanyHolidayAsync(Guid companyId, DateTime requestDate, CancellationToken cancellationToken)
    {
        var date = UtcDateTime.Normalize(requestDate).Date;
        var holidays = await dbContext.AttendanceHolidays
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        if (holidays.Any(x => HolidayMatchesDate(x, date)))
        {
            throw new BadRequestException("Permission request date must not be a configured company holiday.");
        }
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

    private async Task<AttendanceConfiguration> LoadConfigurationAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var configuration = await dbContext.AttendanceConfigurations
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);

        if (configuration is not null)
        {
            return configuration;
        }

        return AttendanceConfiguration.Create(Guid.NewGuid(), new UpsertAttendanceConfigurationDto { CompanyId = companyId });
    }

    private static void ValidatePermissionWithinWorkingDay(
        DateTime requestDate,
        DateTime requestedStartUtc,
        DateTime requestedEndUtc,
        AttendanceConfiguration configuration)
    {
        var date = UtcDateTime.Normalize(requestDate).Date;
        var start = UtcDateTime.Normalize(requestedStartUtc);
        var end = UtcDateTime.Normalize(requestedEndUtc);

        if (start.Date != date || end.Date != date)
        {
            throw new BadRequestException("Permission request must start and end on the same working day.");
        }

        var schedule = configuration.GetSchedule(date);
        if (!schedule.IsWorkingDay || configuration.IsWeekend(date))
        {
            throw new BadRequestException("Permission request date must be a configured working day.");
        }

        var startsAfterWorkStart = start.TimeOfDay >= schedule.StartTime;
        var endsBeforeWorkEnd = end.TimeOfDay <= schedule.EndTime;
        if (!startsAfterWorkStart || !endsBeforeWorkEnd)
        {
            throw new BadRequestException("Permission request duration must be inside the configured working day.");
        }
    }
}

public class ReviewMidDayPermissionRequestHandler(AttendanceDbContext dbContext, ISender sender)
    : ICommandHandler<ReviewMidDayPermissionRequestCommand, ReviewMidDayPermissionRequestResult>
{
    public async Task<ReviewMidDayPermissionRequestResult> Handle(ReviewMidDayPermissionRequestCommand request, CancellationToken cancellationToken)
    {
        var permission = await dbContext.MidDayPermissionRequests
            .FirstOrDefaultAsync(x => x.Id == request.Review.RequestId, cancellationToken)
            ?? throw new NotFoundException("MidDayPermissionRequest", request.Review.RequestId);

        await EnsureReviewerCanReviewAsync(request.ReviewerEmployeeId, permission.EmployeeId, cancellationToken);

        if (request.Review.IsApproved)
        {
            permission.Approve(
                request.Review.ApprovedStartUtc ?? permission.RequestedStartUtc,
                request.Review.ApprovedEndUtc ?? permission.RequestedEndUtc,
                request.ReviewedBy,
                request.Review.ApproverComment);
        }
        else
        {
            permission.Reject(request.ReviewedBy, request.Review.ApproverComment);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReviewMidDayPermissionRequestResult(permission.Adapt<MidDayPermissionRequestDto>());
    }

    private async Task EnsureReviewerCanReviewAsync(Guid reviewerEmployeeId, Guid employeeId, CancellationToken cancellationToken)
    {
        if (reviewerEmployeeId == employeeId)
        {
            throw new UnauthorizedAccessException("Employees cannot review their own permission requests.");
        }

        var reviewer = await sender.Send(new GetEmployeeAttendanceProfileQuery(reviewerEmployeeId), cancellationToken);
        var employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(employeeId), cancellationToken);

        if (!CanReviewAttendanceRequest(reviewer, employee))
        {
            throw new UnauthorizedAccessException("The signed-in employee cannot review this permission request.");
        }
    }

    private static bool CanReviewAttendanceRequest(
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

public class GetMidDayPermissionRequestsHandler(AttendanceDbContext dbContext, ISender sender)
    : IQueryHandler<GetMidDayPermissionRequestsQuery, GetMidDayPermissionRequestsResult>
{
    public async Task<GetMidDayPermissionRequestsResult> Handle(GetMidDayPermissionRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.MidDayPermissionRequests.AsNoTracking()
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
        var scopedRows = new List<MidDayPermissionRequestDto>();
        var employeeProfiles = new Dictionary<Guid, GetEmployeeAttendanceProfileResult>();
        var candidateRows = await query
            .OrderByDescending(x => x.Date)
            .ProjectToType<MidDayPermissionRequestDto>()
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

            if (CanReviewAttendanceRequest(reviewer, employee))
            {
                scopedRows.Add(row);
            }
        }

        var total = scopedRows.Count;
        var rows = scopedRows
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToList();

        return new GetMidDayPermissionRequestsResult(new PaginatedResult<MidDayPermissionRequestDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            total,
            rows));
    }

    private static bool CanReviewAttendanceRequest(
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

