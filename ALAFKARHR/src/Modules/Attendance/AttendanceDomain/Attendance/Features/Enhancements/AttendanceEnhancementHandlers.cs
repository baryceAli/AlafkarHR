using AttendanceDomain.Attendance.Models;
using EmployeeModule.Contracts.Employees.Features.GetCompanyEmployeeRosterProfiles;
using FluentValidation;
using Shared.Contracts.Leave;
using Shared.Pagination;

namespace AttendanceDomain.Attendance.Features.Enhancements;

public record GetShiftSchedulesQuery(Guid CompanyId, AttendanceRosterStatus? Status) : IQuery<GetShiftSchedulesResult>;
public record GetShiftSchedulesResult(List<ShiftScheduleDto> ScheduleList);
public record UpsertShiftScheduleCommand(UpsertShiftScheduleDto Schedule, string? UserId) : ICommand<UpsertShiftScheduleResult>;
public record UpsertShiftScheduleResult(ShiftScheduleDto Schedule);
public record ChangeShiftScheduleStatusCommand(Guid ScheduleId, AttendanceRosterStatus TargetStatus, string? UserId) : ICommand<UpsertShiftScheduleResult>;

public record GetShiftScheduleAssignmentsQuery(Guid? ScheduleId, Guid? EmployeeId, DateTime? FromDate, DateTime? ToDate, PaginationRequest PaginationRequest)
    : IQuery<GetShiftScheduleAssignmentsResult>;
public record GetShiftScheduleAssignmentsResult(PaginatedResult<ShiftScheduleAssignmentDto> AssignmentList);
public record UpsertShiftScheduleAssignmentCommand(UpsertShiftScheduleAssignmentDto Assignment, string? UserId) : ICommand<UpsertShiftScheduleAssignmentResult>;
public record UpsertShiftScheduleAssignmentResult(ShiftScheduleAssignmentDto Assignment);
public record BulkShiftScheduleAssignmentCommand(BulkShiftScheduleAssignmentDto Assignment, string? UserId) : ICommand<BulkShiftScheduleAssignmentResult>;
public record BulkShiftScheduleAssignmentResult(int CreatedCount);
public record DeleteShiftScheduleAssignmentCommand(Guid AssignmentId, string? UserId) : ICommand<DeleteShiftScheduleAssignmentResult>;
public record DeleteShiftScheduleAssignmentResult(bool IsSuccess);

public record GetShiftSwapRequestsQuery(Guid CompanyId, AttendanceExceptionStatus? Status, Guid? EmployeeId) : IQuery<GetShiftSwapRequestsResult>;
public record GetShiftSwapRequestsResult(List<ShiftSwapRequestDto> RequestList);
public record CreateShiftSwapRequestCommand(CreateShiftSwapRequestDto Request) : ICommand<CreateShiftSwapRequestResult>;
public record CreateShiftSwapRequestResult(ShiftSwapRequestDto Request);
public record ReviewShiftSwapRequestCommand(ReviewShiftSwapRequestDto Review, string? UserId) : ICommand<ReviewShiftSwapRequestResult>;
public record ReviewShiftSwapRequestResult(ShiftSwapRequestDto Request);
public record CancelShiftSwapRequestCommand(Guid RequestId, string? UserId) : ICommand<ReviewShiftSwapRequestResult>;

public record GetAttendanceCorrectionsQuery(Guid CompanyId, AttendanceExceptionStatus? Status, Guid? EmployeeId) : IQuery<GetAttendanceCorrectionsResult>;
public record GetAttendanceCorrectionsResult(List<AttendanceCorrectionDto> CorrectionList);
public record CreateAttendanceCorrectionCommand(CreateAttendanceCorrectionDto Correction) : ICommand<CreateAttendanceCorrectionResult>;
public record CreateAttendanceCorrectionResult(AttendanceCorrectionDto Correction);
public record ReviewAttendanceCorrectionCommand(ReviewAttendanceCorrectionDto Review, string? UserId) : ICommand<ReviewAttendanceCorrectionResult>;
public record ReviewAttendanceCorrectionResult(AttendanceCorrectionDto Correction);
public record ApplyAttendanceCorrectionCommand(Guid CorrectionId, string? UserId) : ICommand<ReviewAttendanceCorrectionResult>;

public record GetBiometricImportBatchesQuery(Guid CompanyId) : IQuery<GetBiometricImportBatchesResult>;
public record GetBiometricImportBatchesResult(List<BiometricImportBatchDto> BatchList);
public record CreateBiometricImportBatchCommand(CreateBiometricImportBatchDto Batch) : ICommand<CreateBiometricImportBatchResult>;
public record CreateBiometricImportBatchResult(BiometricImportBatchDto Batch);
public record UpsertBiometricImportRowCommand(UpsertBiometricImportRowDto Row, string? UserId) : ICommand<UpsertBiometricImportRowResult>;
public record UpsertBiometricImportRowResult(BiometricImportRowDto Row);
public record ReviewBiometricImportRowCommand(ReviewBiometricImportRowDto Review, string? UserId) : ICommand<UpsertBiometricImportRowResult>;
public record PostBiometricImportBatchCommand(Guid BatchId, string? UserId) : ICommand<CreateBiometricImportBatchResult>;

