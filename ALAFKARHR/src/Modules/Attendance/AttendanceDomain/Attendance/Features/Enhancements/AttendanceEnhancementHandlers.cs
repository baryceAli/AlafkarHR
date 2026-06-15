using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using FluentValidation;
using Shared.Pagination;
using Shared.SaveImages;

namespace AttendanceDomain.Attendance.Features.Enhancements;

public record GetAttendanceConfigurationQuery(Guid CompanyId) : IQuery<GetAttendanceConfigurationResult>;
public record GetAttendanceConfigurationResult(AttendanceConfigurationDto Configuration);
public record UpsertAttendanceConfigurationCommand(UpsertAttendanceConfigurationDto Configuration, string? ModifiedBy)
    : ICommand<UpsertAttendanceConfigurationResult>;
public record UpsertAttendanceConfigurationResult(AttendanceConfigurationDto Configuration);

public record GetAttendanceHolidaysQuery(Guid CompanyId, DateTime? FromDate, DateTime? ToDate)
    : IQuery<GetAttendanceHolidaysResult>;
public record GetAttendanceHolidaysResult(List<AttendanceHolidayDto> HolidayList);
public record UpsertAttendanceHolidayCommand(UpsertAttendanceHolidayDto Holiday, string? ModifiedBy)
    : ICommand<UpsertAttendanceHolidayResult>;
public record UpsertAttendanceHolidayResult(AttendanceHolidayDto Holiday);
public record DeleteAttendanceHolidayCommand(Guid HolidayId, string? DeletedBy) : ICommand<DeleteAttendanceHolidayResult>;
public record DeleteAttendanceHolidayResult(bool IsSuccess);

public record GetAttendanceBreakPoliciesQuery(Guid CompanyId) : IQuery<GetAttendanceBreakPoliciesResult>;
public record GetAttendanceBreakPoliciesResult(List<AttendanceBreakPolicyDto> PolicyList);
public record UpsertAttendanceBreakPolicyCommand(UpsertAttendanceBreakPolicyDto Policy, string? ModifiedBy)
    : ICommand<UpsertAttendanceBreakPolicyResult>;
public record UpsertAttendanceBreakPolicyResult(AttendanceBreakPolicyDto Policy);

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

public record CreateMidDayPermissionRequestCommand(CreateMidDayPermissionRequestDto Request)
    : ICommand<CreateMidDayPermissionRequestResult>;
public record CreateMidDayPermissionRequestResult(MidDayPermissionRequestDto Request);
public record ReviewMidDayPermissionRequestCommand(ReviewMidDayPermissionRequestDto Review, string ReviewedBy)
    : ICommand<ReviewMidDayPermissionRequestResult>;
public record ReviewMidDayPermissionRequestResult(MidDayPermissionRequestDto Request);
public record GetMidDayPermissionRequestsQuery(Guid CompanyId, AttendanceExceptionStatus? Status, Guid? EmployeeId, PaginationRequest PaginationRequest)
    : IQuery<GetMidDayPermissionRequestsResult>;
public record GetMidDayPermissionRequestsResult(PaginatedResult<MidDayPermissionRequestDto> RequestList);

public record GetAttendanceReportQuery(AttendanceReportFilterDto Filter) : IQuery<GetAttendanceReportResult>;
public record GetAttendanceReportResult(AttendanceReportDto Report);

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

public class GetAttendanceBreakPoliciesHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetAttendanceBreakPoliciesQuery, GetAttendanceBreakPoliciesResult>
{
    public async Task<GetAttendanceBreakPoliciesResult> Handle(GetAttendanceBreakPoliciesQuery request, CancellationToken cancellationToken)
    {
        var policies = await dbContext.AttendanceBreakPolicies
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderByDescending(x =>
                x.Scope == ShiftAssignmentScope.Employee ? 4 :
                x.Scope == ShiftAssignmentScope.Department ? 3 :
                x.Scope == ShiftAssignmentScope.Administration ? 2 :
                x.Scope == ShiftAssignmentScope.Company ? 1 : 0)
            .ProjectToType<AttendanceBreakPolicyDto>()
            .ToListAsync(cancellationToken);

        return new GetAttendanceBreakPoliciesResult(policies);
    }
}

