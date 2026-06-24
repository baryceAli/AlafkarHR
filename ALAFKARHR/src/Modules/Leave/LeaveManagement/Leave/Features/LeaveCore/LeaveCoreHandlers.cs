using AttendanceDomain.Attendance.Models;
using AttendanceDomain.Data;
using LeaveManagement.Data;
using LeaveManagement.Leave.Models;

namespace LeaveManagement.Leave.Features.LeaveCore;

public record GetLeaveTypesQuery(Guid CompanyId) : IQuery<GetLeaveTypesResult>;
public record GetLeaveTypesResult(List<LeaveTypeDto> LeaveTypes);
public record UpsertLeaveTypeCommand(UpsertLeaveTypeDto LeaveType, string? UserId) : ICommand<UpsertLeaveTypeResult>;
public record UpsertLeaveTypeResult(LeaveTypeDto LeaveType);
public record DeleteLeaveTypeCommand(Guid Id, string? UserId) : ICommand<DeleteLeaveTypeResult>;
public record DeleteLeaveTypeResult(bool Success);

public record GetLeavePeriodsQuery(Guid CompanyId) : IQuery<GetLeavePeriodsResult>;
public record GetLeavePeriodsResult(List<LeavePeriodDto> LeavePeriods);
public record UpsertLeavePeriodCommand(UpsertLeavePeriodDto LeavePeriod, string? UserId) : ICommand<UpsertLeavePeriodResult>;
public record UpsertLeavePeriodResult(LeavePeriodDto LeavePeriod);
public record DeleteLeavePeriodCommand(Guid Id, string? UserId) : ICommand<DeleteLeavePeriodResult>;
public record DeleteLeavePeriodResult(bool Success);

public record GetLeavePoliciesQuery(Guid CompanyId) : IQuery<GetLeavePoliciesResult>;
public record GetLeavePoliciesResult(List<LeavePolicyDto> LeavePolicies);
public record UpsertLeavePolicyCommand(UpsertLeavePolicyDto LeavePolicy, string? UserId) : ICommand<UpsertLeavePolicyResult>;
public record UpsertLeavePolicyResult(LeavePolicyDto LeavePolicy);
public record DeleteLeavePolicyCommand(Guid Id, string? UserId) : ICommand<DeleteLeavePolicyResult>;
public record DeleteLeavePolicyResult(bool Success);

public record GetLeavePolicyAssignmentsQuery(Guid CompanyId) : IQuery<GetLeavePolicyAssignmentsResult>;
public record GetLeavePolicyAssignmentsResult(List<LeavePolicyAssignmentDto> Assignments);
public record UpsertLeavePolicyAssignmentCommand(UpsertLeavePolicyAssignmentDto Assignment, string? UserId) : ICommand<UpsertLeavePolicyAssignmentResult>;
public record UpsertLeavePolicyAssignmentResult(LeavePolicyAssignmentDto Assignment);
public record DeleteLeavePolicyAssignmentCommand(Guid Id, string? UserId) : ICommand<DeleteLeavePolicyAssignmentResult>;
public record DeleteLeavePolicyAssignmentResult(bool Success);
public record GenerateLeaveAllocationsCommand(GenerateLeaveAllocationsDto Request, string? UserId) : ICommand<GenerateLeaveAllocationsResult>;
public record GenerateLeaveAllocationsResult(int CreatedEntries);

public record GetLeaveApplicationsQuery(Guid CompanyId, Guid? EmployeeId, LeaveApplicationStatus? Status) : IQuery<GetLeaveApplicationsResult>;
public record GetLeaveApplicationsResult(List<LeaveApplicationDto> Applications);
public record UpsertLeaveApplicationCommand(UpsertLeaveApplicationDto Application, string? UserId) : ICommand<UpsertLeaveApplicationResult>;
public record UpsertLeaveApplicationResult(LeaveApplicationDto Application);
public record SubmitLeaveApplicationCommand(Guid Id, string? UserId) : ICommand<LeaveApplicationActionResult>;
public record ReviewLeaveApplicationCommand(ReviewLeaveApplicationDto Review, string UserId) : ICommand<LeaveApplicationActionResult>;
public record CancelLeaveApplicationCommand(Guid Id, string? UserId) : ICommand<LeaveApplicationActionResult>;
public record LeaveApplicationActionResult(LeaveApplicationDto Application);