public record GetAttendanceWorkEntriesQuery(Guid CompanyId, Guid? EmployeeId, DateTime? FromDate, DateTime? ToDate, AttendanceWorkEntryStatus? Status)
    : IQuery<GetAttendanceWorkEntriesResult>;
public record GetAttendanceWorkEntriesResult(List<PayrollWorkEntryDto> EntryList);
public record GenerateAttendanceWorkEntriesCommand(GenerateAttendanceWorkEntriesDto Request, string? UserId) : ICommand<GenerateAttendanceWorkEntriesResult>;
public record GenerateAttendanceWorkEntriesResult(int CreatedCount, List<PayrollWorkEntryDto> EntryList);
public record UpsertAttendanceWorkEntryCommand(UpsertAttendanceWorkEntryDto Entry, string? UserId) : ICommand<UpsertAttendanceWorkEntryResult>;
public record UpsertAttendanceWorkEntryResult(PayrollWorkEntryDto Entry);
public record ChangeAttendanceWorkEntryStatusCommand(Guid EntryId, AttendanceWorkEntryStatus TargetStatus, string? UserId) : ICommand<UpsertAttendanceWorkEntryResult>;

public class UpsertShiftScheduleValidator : AbstractValidator<UpsertShiftScheduleCommand>
{
    public UpsertShiftScheduleValidator()
    {
        RuleFor(x => x.Schedule.CompanyId).NotEmpty();
        RuleFor(x => x.Schedule.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Schedule.EndDate.Date).GreaterThanOrEqualTo(x => x.Schedule.StartDate.Date);
    }
}