public class UpsertAttendanceBreakPolicyHandler(AttendanceDbContext dbContext)
    : ICommandHandler<UpsertAttendanceBreakPolicyCommand, UpsertAttendanceBreakPolicyResult>
{
    public async Task<UpsertAttendanceBreakPolicyResult> Handle(UpsertAttendanceBreakPolicyCommand request, CancellationToken cancellationToken)
    {
        AttendanceBreakPolicy policy;
        if (request.Policy.Id.HasValue && request.Policy.Id.Value != Guid.Empty)
        {
            policy = await dbContext.AttendanceBreakPolicies
                .FirstOrDefaultAsync(x => x.Id == request.Policy.Id.Value && !x.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("AttendanceBreakPolicy", request.Policy.Id.Value);

            policy.Update(request.Policy, request.ModifiedBy);
        }
        else
        {
            policy = AttendanceBreakPolicy.Create(Guid.NewGuid(), request.Policy);
            await dbContext.AttendanceBreakPolicies.AddAsync(policy, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAttendanceBreakPolicyResult(policy.Adapt<AttendanceBreakPolicyDto>());
    }
}

public class CreateEmergencyLeaveRequestHandler(AttendanceDbContext dbContext)
    : ICommandHandler<CreateEmergencyLeaveRequestCommand, CreateEmergencyLeaveRequestResult>
{
    public async Task<CreateEmergencyLeaveRequestResult> Handle(CreateEmergencyLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        await EnsureLeaveContainsWorkingDayAsync(request.Request.CompanyId, request.Request.StartDate, request.Request.EndDate, cancellationToken);

        var leave = EmergencyLeaveRequest.Create(Guid.NewGuid(), request.Request);
        await dbContext.EmergencyLeaveRequests.AddAsync(leave, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
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
        var configuration = await dbContext.AttendanceConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        var configurationDto = configuration?.ToDto() ?? AttendanceConfiguration.DefaultDto(companyId);
        var holidays = await dbContext.AttendanceHolidays
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

public class ReviewEmergencyLeaveRequestHandler(AttendanceDbContext dbContext, ISender sender)
    : ICommandHandler<ReviewEmergencyLeaveRequestCommand, ReviewEmergencyLeaveRequestResult>
{
    public async Task<ReviewEmergencyLeaveRequestResult> Handle(ReviewEmergencyLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leave = await dbContext.EmergencyLeaveRequests
            .FirstOrDefaultAsync(x => x.Id == request.Review.RequestId, cancellationToken)
            ?? throw new NotFoundException("EmergencyLeaveRequest", request.Review.RequestId);

        await EnsureReviewerCanReviewAsync(request.ReviewerEmployeeId, leave.EmployeeId, cancellationToken);

        if (request.Review.IsApproved)
        {
            leave.Approve(request.ReviewedBy, request.Review.ApproverComment);
        }
        else
        {
            leave.Reject(request.ReviewedBy, request.Review.ApproverComment);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReviewEmergencyLeaveRequestResult(leave.Adapt<EmergencyLeaveRequestDto>());
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

public class GetEmergencyLeaveRequestsHandler(AttendanceDbContext dbContext, ISender sender)
    : IQueryHandler<GetEmergencyLeaveRequestsQuery, GetEmergencyLeaveRequestsResult>
{
    public async Task<GetEmergencyLeaveRequestsResult> Handle(GetEmergencyLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.EmergencyLeaveRequests.AsNoTracking()
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

public class ReviewMidDayPermissionRequestHandler(AttendanceDbContext dbContext)
    : ICommandHandler<ReviewMidDayPermissionRequestCommand, ReviewMidDayPermissionRequestResult>
{
    public async Task<ReviewMidDayPermissionRequestResult> Handle(ReviewMidDayPermissionRequestCommand request, CancellationToken cancellationToken)
    {
        var permission = await dbContext.MidDayPermissionRequests
            .FirstOrDefaultAsync(x => x.Id == request.Review.RequestId, cancellationToken)
            ?? throw new NotFoundException("MidDayPermissionRequest", request.Review.RequestId);

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
}

public class GetMidDayPermissionRequestsHandler(AttendanceDbContext dbContext)
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

        var total = await query.LongCountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.Date)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ProjectToType<MidDayPermissionRequestDto>()
            .ToListAsync(cancellationToken);

        return new GetMidDayPermissionRequestsResult(new PaginatedResult<MidDayPermissionRequestDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            total,
            rows));
    }
}

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
                ShiftStartUtc = x.ShiftStart,
                ShiftEndUtc = x.ShiftEnd,
                CheckInUtc = x.ActualStartTime,
                CheckOutUtc = x.ActualEndTime,
                TotalWorkingHours = x.TotalHours,
                NetWorkingHours = x.TotalHours
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

        if (MatchesCategory(request.Filter.Category, "EmergencyLeave"))
        {
            var leaves = dbContext.EmergencyLeaveRequests.AsNoTracking()
                .Where(x => x.CompanyId == request.Filter.CompanyId
                    && x.EndDate >= fromDate
                    && x.StartDate <= toDate);

            if (request.Filter.EmployeeId.HasValue)
            {
                leaves = leaves.Where(x => x.EmployeeId == request.Filter.EmployeeId.Value);
            }

            if (request.Filter.Status.HasValue)
            {
                leaves = leaves.Where(x => x.Status == request.Filter.Status.Value);
            }

            rows.AddRange(await leaves.Select(x => new AttendanceReportRowDto
            {
                Date = x.StartDate,
                EmployeeId = x.EmployeeId,
                Category = "EmergencyLeave",
                Status = x.Status,
                Reason = x.Reason,
                ApproverComment = x.ApproverComment
            }).ToListAsync(cancellationToken));
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