public record GetLeaveLedgerEntriesQuery(Guid CompanyId, Guid? EmployeeId, Guid? LeaveTypeId, Guid? LeavePeriodId) : IQuery<GetLeaveLedgerEntriesResult>;
public record GetLeaveLedgerEntriesResult(List<LeaveLedgerEntryDto> Entries);
public record CreateLeaveLedgerAdjustmentCommand(CreateLeaveLedgerAdjustmentDto Adjustment, string? UserId) : ICommand<CreateLeaveLedgerAdjustmentResult>;
public record CreateLeaveLedgerAdjustmentResult(LeaveLedgerEntryDto Entry);
public record CreateLeaveEncashmentCommand(CreateLeaveEncashmentDto Encashment, string? UserId) : ICommand<CreateLeaveEncashmentResult>;
public record CreateLeaveEncashmentResult(LeaveEncashmentDto Encashment, LeaveLedgerEntryDto Entry);

public class LeaveCoreHandler(LeaveDbContext leaveDbContext, AttendanceDbContext attendanceDbContext) :
    IQueryHandler<GetLeaveTypesQuery, GetLeaveTypesResult>,
    ICommandHandler<UpsertLeaveTypeCommand, UpsertLeaveTypeResult>,
    ICommandHandler<DeleteLeaveTypeCommand, DeleteLeaveTypeResult>,
    IQueryHandler<GetLeavePeriodsQuery, GetLeavePeriodsResult>,
    ICommandHandler<UpsertLeavePeriodCommand, UpsertLeavePeriodResult>,
    ICommandHandler<DeleteLeavePeriodCommand, DeleteLeavePeriodResult>,
    IQueryHandler<GetLeavePoliciesQuery, GetLeavePoliciesResult>,
    ICommandHandler<UpsertLeavePolicyCommand, UpsertLeavePolicyResult>,
    ICommandHandler<DeleteLeavePolicyCommand, DeleteLeavePolicyResult>,
    IQueryHandler<GetLeavePolicyAssignmentsQuery, GetLeavePolicyAssignmentsResult>,
    ICommandHandler<UpsertLeavePolicyAssignmentCommand, UpsertLeavePolicyAssignmentResult>,
    ICommandHandler<DeleteLeavePolicyAssignmentCommand, DeleteLeavePolicyAssignmentResult>,
    ICommandHandler<GenerateLeaveAllocationsCommand, GenerateLeaveAllocationsResult>,
    IQueryHandler<GetLeaveApplicationsQuery, GetLeaveApplicationsResult>,
    ICommandHandler<UpsertLeaveApplicationCommand, UpsertLeaveApplicationResult>,
    ICommandHandler<SubmitLeaveApplicationCommand, LeaveApplicationActionResult>,
    ICommandHandler<ReviewLeaveApplicationCommand, LeaveApplicationActionResult>,
    ICommandHandler<CancelLeaveApplicationCommand, LeaveApplicationActionResult>,
    IQueryHandler<GetLeaveLedgerEntriesQuery, GetLeaveLedgerEntriesResult>,
    ICommandHandler<CreateLeaveLedgerAdjustmentCommand, CreateLeaveLedgerAdjustmentResult>,
    ICommandHandler<CreateLeaveEncashmentCommand, CreateLeaveEncashmentResult>
{
    public async Task<GetLeaveTypesResult> Handle(GetLeaveTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await leaveDbContext.LeaveTypes.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
        return new GetLeaveTypesResult(types.Select(ToDto).ToList());
    }

    public async Task<UpsertLeaveTypeResult> Handle(UpsertLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var type = request.LeaveType.Id.HasValue
            ? await leaveDbContext.LeaveTypes.FirstOrDefaultAsync(x => x.Id == request.LeaveType.Id.Value, cancellationToken)
                ?? throw new NotFoundException("LeaveType", request.LeaveType.Id.Value)
            : null;

        if (type is null)
        {
            type = LeaveType.Create(Guid.NewGuid(), request.LeaveType, request.UserId);
            await leaveDbContext.LeaveTypes.AddAsync(type, cancellationToken);
        }
        else
        {
            type.Update(request.LeaveType, request.UserId);
        }

        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new UpsertLeaveTypeResult(ToDto(type));
    }

    public async Task<DeleteLeaveTypeResult> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await leaveDbContext.LeaveTypes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("LeaveType", request.Id);
        type.Remove(request.UserId);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new DeleteLeaveTypeResult(true);
    }

    public async Task<GetLeavePeriodsResult> Handle(GetLeavePeriodsQuery request, CancellationToken cancellationToken)
    {
        var periods = await leaveDbContext.LeavePeriods.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);
        return new GetLeavePeriodsResult(periods.Select(ToDto).ToList());
    }

    public async Task<UpsertLeavePeriodResult> Handle(UpsertLeavePeriodCommand request, CancellationToken cancellationToken)
    {
        var period = request.LeavePeriod.Id.HasValue
            ? await leaveDbContext.LeavePeriods.FirstOrDefaultAsync(x => x.Id == request.LeavePeriod.Id.Value, cancellationToken)
                ?? throw new NotFoundException("LeavePeriod", request.LeavePeriod.Id.Value)
            : null;

        if (period is null)
        {
            period = LeavePeriod.Create(Guid.NewGuid(), request.LeavePeriod, request.UserId);
            await leaveDbContext.LeavePeriods.AddAsync(period, cancellationToken);
        }
        else
        {
            period.Update(request.LeavePeriod, request.UserId);
        }

        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new UpsertLeavePeriodResult(ToDto(period));
    }

    public async Task<DeleteLeavePeriodResult> Handle(DeleteLeavePeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await leaveDbContext.LeavePeriods.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("LeavePeriod", request.Id);
        period.Remove(request.UserId);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new DeleteLeavePeriodResult(true);
    }

    public async Task<GetLeavePoliciesResult> Handle(GetLeavePoliciesQuery request, CancellationToken cancellationToken)
    {
        var policies = await leaveDbContext.LeavePolicies.AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderBy(x => x.NameEng)
            .ToListAsync(cancellationToken);
        var types = await GetTypeMapAsync(request.CompanyId, cancellationToken);
        return new GetLeavePoliciesResult(policies.Select(x => ToDto(x, types)).ToList());
    }

    public async Task<UpsertLeavePolicyResult> Handle(UpsertLeavePolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = request.LeavePolicy.Id.HasValue
            ? await leaveDbContext.LeavePolicies.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == request.LeavePolicy.Id.Value, cancellationToken)
                ?? throw new NotFoundException("LeavePolicy", request.LeavePolicy.Id.Value)
            : null;

        if (policy is null)
        {
            policy = LeavePolicy.Create(Guid.NewGuid(), request.LeavePolicy, request.UserId);
            await leaveDbContext.LeavePolicies.AddAsync(policy, cancellationToken);
        }
        else
        {
            policy.Update(request.LeavePolicy, request.UserId);
        }

        await leaveDbContext.SaveChangesAsync(cancellationToken);
        var types = await GetTypeMapAsync(policy.CompanyId, cancellationToken);
        return new UpsertLeavePolicyResult(ToDto(policy, types));
    }

    public async Task<DeleteLeavePolicyResult> Handle(DeleteLeavePolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await leaveDbContext.LeavePolicies.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("LeavePolicy", request.Id);
        policy.Remove(request.UserId);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new DeleteLeavePolicyResult(true);
    }

    public async Task<GetLeavePolicyAssignmentsResult> Handle(GetLeavePolicyAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await leaveDbContext.LeavePolicyAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderByDescending(x => x.EffectiveFrom)
            .ToListAsync(cancellationToken);
        var policies = await leaveDbContext.LeavePolicies.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        return new GetLeavePolicyAssignmentsResult(assignments.Select(x => ToDto(x, policies)).ToList());
    }

    public async Task<UpsertLeavePolicyAssignmentResult> Handle(UpsertLeavePolicyAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = request.Assignment.Id.HasValue
            ? await leaveDbContext.LeavePolicyAssignments.FirstOrDefaultAsync(x => x.Id == request.Assignment.Id.Value, cancellationToken)
                ?? throw new NotFoundException("LeavePolicyAssignment", request.Assignment.Id.Value)
            : null;

        if (assignment is null)
        {
            assignment = LeavePolicyAssignment.Create(Guid.NewGuid(), request.Assignment, request.UserId);
            await leaveDbContext.LeavePolicyAssignments.AddAsync(assignment, cancellationToken);
        }
        else
        {
            assignment.Update(request.Assignment, request.UserId);
        }

        await leaveDbContext.SaveChangesAsync(cancellationToken);
        var policies = await leaveDbContext.LeavePolicies.AsNoTracking()
            .Where(x => x.CompanyId == assignment.CompanyId)
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        return new UpsertLeavePolicyAssignmentResult(ToDto(assignment, policies));
    }

    public async Task<DeleteLeavePolicyAssignmentResult> Handle(DeleteLeavePolicyAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await leaveDbContext.LeavePolicyAssignments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("LeavePolicyAssignment", request.Id);
        assignment.Remove(request.UserId);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new DeleteLeavePolicyAssignmentResult(true);
    }

    public async Task<GenerateLeaveAllocationsResult> Handle(GenerateLeaveAllocationsCommand request, CancellationToken cancellationToken)
    {
        if (!request.Request.EmployeeId.HasValue)
        {
            throw new BadRequestException("Employee is required when generating leave allocations.");
        }

        var period = await leaveDbContext.LeavePeriods.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Request.LeavePeriodId && x.CompanyId == request.Request.CompanyId, cancellationToken)
            ?? throw new NotFoundException("LeavePeriod", request.Request.LeavePeriodId);
        var assignments = await leaveDbContext.LeavePolicyAssignments.AsNoTracking()
            .Where(x => x.CompanyId == request.Request.CompanyId
                && x.EffectiveFrom <= period.EndDate
                && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= period.StartDate)
                && (request.Request.EmployeeId == null || x.EmployeeId == request.Request.EmployeeId || x.Target == LeavePolicyAssignmentTarget.Company))
            .ToListAsync(cancellationToken);
        var policyIds = assignments.Select(x => x.PolicyId).Distinct().ToList();
        var policies = await leaveDbContext.LeavePolicies.Include(x => x.Lines)
            .Where(x => policyIds.Contains(x.Id) && x.IsActive)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var created = 0;
        foreach (var assignment in assignments)
        {
            if (!policies.TryGetValue(assignment.PolicyId, out var policy))
            {
                continue;
            }

            var employeeIds = assignment.Target == LeavePolicyAssignmentTarget.Employee && assignment.EmployeeId.HasValue
                ? [assignment.EmployeeId.Value]
                : request.Request.EmployeeId.HasValue ? [request.Request.EmployeeId.Value] : Array.Empty<Guid>();

            foreach (var employeeId in employeeIds)
            {
                foreach (var line in policy.Lines)
                {
                    var exists = await leaveDbContext.LeaveLedgerEntries.AnyAsync(x => x.CompanyId == request.Request.CompanyId
                        && x.EmployeeId == employeeId
                        && x.LeaveTypeId == line.LeaveTypeId
                        && x.LeavePeriodId == period.Id
                        && x.EntryType == LeaveLedgerEntryType.Allocation, cancellationToken);
                    if (exists || line.AnnualAllocationDays <= 0)
                    {
                        continue;
                    }

                    var balanceAfter = await GetCurrentBalanceAsync(request.Request.CompanyId, employeeId, line.LeaveTypeId, cancellationToken) + line.AnnualAllocationDays;
                    await leaveDbContext.LeaveLedgerEntries.AddAsync(LeaveLedgerEntry.Create(
                        Guid.NewGuid(),
                        request.Request.CompanyId,
                        employeeId,
                        line.LeaveTypeId,
                        period.Id,
                        policy.Id,
                        LeaveLedgerEntryType.Allocation,
                        period.StartDate,
                        line.AnnualAllocationDays,
                        balanceAfter,
                        "Policy allocation",
                        request.UserId), cancellationToken);
                    created++;
                }
            }
        }

        await leaveDbContext.SaveChangesAsync(cancellationToken);
        return new GenerateLeaveAllocationsResult(created);
    }

    public async Task<GetLeaveApplicationsResult> Handle(GetLeaveApplicationsQuery request, CancellationToken cancellationToken)
    {
        var query = leaveDbContext.LeaveApplications.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);
        if (request.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        var applications = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        var types = await GetTypeMapAsync(request.CompanyId, cancellationToken);
        return new GetLeaveApplicationsResult(applications.Select(x => ToDto(x, types)).ToList());
    }

    public async Task<UpsertLeaveApplicationResult> Handle(UpsertLeaveApplicationCommand request, CancellationToken cancellationToken)
    {
        var leaveType = await leaveDbContext.LeaveTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Application.LeaveTypeId && x.CompanyId == request.Application.CompanyId, cancellationToken)
            ?? throw new NotFoundException("LeaveType", request.Application.LeaveTypeId);
        if (leaveType.RequiresAttachment && string.IsNullOrWhiteSpace(request.Application.AttachmentPath))
        {
            throw new BadRequestException("Attachment is required for this leave type.");
        }

        var totalDays = await CountWorkingLeaveDaysAsync(request.Application.CompanyId, request.Application.StartDate, request.Application.EndDate, cancellationToken);
        var application = request.Application.Id.HasValue
            ? await leaveDbContext.LeaveApplications.FirstOrDefaultAsync(x => x.Id == request.Application.Id.Value, cancellationToken)
                ?? throw new NotFoundException("LeaveApplication", request.Application.Id.Value)
            : null;

        if (application is null)
        {
            application = LeaveApplication.Create(Guid.NewGuid(), request.Application, totalDays, request.UserId);
            await leaveDbContext.LeaveApplications.AddAsync(application, cancellationToken);
        }
        else
        {
            application.UpdateDraft(request.Application, totalDays, request.UserId);
        }

        await leaveDbContext.SaveChangesAsync(cancellationToken);
        var types = await GetTypeMapAsync(application.CompanyId, cancellationToken);
        return new UpsertLeaveApplicationResult(ToDto(application, types));
    }

    public async Task<LeaveApplicationActionResult> Handle(SubmitLeaveApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await leaveDbContext.LeaveApplications.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("LeaveApplication", request.Id);
        application.Submit(request.UserId);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        var types = await GetTypeMapAsync(application.CompanyId, cancellationToken);
        return new LeaveApplicationActionResult(ToDto(application, types));
    }

    public async Task<LeaveApplicationActionResult> Handle(ReviewLeaveApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await leaveDbContext.LeaveApplications.FirstOrDefaultAsync(x => x.Id == request.Review.ApplicationId, cancellationToken)
            ?? throw new NotFoundException("LeaveApplication", request.Review.ApplicationId);

        if (request.Review.IsApproved)
        {
            var leaveType = await leaveDbContext.LeaveTypes.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == application.LeaveTypeId && x.CompanyId == application.CompanyId, cancellationToken)
                ?? throw new NotFoundException("LeaveType", application.LeaveTypeId);
            var usageDays = -application.TotalDays;
            var currentBalance = await GetCurrentBalanceAsync(application.CompanyId, application.EmployeeId, application.LeaveTypeId, cancellationToken);
            var balanceAfter = currentBalance + usageDays;
            if (!leaveType.AllowNegativeBalance && balanceAfter < 0)
            {
                throw new BadRequestException("Employee does not have enough leave balance.");
            }

            if (leaveType.AllowNegativeBalance && balanceAfter < -leaveType.NegativeBalanceLimit)
            {
                throw new BadRequestException("Leave request exceeds the negative balance limit.");
            }

            application.Approve(request.UserId, request.Review.ApproverComment);
            await leaveDbContext.LeaveLedgerEntries.AddAsync(LeaveLedgerEntry.Create(
                Guid.NewGuid(),
                application.CompanyId,
                application.EmployeeId,
                application.LeaveTypeId,
                null,
                application.Id,
                LeaveLedgerEntryType.Application,
                application.StartDate,
                usageDays,
                balanceAfter,
                application.Reason,
                request.UserId), cancellationToken);
        }
        else
        {
            application.Reject(request.UserId, request.Review.ApproverComment);
        }

        await leaveDbContext.SaveChangesAsync(cancellationToken);
        var types = await GetTypeMapAsync(application.CompanyId, cancellationToken);
        return new LeaveApplicationActionResult(ToDto(application, types));
    }

    public async Task<LeaveApplicationActionResult> Handle(CancelLeaveApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await leaveDbContext.LeaveApplications.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("LeaveApplication", request.Id);
        application.Cancel(request.UserId);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        var types = await GetTypeMapAsync(application.CompanyId, cancellationToken);
        return new LeaveApplicationActionResult(ToDto(application, types));
    }

    public async Task<GetLeaveLedgerEntriesResult> Handle(GetLeaveLedgerEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = leaveDbContext.LeaveLedgerEntries.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId);
        if (request.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        if (request.LeaveTypeId.HasValue)
        {
            query = query.Where(x => x.LeaveTypeId == request.LeaveTypeId.Value);
        }

        if (request.LeavePeriodId.HasValue)
        {
            query = query.Where(x => x.LeavePeriodId == request.LeavePeriodId.Value);
        }

        var entries = await query.OrderByDescending(x => x.PostingDate).ThenByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        var types = await GetTypeMapAsync(request.CompanyId, cancellationToken);
        return new GetLeaveLedgerEntriesResult(entries.Select(x => ToDto(x, types)).ToList());
    }

    public async Task<CreateLeaveLedgerAdjustmentResult> Handle(CreateLeaveLedgerAdjustmentCommand request, CancellationToken cancellationToken)
    {
        var balanceAfter = await GetCurrentBalanceAsync(request.Adjustment.CompanyId, request.Adjustment.EmployeeId, request.Adjustment.LeaveTypeId, cancellationToken) + request.Adjustment.Days;
        var entry = LeaveLedgerEntry.Create(
            Guid.NewGuid(),
            request.Adjustment.CompanyId,
            request.Adjustment.EmployeeId,
            request.Adjustment.LeaveTypeId,
            request.Adjustment.LeavePeriodId,
            null,
            LeaveLedgerEntryType.Adjustment,
            request.Adjustment.PostingDate,
            request.Adjustment.Days,
            balanceAfter,
            request.Adjustment.Notes,
            request.UserId);
        await leaveDbContext.LeaveLedgerEntries.AddAsync(entry, cancellationToken);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        var types = await GetTypeMapAsync(entry.CompanyId, cancellationToken);
        return new CreateLeaveLedgerAdjustmentResult(ToDto(entry, types));
    }

    public async Task<CreateLeaveEncashmentResult> Handle(CreateLeaveEncashmentCommand request, CancellationToken cancellationToken)
    {
        var balanceAfter = await GetCurrentBalanceAsync(request.Encashment.CompanyId, request.Encashment.EmployeeId, request.Encashment.LeaveTypeId, cancellationToken) - request.Encashment.Days;
        if (balanceAfter < 0)
        {
            throw new BadRequestException("Employee does not have enough leave balance for encashment.");
        }

        var encashment = LeaveEncashmentRequest.Create(Guid.NewGuid(), request.Encashment, request.UserId);
        await leaveDbContext.LeaveEncashmentRequests.AddAsync(encashment, cancellationToken);
        var entry = LeaveLedgerEntry.Create(
            Guid.NewGuid(),
            request.Encashment.CompanyId,
            request.Encashment.EmployeeId,
            request.Encashment.LeaveTypeId,
            null,
            encashment.Id,
            LeaveLedgerEntryType.Encashment,
            DateTime.Today,
            -request.Encashment.Days,
            balanceAfter,
            "Leave encashment",
            request.UserId);
        await leaveDbContext.LeaveLedgerEntries.AddAsync(entry, cancellationToken);
        await leaveDbContext.SaveChangesAsync(cancellationToken);
        var types = await GetTypeMapAsync(entry.CompanyId, cancellationToken);
        return new CreateLeaveEncashmentResult(ToDto(encashment), ToDto(entry, types));
    }

    private async Task<decimal> CountWorkingLeaveDaysAsync(Guid companyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var fromDate = UtcDateTime.Normalize(startDate).Date;
        var toDate = UtcDateTime.Normalize(endDate).Date;
        var configuration = await attendanceDbContext.AttendanceConfigurations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        var weekendDays = configuration?.ToDto().WeekendDays ?? [DayOfWeek.Friday, DayOfWeek.Saturday];
        var holidays = await attendanceDbContext.AttendanceHolidays.AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && x.IsActive
                && !x.IsDeleted
                && (x.EndDate >= fromDate || x.IsRecurringYearly)
                && (x.StartDate <= toDate || x.IsRecurringYearly))
            .ToListAsync(cancellationToken);

        decimal days = 0;
        for (var date = fromDate; date <= toDate; date = date.AddDays(1))
        {
            if (!weekendDays.Contains(date.DayOfWeek) && !holidays.Any(x => HolidayMatchesDate(x, date)))
            {
                days++;
            }
        }

        return days;
    }

    private async Task<decimal> GetCurrentBalanceAsync(Guid companyId, Guid employeeId, Guid leaveTypeId, CancellationToken cancellationToken)
        => await leaveDbContext.LeaveLedgerEntries.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId)
            .SumAsync(x => x.Days, cancellationToken);

    private async Task<Dictionary<Guid, LeaveType>> GetTypeMapAsync(Guid companyId, CancellationToken cancellationToken)
        => await leaveDbContext.LeaveTypes.AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

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

    private static LeaveTypeDto ToDto(LeaveType type) => new()
    {
        Id = type.Id,
        CompanyId = type.CompanyId,
        Code = type.Code,
        Name = type.Name,
        NameEng = type.NameEng,
        IsPaid = type.IsPaid,
        AllowNegativeBalance = type.AllowNegativeBalance,
        NegativeBalanceLimit = type.NegativeBalanceLimit,
        RequiresAttachment = type.RequiresAttachment,
        IsEmergencyLeave = type.IsEmergencyLeave,
        IsActive = type.IsActive
    };

    private static LeavePeriodDto ToDto(LeavePeriod period) => new()
    {
        Id = period.Id,
        CompanyId = period.CompanyId,
        Name = period.Name,
        StartDate = period.StartDate,
        EndDate = period.EndDate,
        IsClosed = period.IsClosed
    };

    private static LeavePolicyDto ToDto(LeavePolicy policy, Dictionary<Guid, LeaveType> types) => new()
    {
        Id = policy.Id,
        CompanyId = policy.CompanyId,
        Name = policy.Name,
        NameEng = policy.NameEng,
        IsActive = policy.IsActive,
        Lines = policy.Lines.Select(line =>
        {
            types.TryGetValue(line.LeaveTypeId, out var type);
            return new LeavePolicyLineDto
            {
                Id = line.Id,
                LeaveTypeId = line.LeaveTypeId,
                LeaveTypeName = type?.Name,
                LeaveTypeNameEng = type?.NameEng,
                AnnualAllocationDays = line.AnnualAllocationDays,
                AccruesMonthly = line.AccruesMonthly,
                AllowCarryForward = line.AllowCarryForward,
                MaxCarryForwardDays = line.MaxCarryForwardDays
            };
        }).ToList()
    };

    private static LeavePolicyAssignmentDto ToDto(LeavePolicyAssignment assignment, Dictionary<Guid, LeavePolicy> policies)
    {
        policies.TryGetValue(assignment.PolicyId, out var policy);
        return new LeavePolicyAssignmentDto
        {
            Id = assignment.Id,
            CompanyId = assignment.CompanyId,
            PolicyId = assignment.PolicyId,
            PolicyName = policy?.Name,
            PolicyNameEng = policy?.NameEng,
            Target = assignment.Target,
            EmployeeId = assignment.EmployeeId,
            DepartmentId = assignment.DepartmentId,
            EffectiveFrom = assignment.EffectiveFrom,
            EffectiveTo = assignment.EffectiveTo
        };
    }

    private static LeaveApplicationDto ToDto(LeaveApplication application, Dictionary<Guid, LeaveType> types)
    {
        types.TryGetValue(application.LeaveTypeId, out var type);
        return new LeaveApplicationDto
        {
            Id = application.Id,
            CompanyId = application.CompanyId,
            EmployeeId = application.EmployeeId,
            LeaveTypeId = application.LeaveTypeId,
            LeaveTypeName = type?.Name,
            LeaveTypeNameEng = type?.NameEng,
            StartDate = application.StartDate,
            EndDate = application.EndDate,
            TotalDays = application.TotalDays,
            Status = application.Status,
            Reason = application.Reason,
            AttachmentPath = application.AttachmentPath,
            ApproverUserId = application.ApproverUserId,
            ApprovalDateUtc = application.ApprovalDateUtc,
            ApproverComment = application.ApproverComment,
            CreatedAt = application.CreatedAt
        };
    }

    private static LeaveLedgerEntryDto ToDto(LeaveLedgerEntry entry, Dictionary<Guid, LeaveType> types)
    {
        types.TryGetValue(entry.LeaveTypeId, out var type);
        return new LeaveLedgerEntryDto
        {
            Id = entry.Id,
            CompanyId = entry.CompanyId,
            EmployeeId = entry.EmployeeId,
            LeaveTypeId = entry.LeaveTypeId,
            LeaveTypeName = type?.Name,
            LeaveTypeNameEng = type?.NameEng,
            LeavePeriodId = entry.LeavePeriodId,
            SourceDocumentId = entry.SourceDocumentId,
            EntryType = entry.EntryType,
            PostingDate = entry.PostingDate,
            Days = entry.Days,
            BalanceAfter = entry.BalanceAfter,
            Notes = entry.Notes
        };
    }

    private static LeaveEncashmentDto ToDto(LeaveEncashmentRequest encashment) => new()
    {
        Id = encashment.Id,
        CompanyId = encashment.CompanyId,
        EmployeeId = encashment.EmployeeId,
        LeaveTypeId = encashment.LeaveTypeId,
        Days = encashment.Days,
        Amount = encashment.Amount,
        Status = encashment.Status
    };
}