public class GetShiftSchedulesHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetShiftSchedulesQuery, GetShiftSchedulesResult>
{
    public async Task<GetShiftSchedulesResult> Handle(GetShiftSchedulesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.ShiftSchedules.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        var schedules = await query
            .OrderByDescending(x => x.StartDate)
            .Select(x => new ShiftScheduleDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Status = x.Status,
                Notes = x.Notes,
                PublishedAtUtc = x.PublishedAtUtc,
                LockedAtUtc = x.LockedAtUtc,
                AssignmentCount = dbContext.ShiftScheduleAssignments.Count(a => a.ScheduleId == x.Id && !a.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        return new GetShiftSchedulesResult(schedules);
    }
}

public class UpsertShiftScheduleHandler(AttendanceDbContext dbContext)
    : ICommandHandler<UpsertShiftScheduleCommand, UpsertShiftScheduleResult>
{
    public async Task<UpsertShiftScheduleResult> Handle(UpsertShiftScheduleCommand request, CancellationToken cancellationToken)
    {
        ShiftSchedule schedule;
        if (request.Schedule.Id.HasValue && request.Schedule.Id.Value != Guid.Empty)
        {
            schedule = await dbContext.ShiftSchedules.FirstOrDefaultAsync(x => x.Id == request.Schedule.Id.Value && !x.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("ShiftSchedule", request.Schedule.Id.Value);
            schedule.Update(request.Schedule, request.UserId);
        }
        else
        {
            schedule = ShiftSchedule.Create(Guid.NewGuid(), request.Schedule);
            await dbContext.ShiftSchedules.AddAsync(schedule, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertShiftScheduleResult(await MapScheduleAsync(schedule.Id, cancellationToken));
    }

    private async Task<ShiftScheduleDto> MapScheduleAsync(Guid id, CancellationToken cancellationToken)
        => (await new GetShiftSchedulesHandler(dbContext).Handle(new GetShiftSchedulesQuery(Guid.Empty, null), cancellationToken)).ScheduleList
            .FirstOrDefault(x => x.Id == id)
            ?? (await dbContext.ShiftSchedules.AsNoTracking().Where(x => x.Id == id).ProjectToType<ShiftScheduleDto>().FirstAsync(cancellationToken));
}

public class ChangeShiftScheduleStatusHandler(AttendanceDbContext dbContext)
    : ICommandHandler<ChangeShiftScheduleStatusCommand, UpsertShiftScheduleResult>
{
    public async Task<UpsertShiftScheduleResult> Handle(ChangeShiftScheduleStatusCommand request, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.ShiftSchedules.FirstOrDefaultAsync(x => x.Id == request.ScheduleId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("ShiftSchedule", request.ScheduleId);

        switch (request.TargetStatus)
        {
            case AttendanceRosterStatus.Published:
                schedule.Publish(request.UserId);
                break;
            case AttendanceRosterStatus.Locked:
                schedule.Lock(request.UserId);
                break;
            case AttendanceRosterStatus.Cancelled:
                schedule.Cancel(request.UserId);
                break;
            default:
                throw new BadRequestException("Unsupported schedule status transition.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertShiftScheduleResult(schedule.Adapt<ShiftScheduleDto>());
    }
}

public class GetShiftScheduleAssignmentsHandler(AttendanceDbContext dbContext)
    : IQueryHandler<GetShiftScheduleAssignmentsQuery, GetShiftScheduleAssignmentsResult>
{
    public async Task<GetShiftScheduleAssignmentsResult> Handle(GetShiftScheduleAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.ShiftScheduleAssignments.AsNoTracking().Where(x => !x.IsDeleted);

        if (request.ScheduleId.HasValue) query = query.Where(x => x.ScheduleId == request.ScheduleId.Value);
        if (request.EmployeeId.HasValue) query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        if (request.FromDate.HasValue) query = query.Where(x => x.WorkDate >= UtcDateTime.Normalize(request.FromDate.Value).Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.WorkDate <= UtcDateTime.Normalize(request.ToDate.Value).Date);

        var total = await query.LongCountAsync(cancellationToken);
        var assignments = await query
            .OrderBy(x => x.WorkDate)
            .ThenBy(x => x.EmployeeId)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(x => new ShiftScheduleAssignmentDto
            {
                Id = x.Id,
                ScheduleId = x.ScheduleId,
                CompanyId = x.CompanyId,
                EmployeeId = x.EmployeeId,
                ShiftId = x.ShiftId,
                WorkDate = x.WorkDate,
                ShiftName = dbContext.Shifts.Where(s => s.Id == x.ShiftId).Select(s => s.Name).FirstOrDefault(),
                Notes = x.Notes
            })
            .ToListAsync(cancellationToken);

        return new GetShiftScheduleAssignmentsResult(new PaginatedResult<ShiftScheduleAssignmentDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            total,
            assignments));
    }
}

public class UpsertShiftScheduleAssignmentHandler(AttendanceDbContext dbContext)
    : ICommandHandler<UpsertShiftScheduleAssignmentCommand, UpsertShiftScheduleAssignmentResult>
{
    public async Task<UpsertShiftScheduleAssignmentResult> Handle(UpsertShiftScheduleAssignmentCommand request, CancellationToken cancellationToken)
    {
        await EnsureScheduleCanEditAsync(dbContext, request.Assignment.ScheduleId, cancellationToken);
        ShiftScheduleAssignment assignment;
        if (request.Assignment.Id.HasValue && request.Assignment.Id.Value != Guid.Empty)
        {
            assignment = await dbContext.ShiftScheduleAssignments.FirstOrDefaultAsync(x => x.Id == request.Assignment.Id.Value && !x.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("ShiftScheduleAssignment", request.Assignment.Id.Value);
            assignment.Update(request.Assignment, request.UserId);
        }
        else
        {
            assignment = ShiftScheduleAssignment.Create(Guid.NewGuid(), request.Assignment);
            await dbContext.ShiftScheduleAssignments.AddAsync(assignment, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertShiftScheduleAssignmentResult(assignment.Adapt<ShiftScheduleAssignmentDto>());
    }

    internal static async Task EnsureScheduleCanEditAsync(AttendanceDbContext dbContext, Guid scheduleId, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.ShiftSchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == scheduleId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("ShiftSchedule", scheduleId);
        if (schedule.Status is AttendanceRosterStatus.Locked or AttendanceRosterStatus.Cancelled)
        {
            throw new BadRequestException("Roster assignments cannot be changed for locked or cancelled schedules.");
        }
    }
}

public class BulkShiftScheduleAssignmentHandler(AttendanceDbContext dbContext)
    : ICommandHandler<BulkShiftScheduleAssignmentCommand, BulkShiftScheduleAssignmentResult>
{
    public async Task<BulkShiftScheduleAssignmentResult> Handle(BulkShiftScheduleAssignmentCommand request, CancellationToken cancellationToken)
    {
        await UpsertShiftScheduleAssignmentHandler.EnsureScheduleCanEditAsync(dbContext, request.Assignment.ScheduleId, cancellationToken);

        if (request.Assignment.EmployeeIds.Count == 0)
        {
            throw new BadRequestException("At least one employee is required for bulk roster assignment.");
        }

        var from = UtcDateTime.Normalize(request.Assignment.StartDate).Date;
        var to = UtcDateTime.Normalize(request.Assignment.EndDate).Date;
        if (to < from)
        {
            throw new BadRequestException("Assignment end date must be on or after start date.");
        }

        var created = 0;
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            foreach (var employeeId in request.Assignment.EmployeeIds.Distinct())
            {
                var exists = await dbContext.ShiftScheduleAssignments.AnyAsync(x =>
                    x.ScheduleId == request.Assignment.ScheduleId
                    && x.EmployeeId == employeeId
                    && x.WorkDate == date
                    && !x.IsDeleted, cancellationToken);
                if (exists)
                {
                    continue;
                }

                await dbContext.ShiftScheduleAssignments.AddAsync(ShiftScheduleAssignment.Create(Guid.NewGuid(), new UpsertShiftScheduleAssignmentDto
                {
                    ScheduleId = request.Assignment.ScheduleId,
                    CompanyId = request.Assignment.CompanyId,
                    EmployeeId = employeeId,
                    ShiftId = request.Assignment.ShiftId,
                    WorkDate = date,
                    Notes = request.Assignment.Notes
                }), cancellationToken);
                created++;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new BulkShiftScheduleAssignmentResult(created);
    }
}

public class DeleteShiftScheduleAssignmentHandler(AttendanceDbContext dbContext)
    : ICommandHandler<DeleteShiftScheduleAssignmentCommand, DeleteShiftScheduleAssignmentResult>
{
    public async Task<DeleteShiftScheduleAssignmentResult> Handle(DeleteShiftScheduleAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await dbContext.ShiftScheduleAssignments.FirstOrDefaultAsync(x => x.Id == request.AssignmentId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("ShiftScheduleAssignment", request.AssignmentId);

        assignment.Delete(request.UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteShiftScheduleAssignmentResult(true);
    }
}

public class ShiftSwapHandlers(AttendanceDbContext dbContext) :
    IQueryHandler<GetShiftSwapRequestsQuery, GetShiftSwapRequestsResult>,
    ICommandHandler<CreateShiftSwapRequestCommand, CreateShiftSwapRequestResult>,
    ICommandHandler<ReviewShiftSwapRequestCommand, ReviewShiftSwapRequestResult>,
    ICommandHandler<CancelShiftSwapRequestCommand, ReviewShiftSwapRequestResult>
{
    public async Task<GetShiftSwapRequestsResult> Handle(GetShiftSwapRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.ShiftSwapRequests.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
        if (request.EmployeeId.HasValue) query = query.Where(x => x.RequestingEmployeeId == request.EmployeeId.Value || x.TargetEmployeeId == request.EmployeeId.Value);

        return new GetShiftSwapRequestsResult(await query.OrderByDescending(x => x.WorkDate).ProjectToType<ShiftSwapRequestDto>().ToListAsync(cancellationToken));
    }

    public async Task<CreateShiftSwapRequestResult> Handle(CreateShiftSwapRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = ShiftSwapRequest.Create(Guid.NewGuid(), request.Request);
        await dbContext.ShiftSwapRequests.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateShiftSwapRequestResult(entity.Adapt<ShiftSwapRequestDto>());
    }

    public async Task<ReviewShiftSwapRequestResult> Handle(ReviewShiftSwapRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ShiftSwapRequests.FirstOrDefaultAsync(x => x.Id == request.Review.RequestId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("ShiftSwapRequest", request.Review.RequestId);
        entity.Review(request.Review.IsApproved, request.Review.ManagerNote, request.UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReviewShiftSwapRequestResult(entity.Adapt<ShiftSwapRequestDto>());
    }

    public async Task<ReviewShiftSwapRequestResult> Handle(CancelShiftSwapRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ShiftSwapRequests.FirstOrDefaultAsync(x => x.Id == request.RequestId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("ShiftSwapRequest", request.RequestId);
        entity.Cancel(request.UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReviewShiftSwapRequestResult(entity.Adapt<ShiftSwapRequestDto>());
    }
}

public class AttendanceCorrectionHandlers(AttendanceDbContext dbContext) :
    IQueryHandler<GetAttendanceCorrectionsQuery, GetAttendanceCorrectionsResult>,
    ICommandHandler<CreateAttendanceCorrectionCommand, CreateAttendanceCorrectionResult>,
    ICommandHandler<ReviewAttendanceCorrectionCommand, ReviewAttendanceCorrectionResult>,
    ICommandHandler<ApplyAttendanceCorrectionCommand, ReviewAttendanceCorrectionResult>
{
    public async Task<GetAttendanceCorrectionsResult> Handle(GetAttendanceCorrectionsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.AttendanceCorrections.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
        if (request.EmployeeId.HasValue) query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        return new GetAttendanceCorrectionsResult(await query.OrderByDescending(x => x.WorkDate).ProjectToType<AttendanceCorrectionDto>().ToListAsync(cancellationToken));
    }

    public async Task<CreateAttendanceCorrectionResult> Handle(CreateAttendanceCorrectionCommand request, CancellationToken cancellationToken)
    {
        var currentSession = await ResolveCurrentSessionAsync(request.Correction, cancellationToken);
        var entity = AttendanceCorrection.Create(Guid.NewGuid(), request.Correction, currentSession);
        await dbContext.AttendanceCorrections.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateAttendanceCorrectionResult(entity.Adapt<AttendanceCorrectionDto>());
    }

    public async Task<ReviewAttendanceCorrectionResult> Handle(ReviewAttendanceCorrectionCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.AttendanceCorrections.FirstOrDefaultAsync(x => x.Id == request.Review.CorrectionId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("AttendanceCorrection", request.Review.CorrectionId);
        entity.Review(request.Review.IsApproved, request.Review.ManagerNote, request.UserId);
        if (request.Review.IsApproved)
        {
            await ApplyCorrectionAsync(entity, request.UserId, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReviewAttendanceCorrectionResult(entity.Adapt<AttendanceCorrectionDto>());
    }

    public async Task<ReviewAttendanceCorrectionResult> Handle(ApplyAttendanceCorrectionCommand request, CancellationToken cancellationToken)
    {
        var correction = await dbContext.AttendanceCorrections.FirstOrDefaultAsync(x => x.Id == request.CorrectionId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("AttendanceCorrection", request.CorrectionId);

        await ApplyCorrectionAsync(correction, request.UserId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReviewAttendanceCorrectionResult(correction.Adapt<AttendanceCorrectionDto>());
    }

    private async Task ApplyCorrectionAsync(AttendanceCorrection correction, string? userId, CancellationToken cancellationToken)
    {
        correction.MarkApplied(userId);
        if (correction.SessionId.HasValue)
        {
            var session = await dbContext.AttendanceSessions.FirstOrDefaultAsync(x => x.Id == correction.SessionId.Value, cancellationToken);
            session?.Normalize(correction.CorrectedCheckInUtc, correction.CorrectedCheckOutUtc, false, correction.ManagerNote ?? correction.Reason, userId ?? "system");
        }

        await EnsureWorkEntryAsync(correction.CompanyId, correction.EmployeeId, correction.WorkDate, AttendanceWorkEntryType.ManualCorrection, WorkHours(correction.CorrectedCheckInUtc, correction.CorrectedCheckOutUtc), "AttendanceCorrection", correction.Id, correction.Reason, userId, cancellationToken);
    }

    private async Task<AttendanceSession?> ResolveCurrentSessionAsync(CreateAttendanceCorrectionDto correction, CancellationToken cancellationToken)
    {
        if (correction.SessionId.HasValue)
        {
            return await dbContext.AttendanceSessions.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == correction.SessionId.Value && x.CompanyId == correction.CompanyId && x.EmployeeId == correction.EmployeeId, cancellationToken);
        }

        var workDate = UtcDateTime.Normalize(correction.WorkDate).Date;
        return await dbContext.AttendanceSessions.AsNoTracking()
            .Where(x => x.CompanyId == correction.CompanyId
                && x.EmployeeId == correction.EmployeeId
                && x.ShiftStart.Date == workDate)
            .OrderByDescending(x => x.ActualStartTime ?? x.ShiftStart)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task EnsureWorkEntryAsync(Guid companyId, Guid employeeId, DateTime workDate, AttendanceWorkEntryType type, decimal hours, string sourceModule, Guid sourceDocumentId, string? notes, string? userId, CancellationToken cancellationToken)
    {
        var existing = await dbContext.AttendanceWorkEntries.FirstOrDefaultAsync(x => x.SourceModule == sourceModule && x.SourceDocumentId == sourceDocumentId && !x.IsDeleted, cancellationToken);
        if (existing is not null)
        {
            existing.Update(new UpsertAttendanceWorkEntryDto
            {
                Id = existing.Id,
                CompanyId = companyId,
                EmployeeId = employeeId,
                WorkDate = workDate,
                EntryType = type,
                Hours = hours,
                SourceModule = sourceModule,
                SourceDocumentId = sourceDocumentId,
                Notes = notes
            }, userId);
            return;
        }

        await dbContext.AttendanceWorkEntries.AddAsync(AttendanceWorkEntry.Create(Guid.NewGuid(), new UpsertAttendanceWorkEntryDto
        {
            CompanyId = companyId,
            EmployeeId = employeeId,
            WorkDate = workDate,
            EntryType = type,
            Hours = hours,
            SourceModule = sourceModule,
            SourceDocumentId = sourceDocumentId,
            Notes = notes
        }), cancellationToken);
    }

    private static decimal WorkHours(DateTime? start, DateTime? end)
        => start.HasValue && end.HasValue && end.Value > start.Value
            ? decimal.Round((decimal)(end.Value - start.Value).TotalHours, 2)
            : 0;
}

public class BiometricImportHandlers(AttendanceDbContext dbContext) :
    IQueryHandler<GetBiometricImportBatchesQuery, GetBiometricImportBatchesResult>,
    ICommandHandler<CreateBiometricImportBatchCommand, CreateBiometricImportBatchResult>,
    ICommandHandler<UpsertBiometricImportRowCommand, UpsertBiometricImportRowResult>,
    ICommandHandler<ReviewBiometricImportRowCommand, UpsertBiometricImportRowResult>,
    ICommandHandler<PostBiometricImportBatchCommand, CreateBiometricImportBatchResult>
{
    public async Task<GetBiometricImportBatchesResult> Handle(GetBiometricImportBatchesQuery request, CancellationToken cancellationToken)
    {
        var batches = await dbContext.BiometricImportBatches.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderByDescending(x => x.ImportedAtUtc)
            .ProjectToType<BiometricImportBatchDto>()
            .ToListAsync(cancellationToken);
        return new GetBiometricImportBatchesResult(batches);
    }

    public async Task<CreateBiometricImportBatchResult> Handle(CreateBiometricImportBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = BiometricImportBatch.Create(Guid.NewGuid(), request.Batch);
        await dbContext.BiometricImportBatches.AddAsync(batch, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateBiometricImportBatchResult(batch.Adapt<BiometricImportBatchDto>());
    }

    public async Task<UpsertBiometricImportRowResult> Handle(UpsertBiometricImportRowCommand request, CancellationToken cancellationToken)
    {
        BiometricImportRow row;
        if (request.Row.Id.HasValue && request.Row.Id.Value != Guid.Empty)
        {
            row = await dbContext.BiometricImportRows.FirstOrDefaultAsync(x => x.Id == request.Row.Id.Value && !x.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("BiometricImportRow", request.Row.Id.Value);
            row.Update(request.Row, request.UserId);
        }
        else
        {
            row = BiometricImportRow.Create(Guid.NewGuid(), request.Row);
            await dbContext.BiometricImportRows.AddAsync(row, cancellationToken);
        }

        await RefreshImportCountsAsync(request.Row.BatchId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertBiometricImportRowResult(row.Adapt<BiometricImportRowDto>());
    }

    public async Task<UpsertBiometricImportRowResult> Handle(ReviewBiometricImportRowCommand request, CancellationToken cancellationToken)
    {
        var row = await dbContext.BiometricImportRows.FirstOrDefaultAsync(x => x.Id == request.Review.RowId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("BiometricImportRow", request.Review.RowId);
        row.Review(request.Review.IsAccepted, request.Review.ErrorMessage, request.UserId);
        await RefreshImportCountsAsync(row.BatchId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertBiometricImportRowResult(row.Adapt<BiometricImportRowDto>());
    }

    public async Task<CreateBiometricImportBatchResult> Handle(PostBiometricImportBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = await dbContext.BiometricImportBatches.FirstOrDefaultAsync(x => x.Id == request.BatchId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("BiometricImportBatch", request.BatchId);
        var rows = await dbContext.BiometricImportRows.Where(x => x.BatchId == request.BatchId && x.Status == AttendanceImportRowStatus.Accepted && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var row in rows)
        {
            row.MarkPosted(request.UserId);
        }

        batch.MarkPosted(request.UserId);
        await RefreshImportCountsAsync(batch.Id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateBiometricImportBatchResult(batch.Adapt<BiometricImportBatchDto>());
    }

    private async Task RefreshImportCountsAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await dbContext.BiometricImportBatches.FirstOrDefaultAsync(x => x.Id == batchId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("BiometricImportBatch", batchId);
        var rows = await dbContext.BiometricImportRows.AsNoTracking().Where(x => x.BatchId == batchId && !x.IsDeleted).ToListAsync(cancellationToken);
        batch.SetCounts(rows.Count, rows.Count(x => x.Status is AttendanceImportRowStatus.Accepted or AttendanceImportRowStatus.Posted), rows.Count(x => x.Status == AttendanceImportRowStatus.Rejected));
    }
}

public class AttendanceWorkEntryHandlers(AttendanceDbContext dbContext, ISender sender) :
    IQueryHandler<GetAttendanceWorkEntriesQuery, GetAttendanceWorkEntriesResult>,
    ICommandHandler<GenerateAttendanceWorkEntriesCommand, GenerateAttendanceWorkEntriesResult>,
    ICommandHandler<UpsertAttendanceWorkEntryCommand, UpsertAttendanceWorkEntryResult>,
    ICommandHandler<ChangeAttendanceWorkEntryStatusCommand, UpsertAttendanceWorkEntryResult>
{
    public async Task<GetAttendanceWorkEntriesResult> Handle(GetAttendanceWorkEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.AttendanceWorkEntries.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.EmployeeId.HasValue) query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        if (request.FromDate.HasValue) query = query.Where(x => x.WorkDate >= UtcDateTime.Normalize(request.FromDate.Value).Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.WorkDate <= UtcDateTime.Normalize(request.ToDate.Value).Date);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
        return new GetAttendanceWorkEntriesResult(await query.OrderByDescending(x => x.WorkDate).ProjectToType<PayrollWorkEntryDto>().ToListAsync(cancellationToken));
    }

    public async Task<GenerateAttendanceWorkEntriesResult> Handle(GenerateAttendanceWorkEntriesCommand request, CancellationToken cancellationToken)
    {
        var from = UtcDateTime.Normalize(request.Request.FromDate).Date;
        var to = UtcDateTime.Normalize(request.Request.ToDate).Date;
        if (to < from)
        {
            throw new BadRequestException("Work entry generation end date must be on or after start date.");
        }

        var created = 0;
        var completedSessions = await dbContext.AttendanceSessions.AsNoTracking()
            .Where(x => x.CompanyId == request.Request.CompanyId
                && x.Status == AttendanceSessionStatus.Completed
                && x.ShiftStart.Date >= from
                && x.ShiftStart.Date <= to
                && (!request.Request.EmployeeId.HasValue || x.EmployeeId == request.Request.EmployeeId.Value))
            .ToListAsync(cancellationToken);

        foreach (var session in completedSessions)
        {
            created += await EnsureWorkEntryAsync(session.CompanyId, session.EmployeeId, session.ShiftStart.Date, AttendanceWorkEntryType.Regular, session.TotalHours, "AttendanceSession", session.Id, session.NormalizationNote, cancellationToken);
            var expectedHours = Math.Max(0, (decimal)(session.ShiftEnd - session.ShiftStart).TotalHours);
            if (session.TotalHours > expectedHours)
            {
                created += await EnsureWorkEntryAsync(session.CompanyId, session.EmployeeId, session.ShiftStart.Date, AttendanceWorkEntryType.Overtime, session.TotalHours - expectedHours, "AttendanceSessionOvertime", session.Id, "Generated overtime from attendance session.", cancellationToken);
            }
        }

        var plannedRows = await ResolvePlannedRosterRowsAsync(request.Request, from, to, cancellationToken);
        var plannedEmployeeIds = plannedRows.Select(x => x.EmployeeId).Distinct().ToList();
        var sessions = await dbContext.AttendanceSessions.AsNoTracking()
            .Where(x => x.CompanyId == request.Request.CompanyId
                && x.ShiftStart.Date >= from
                && x.ShiftStart.Date <= to
                && plannedEmployeeIds.Contains(x.EmployeeId))
            .ToListAsync(cancellationToken);
        var approvedLeaveDays = await sender.Send(
            new GetApprovedLeaveCoverageQuery(request.Request.CompanyId, plannedEmployeeIds, from, to),
            cancellationToken);
        var approvedLeaveLookup = approvedLeaveDays.Days
            .Select(x => (x.EmployeeId, Date: UtcDateTime.Normalize(x.Date).Date))
            .ToHashSet();

        foreach (var plannedRow in plannedRows)
        {
            var hasSession = sessions.Any(x => x.EmployeeId == plannedRow.EmployeeId && x.ShiftStart.Date == plannedRow.WorkDate);
            var hasApprovedLeave = approvedLeaveLookup.Contains((plannedRow.EmployeeId, plannedRow.WorkDate));
            if (!hasSession && !hasApprovedLeave)
            {
                created += await EnsureWorkEntryAsync(
                    request.Request.CompanyId,
                    plannedRow.EmployeeId,
                    plannedRow.WorkDate,
                    AttendanceWorkEntryType.Absence,
                    0,
                    plannedRow.SourceModule,
                    plannedRow.SourceDocumentId,
                    plannedRow.Notes,
                    cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        var entries = await Handle(new GetAttendanceWorkEntriesQuery(request.Request.CompanyId, request.Request.EmployeeId, from, to, null), cancellationToken);
        return new GenerateAttendanceWorkEntriesResult(created, entries.EntryList);
    }

    private async Task<List<PlannedRosterWorkEntry>> ResolvePlannedRosterRowsAsync(
        GenerateAttendanceWorkEntriesDto request,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken)
    {
        var employeeResult = await sender.Send(new GetCompanyEmployeeRosterProfilesQuery(request.CompanyId), cancellationToken);
        var substituteConfigs = await dbContext.AttendanceRosterSubstituteConfigurations.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .ToDictionaryAsync(x => x.EmployeeId, cancellationToken);
        var employees = employeeResult.Employees
            .Where(x => x.IsActive && x.AdministrationId.HasValue && x.AdministrationId.Value != Guid.Empty)
            .Where(x => !request.EmployeeId.HasValue || x.EmployeeId == request.EmployeeId.Value)
            .Where(x => IsRosterVisible(x, substituteConfigs))
            .ToList();
        var employeeIds = employees.Select(x => x.EmployeeId).ToHashSet();

        if (employeeIds.Count == 0)
        {
            return [];
        }

        var explicitAssignments = await dbContext.ShiftScheduleAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId
                && x.WorkDate >= from
                && x.WorkDate <= to
                && !x.IsDeleted
                && employeeIds.Contains(x.EmployeeId))
            .ToListAsync(cancellationToken);

        var baseAssignments = await dbContext.EmployeeShifts.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId
                && x.IsActive
                && !x.IsDeleted
                && x.EffectiveFrom.Date <= to
                && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= from))
            .ToListAsync(cancellationToken);

        var rows = new List<PlannedRosterWorkEntry>();
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            foreach (var employee in employees)
            {
                var explicitAssignment = explicitAssignments
                    .FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.WorkDate == date);
                if (explicitAssignment is not null)
                {
                    rows.Add(new PlannedRosterWorkEntry(
                        explicitAssignment.EmployeeId,
                        explicitAssignment.WorkDate,
                        "ShiftScheduleAssignmentAbsence",
                        explicitAssignment.Id,
                        "Generated absence from roster assignment without attendance session."));
                    continue;
                }

                var baseAssignment = ResolveBaseAssignment(employee, date, baseAssignments);
                if (baseAssignment is null)
                {
                    continue;
                }

                rows.Add(new PlannedRosterWorkEntry(
                    employee.EmployeeId,
                    date,
                    "EmployeeShiftAbsence",
                    DeterministicGuid($"EmployeeShiftAbsence|{request.CompanyId:N}|{employee.EmployeeId:N}|{date:yyyyMMdd}"),
                    "Generated absence from baseline shift assignment without attendance session."));
            }
        }

        return rows;
    }

    private static EmployeeShift? ResolveBaseAssignment(
        EmployeeRosterProfileDto employee,
        DateTime date,
        List<EmployeeShift> baseAssignments)
        => baseAssignments
            .Where(x => x.EffectiveFrom.Date <= date && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= date))
            .Where(x =>
                (x.Scope == ShiftAssignmentScope.Employee && x.EmployeeId == employee.EmployeeId)
                || (x.Scope == ShiftAssignmentScope.Department && employee.DepartmentId.HasValue && x.DepartmentId == employee.DepartmentId.Value)
                || (x.Scope == ShiftAssignmentScope.Administration && x.AdministrationId.HasValue && x.AdministrationId == employee.AdministrationId)
                || (x.Scope == ShiftAssignmentScope.Company && x.CompanyId == employee.CompanyId))
            .OrderByDescending(x => ScopePriority(x.Scope))
            .ThenByDescending(x => x.EffectiveFrom)
            .FirstOrDefault();

    private static int ScopePriority(ShiftAssignmentScope scope) => scope switch
    {
        ShiftAssignmentScope.Employee => 4,
        ShiftAssignmentScope.Department => 3,
        ShiftAssignmentScope.Administration => 2,
        _ => 1
    };

    private static bool IsRosterVisible(
        EmployeeRosterProfileDto employee,
        Dictionary<Guid, AttendanceRosterSubstituteConfiguration> configs)
        => !configs.TryGetValue(employee.EmployeeId, out var config) || config.IsRosterVisible;

    private static Guid DeterministicGuid(string value)
    {
        var bytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(value));
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x30);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
        return new Guid(bytes);
    }

    public async Task<UpsertAttendanceWorkEntryResult> Handle(UpsertAttendanceWorkEntryCommand request, CancellationToken cancellationToken)
    {
        AttendanceWorkEntry entry;
        if (request.Entry.Id.HasValue && request.Entry.Id.Value != Guid.Empty)
        {
            entry = await dbContext.AttendanceWorkEntries.FirstOrDefaultAsync(x => x.Id == request.Entry.Id.Value && !x.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("AttendanceWorkEntry", request.Entry.Id.Value);
            entry.Update(request.Entry, request.UserId);
        }
        else
        {
            entry = AttendanceWorkEntry.Create(Guid.NewGuid(), request.Entry);
            await dbContext.AttendanceWorkEntries.AddAsync(entry, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAttendanceWorkEntryResult(entry.Adapt<PayrollWorkEntryDto>());
    }

    public async Task<UpsertAttendanceWorkEntryResult> Handle(ChangeAttendanceWorkEntryStatusCommand request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.AttendanceWorkEntries.FirstOrDefaultAsync(x => x.Id == request.EntryId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("AttendanceWorkEntry", request.EntryId);
        if (request.TargetStatus == AttendanceWorkEntryStatus.Approved)
        {
            entry.Approve(request.UserId);
        }
        else if (request.TargetStatus == AttendanceWorkEntryStatus.Locked)
        {
            entry.Lock(request.UserId);
        }
        else
        {
            throw new BadRequestException("Unsupported work entry status transition.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAttendanceWorkEntryResult(entry.Adapt<PayrollWorkEntryDto>());
    }

    private async Task<int> EnsureWorkEntryAsync(Guid companyId, Guid employeeId, DateTime workDate, AttendanceWorkEntryType type, decimal hours, string sourceModule, Guid sourceDocumentId, string? notes, CancellationToken cancellationToken)
    {
        var normalizedWorkDate = UtcDateTime.Normalize(workDate).Date;
        var exists = await dbContext.AttendanceWorkEntries.AnyAsync(x =>
            !x.IsDeleted
            && x.EntryType == type
            && ((x.SourceModule == sourceModule && x.SourceDocumentId == sourceDocumentId)
                || (type == AttendanceWorkEntryType.Absence
                    && x.CompanyId == companyId
                    && x.EmployeeId == employeeId
                    && x.WorkDate == normalizedWorkDate)),
            cancellationToken);
        if (exists)
        {
            return 0;
        }

        await dbContext.AttendanceWorkEntries.AddAsync(AttendanceWorkEntry.Create(Guid.NewGuid(), new UpsertAttendanceWorkEntryDto
        {
            CompanyId = companyId,
            EmployeeId = employeeId,
            WorkDate = workDate,
            EntryType = type,
            Hours = hours,
            SourceModule = sourceModule,
            SourceDocumentId = sourceDocumentId,
            Notes = notes
        }), cancellationToken);
        return 1;
    }

    private sealed record PlannedRosterWorkEntry(
        Guid EmployeeId,
        DateTime WorkDate,
        string SourceModule,
        Guid SourceDocumentId,
        string Notes);
}
