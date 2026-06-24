namespace Payroll.Salaries.Features.Enhancements;

public record ListSalaryStructuresQuery(Guid CompanyId) : IQuery<ListSalaryStructuresResult>;
public record ListSalaryStructuresResult(List<SalaryStructureDto> StructureList);
public record UpsertSalaryStructureCommand(SalaryStructureUpsertDto Structure) : ICommand<PayrollActionResultDto>;
public record SetSalaryStructureStatusCommand(Guid Id, bool IsActive) : ICommand<PayrollActionResultDto>;

public record ListSalaryStructureAssignmentsQuery(Guid CompanyId, Guid? EmployeeId) : IQuery<ListSalaryStructureAssignmentsResult>;
public record ListSalaryStructureAssignmentsResult(List<SalaryStructureAssignmentDto> AssignmentList);
public record UpsertSalaryStructureAssignmentCommand(SalaryStructureAssignmentUpsertDto Assignment) : ICommand<PayrollActionResultDto>;
public record EndSalaryStructureAssignmentCommand(Guid Id, DateTime EffectiveTo) : ICommand<PayrollActionResultDto>;

public record ListPayrollPeriodsQuery(Guid CompanyId) : IQuery<ListPayrollPeriodsResult>;
public record ListPayrollPeriodsResult(List<PayrollPeriodDto> PeriodList);
public record UpsertPayrollPeriodCommand(PayrollPeriodUpsertDto Period) : ICommand<PayrollActionResultDto>;
public record SetPayrollPeriodStatusCommand(Guid Id, PayrollPeriodStatus Status) : ICommand<PayrollActionResultDto>;

public record ListPayrollEntriesQuery(Guid CompanyId, Guid? PayrollPeriodId) : IQuery<ListPayrollEntriesResult>;
public record ListPayrollEntriesResult(List<PayrollEntryDto> EntryList);
public record CreatePayrollEntryCommand(PayrollEntryCreateDto Entry) : ICommand<PayrollActionResultDto>;
public record PayrollEntryActionCommand(Guid Id, string Action) : ICommand<PayrollActionResultDto>;

public record ListPayslipsQuery(Guid CompanyId, Guid? PayrollEntryId, Guid? EmployeeId) : IQuery<ListPayslipsResult>;
public record ListPayslipsResult(List<PayslipDto> PayslipList);
public record GetPayslipQuery(Guid Id) : IQuery<GetPayslipResult>;
public record GetPayslipResult(PayslipDto Payslip);
public record PayslipActionCommand(Guid Id, string Action) : ICommand<PayrollActionResultDto>;

public record ListPayrollInputsQuery(Guid CompanyId, Guid? PayrollPeriodId, Guid? EmployeeId) : IQuery<ListPayrollInputsResult>;
public record ListPayrollInputsResult(List<PayrollInputDto> InputList);
public record UpsertPayrollInputCommand(PayrollInputUpsertDto Input) : ICommand<PayrollActionResultDto>;
public record DeletePayrollInputCommand(Guid Id) : ICommand<PayrollActionResultDto>;

internal static class PayrollEnhancementMapper
{
    public static SalaryStructureDto ToDto(SalaryStructure structure, IReadOnlyDictionary<Guid, Component>? components = null) => new()
    {
        Id = structure.Id,
        CompanyId = structure.CompanyId,
        Name = structure.Name,
        NameEng = structure.NameEng,
        Description = structure.Description,
        IsActive = structure.IsActive,
        CreatedAt = structure.CreatedAt ?? DateTime.MinValue,
        StatusLabel = structure.IsActive ? "Active" : "Inactive",
        Lines = structure.Lines
            .OrderBy(x => x.DisplayOrder)
            .Select(x => ToLineDto(x, components))
            .ToList()
    };

    public static SalaryStructureLineDto ToLineDto(SalaryStructureLine line, IReadOnlyDictionary<Guid, Component>? components = null)
    {
        Component? component = null;
        components?.TryGetValue(line.ComponentId, out component);
        return new SalaryStructureLineDto
        {
            Id = line.Id,
            SalaryStructureId = line.SalaryStructureId,
            ComponentId = line.ComponentId,
            ComponentName = component?.Name,
            ComponentNameEng = component?.NameEng,
            ComponentType = line.ComponentType,
            Amount = line.Amount,
            IsRecurring = line.IsRecurring,
            DisplayOrder = line.DisplayOrder
        };
    }

    public static PayrollPeriodDto ToDto(PayrollPeriod period) => new()
    {
        Id = period.Id,
        CompanyId = period.CompanyId,
        Month = period.Month,
        Year = period.Year,
        StartDate = period.StartDate,
        EndDate = period.EndDate,
        Status = period.Status,
        IsClosed = period.IsClosed,
        PeriodName = $"{period.Year}-{period.Month:00}",
        StatusLabel = period.Status.ToString()
    };

    public static PayrollEntryDto ToDto(PayrollEntry entry, PayrollPeriod? period = null) => new()
    {
        Id = entry.Id,
        CompanyId = entry.CompanyId,
        PayrollPeriodId = entry.PayrollPeriodId,
        PeriodName = period is null ? null : $"{period.Year}-{period.Month:00}",
        Status = entry.Status,
        StatusLabel = entry.Status.ToString(),
        EmployeeCount = entry.EmployeeCount,
        GrossAmount = entry.GrossAmount,
        DeductionAmount = entry.DeductionAmount,
        NetAmount = entry.NetAmount,
        CreatedAt = entry.CreatedAt ?? DateTime.MinValue,
        IsPostedToAccounting = entry.IsPostedToAccounting,
        AccountingJournalEntryId = entry.AccountingJournalEntryId,
        AccountingJournalNumber = entry.AccountingJournalNumber,
        AccountingPostedAt = entry.AccountingPostedAt
    };

    public static PayslipDto ToDto(Payslip payslip, PayrollPeriod? period = null, bool includeLines = false) => new()
    {
        Id = payslip.Id,
        CompanyId = payslip.CompanyId,
        EmployeeId = payslip.EmployeeId,
        EmployeeName = payslip.EmployeeId.ToString("N")[..8],
        PayrollEntryId = payslip.PayrollEntryId,
        PayrollPeriodId = payslip.PayrollPeriodId,
        Month = period?.Month ?? payslip.Month,
        Year = period?.Year ?? payslip.Year,
        Status = payslip.Status,
        StatusLabel = payslip.Status.ToString(),
        BasicAmount = payslip.BasicAmount,
        TotalAllowances = payslip.TotalAllowances,
        TotalBenefits = payslip.TotalBenefits,
        TotalInputs = payslip.TotalInputs,
        TotalDeductions = payslip.TotalDeductions,
        TotalLoans = payslip.TotalLoans,
        GrossAmount = payslip.GrossAmount,
        NetAmount = payslip.NetAmount,
        ApprovedAt = payslip.ApprovedAt,
        PaidAt = payslip.PaidAt,
        IsWpsEligible = payslip.Status is PayslipStatus.Paid or PayslipStatus.Closed,
        Lines = includeLines
            ? payslip.Lines.Select(x => new PayslipLineDto
            {
                Id = x.Id,
                PayslipId = x.PayslipId,
                ComponentId = x.ComponentId,
                Name = x.Name,
                NameEng = x.NameEng,
                InputType = x.InputType,
                Amount = x.Amount,
                IsDeduction = x.IsDeduction,
                SourceType = x.SourceType,
                SourceDocumentId = x.SourceDocumentId
            }).ToList()
            : []
    };
}

public class ListSalaryStructuresHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListSalaryStructuresQuery, ListSalaryStructuresResult>
{
    public async Task<ListSalaryStructuresResult> Handle(ListSalaryStructuresQuery request, CancellationToken cancellationToken)
    {
        var structures = await dbContext.SalaryStructures
            .AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var componentIds = structures.SelectMany(x => x.Lines.Select(l => l.ComponentId)).Distinct().ToList();
        var components = await dbContext.Components
            .AsNoTracking()
            .Where(x => componentIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        return new ListSalaryStructuresResult(structures.Select(x => PayrollEnhancementMapper.ToDto(x, components)).ToList());
    }
}

public class UpsertSalaryStructureHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertSalaryStructureCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(UpsertSalaryStructureCommand request, CancellationToken cancellationToken)
    {
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var dto = request.Structure;
        var structure = dto.Id == Guid.Empty
            ? new SalaryStructure { Id = Guid.NewGuid(), CompanyId = dto.CompanyId, CreatedAt = DateTime.UtcNow, CreatedBy = userId }
            : await dbContext.SalaryStructures.Include(x => x.Lines).FirstAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);

        structure.Name = dto.Name;
        structure.NameEng = dto.NameEng;
        structure.Description = dto.Description;
        structure.ModifiedAt = DateTime.UtcNow;
        structure.ModifiedBy = userId;

        await dbContext.SalaryStructureLines.Where(x => x.SalaryStructureId == structure.Id).ExecuteDeleteAsync(cancellationToken);
        structure.SetLines(dto.Lines.Select((line, index) => new SalaryStructureLine
        {
            Id = Guid.NewGuid(),
            SalaryStructureId = structure.Id,
            ComponentId = line.ComponentId,
            ComponentType = line.ComponentType,
            Amount = line.Amount,
            IsRecurring = line.IsRecurring,
            DisplayOrder = line.DisplayOrder == 0 ? index + 1 : line.DisplayOrder
        }));

        if (dto.Id == Guid.Empty)
        {
            await dbContext.SalaryStructures.AddAsync(structure, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(structure.Id, "Saved", "Salary structure saved successfully");
    }
}

public class SetSalaryStructureStatusHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<SetSalaryStructureStatusCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(SetSalaryStructureStatusCommand request, CancellationToken cancellationToken)
    {
        var structure = await dbContext.SalaryStructures.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        structure.IsActive = request.IsActive;
        structure.ModifiedAt = DateTime.UtcNow;
        structure.ModifiedBy = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(structure.Id, structure.IsActive ? "Active" : "Inactive", "Salary structure status updated");
    }
}

public class ListSalaryStructureAssignmentsHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListSalaryStructureAssignmentsQuery, ListSalaryStructureAssignmentsResult>
{
    public async Task<ListSalaryStructureAssignmentsResult> Handle(ListSalaryStructureAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalaryStructureAssignments.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.EmployeeId.HasValue && request.EmployeeId.Value != Guid.Empty)
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

        var assignments = await query.OrderByDescending(x => x.EffectiveFrom).ToListAsync(cancellationToken);
        var structureIds = assignments.Select(x => x.SalaryStructureId).Distinct().ToList();
        var structures = await dbContext.SalaryStructures.AsNoTracking().Where(x => structureIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

        return new ListSalaryStructureAssignmentsResult(assignments.Select(x =>
        {
            structures.TryGetValue(x.SalaryStructureId, out var structure);
            return new SalaryStructureAssignmentDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeId.ToString("N")[..8],
                SalaryStructureId = x.SalaryStructureId,
                SalaryStructureName = structure?.Name,
                SalaryStructureNameEng = structure?.NameEng,
                EffectiveFrom = x.EffectiveFrom,
                EffectiveTo = x.EffectiveTo,
                IsActive = x.IsActive,
                StatusLabel = x.IsActive ? "Active" : "Ended"
            };
        }).ToList());
    }
}

public class UpsertSalaryStructureAssignmentHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertSalaryStructureAssignmentCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(UpsertSalaryStructureAssignmentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Assignment;
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var assignment = dto.Id == Guid.Empty
            ? new SalaryStructureAssignment { Id = Guid.NewGuid(), CompanyId = dto.CompanyId, CreatedAt = DateTime.UtcNow, CreatedBy = userId }
            : await dbContext.SalaryStructureAssignments.FirstAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);

        if (dto.Id == Guid.Empty)
        {
            var existing = await dbContext.SalaryStructureAssignments
                .Where(x => x.CompanyId == dto.CompanyId && x.EmployeeId == dto.EmployeeId && x.IsActive && !x.IsDeleted)
                .ToListAsync(cancellationToken);
            foreach (var item in existing)
            {
                item.IsActive = false;
                item.EffectiveTo = dto.EffectiveFrom.AddDays(-1);
                item.ModifiedAt = DateTime.UtcNow;
                item.ModifiedBy = userId;
            }
        }

        assignment.EmployeeId = dto.EmployeeId;
        assignment.SalaryStructureId = dto.SalaryStructureId;
        assignment.EffectiveFrom = dto.EffectiveFrom;
        assignment.EffectiveTo = dto.EffectiveTo;
        assignment.IsActive = !dto.EffectiveTo.HasValue || dto.EffectiveTo.Value.Date >= DateTime.Today;
        assignment.ModifiedAt = DateTime.UtcNow;
        assignment.ModifiedBy = userId;

        if (dto.Id == Guid.Empty)
            await dbContext.SalaryStructureAssignments.AddAsync(assignment, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(assignment.Id, assignment.IsActive ? "Active" : "Ended", "Salary structure assignment saved");
    }
}

public class EndSalaryStructureAssignmentHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<EndSalaryStructureAssignmentCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(EndSalaryStructureAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await dbContext.SalaryStructureAssignments.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        assignment.IsActive = false;
        assignment.EffectiveTo = request.EffectiveTo;
        assignment.ModifiedAt = DateTime.UtcNow;
        assignment.ModifiedBy = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(assignment.Id, "Ended", "Assignment ended");
    }
}

public class ListPayrollPeriodsHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListPayrollPeriodsQuery, ListPayrollPeriodsResult>
{
    public async Task<ListPayrollPeriodsResult> Handle(ListPayrollPeriodsQuery request, CancellationToken cancellationToken)
    {
        var periods = await dbContext.PayrollPeriods.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderByDescending(x => x.Year).ThenByDescending(x => x.Month)
            .ToListAsync(cancellationToken);
        return new ListPayrollPeriodsResult(periods.Select(PayrollEnhancementMapper.ToDto).ToList());
    }
}

public class UpsertPayrollPeriodHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertPayrollPeriodCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(UpsertPayrollPeriodCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Period;
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var period = dto.Id == Guid.Empty
            ? new PayrollPeriod { Id = Guid.NewGuid(), CompanyId = dto.CompanyId, CreatedAt = DateTime.UtcNow, CreatedBy = userId }
            : await dbContext.PayrollPeriods.FirstAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);

        period.Month = dto.Month;
        period.Year = dto.Year;
        period.StartDate = dto.StartDate;
        period.EndDate = dto.EndDate;
        period.ModifiedAt = DateTime.UtcNow;
        period.ModifiedBy = userId;

        if (dto.Id == Guid.Empty)
            await dbContext.PayrollPeriods.AddAsync(period, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(period.Id, period.Status.ToString(), "Payroll period saved");
    }
}

public class SetPayrollPeriodStatusHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<SetPayrollPeriodStatusCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(SetPayrollPeriodStatusCommand request, CancellationToken cancellationToken)
    {
        var period = await dbContext.PayrollPeriods.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        period.Status = request.Status;
        period.ModifiedAt = DateTime.UtcNow;
        period.ModifiedBy = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(period.Id, period.Status.ToString(), "Payroll period status updated");
    }
}

public class ListPayrollEntriesHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListPayrollEntriesQuery, ListPayrollEntriesResult>
{
    public async Task<ListPayrollEntriesResult> Handle(ListPayrollEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.PayrollEntries.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.PayrollPeriodId.HasValue && request.PayrollPeriodId.Value != Guid.Empty)
            query = query.Where(x => x.PayrollPeriodId == request.PayrollPeriodId.Value);

        var entries = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        var periodIds = entries.Select(x => x.PayrollPeriodId).Distinct().ToList();
        var periods = await dbContext.PayrollPeriods.AsNoTracking().Where(x => periodIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
        return new ListPayrollEntriesResult(entries.Select(x => PayrollEnhancementMapper.ToDto(x, periods.GetValueOrDefault(x.PayrollPeriodId))).ToList());
    }
}

public class CreatePayrollEntryHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreatePayrollEntryCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(CreatePayrollEntryCommand request, CancellationToken cancellationToken)
    {
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var period = await dbContext.PayrollPeriods.FirstAsync(x => x.Id == request.Entry.PayrollPeriodId && !x.IsDeleted, cancellationToken);
        if (period.IsClosed)
            throw new InvalidOperationException("Closed payroll periods cannot receive new payroll entries");

        var employeeIds = request.Entry.EmployeeIds.Where(x => x != Guid.Empty).Distinct().ToList();
        if (employeeIds.Count == 0)
        {
            employeeIds = await dbContext.SalaryStructureAssignments
                .Where(x => x.CompanyId == request.Entry.CompanyId && x.IsActive && !x.IsDeleted && x.EffectiveFrom <= period.EndDate && (!x.EffectiveTo.HasValue || x.EffectiveTo >= period.StartDate))
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        var entry = new PayrollEntry
        {
            Id = Guid.NewGuid(),
            CompanyId = request.Entry.CompanyId,
            PayrollPeriodId = period.Id,
            Status = PayrollEntryStatus.Draft,
            EmployeeCount = employeeIds.Count,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await dbContext.PayrollEntries.AddAsync(entry, cancellationToken);
        foreach (var employeeId in employeeIds)
        {
            var exists = await dbContext.Payslips.AnyAsync(x => x.CompanyId == entry.CompanyId && x.EmployeeId == employeeId && x.PayrollPeriodId == period.Id && !x.IsDeleted, cancellationToken);
            if (exists)
                continue;

            await dbContext.Payslips.AddAsync(new Payslip
            {
                Id = Guid.NewGuid(),
                CompanyId = entry.CompanyId,
                EmployeeId = employeeId,
                PayrollEntryId = entry.Id,
                PayrollPeriodId = period.Id,
                Month = period.Month,
                Year = period.Year,
                Status = PayslipStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(entry.Id, entry.Status.ToString(), "Payroll entry created");
    }
}

public class PayrollEntryActionHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<PayrollEntryActionCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(PayrollEntryActionCommand request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.PayrollEntries.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);

        switch (request.Action.ToLowerInvariant())
        {
            case "generate":
                await PayrollEnhancementLogic.RecalculateEntryAsync(dbContext, entry, userId, cancellationToken);
                entry.Status = PayrollEntryStatus.Generated;
                break;
            case "approve":
                entry.Status = PayrollEntryStatus.Approved;
                await dbContext.Payslips.Where(x => x.PayrollEntryId == entry.Id && x.Status == PayslipStatus.Calculated).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, PayslipStatus.Approved).SetProperty(x => x.ApprovedAt, DateTime.UtcNow).SetProperty(x => x.ApprovedBy, userId), cancellationToken);
                break;
            case "close":
                entry.Status = PayrollEntryStatus.Closed;
                await dbContext.Payslips.Where(x => x.PayrollEntryId == entry.Id && x.Status == PayslipStatus.Paid).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, PayslipStatus.Closed), cancellationToken);
                break;
            case "reopen":
                entry.Status = PayrollEntryStatus.Generated;
                break;
            case "cancel":
                entry.Status = PayrollEntryStatus.Cancelled;
                await dbContext.Payslips.Where(x => x.PayrollEntryId == entry.Id && x.Status != PayslipStatus.Paid && x.Status != PayslipStatus.Closed).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, PayslipStatus.Cancelled), cancellationToken);
                break;
            default:
                throw new InvalidOperationException("Unsupported payroll entry action");
        }

        entry.ModifiedAt = DateTime.UtcNow;
        entry.ModifiedBy = userId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(entry.Id, entry.Status.ToString(), "Payroll entry updated");
    }
}

public class ListPayslipsHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListPayslipsQuery, ListPayslipsResult>
{
    public async Task<ListPayslipsResult> Handle(ListPayslipsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Payslips.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.PayrollEntryId.HasValue && request.PayrollEntryId.Value != Guid.Empty)
            query = query.Where(x => x.PayrollEntryId == request.PayrollEntryId.Value);
        if (request.EmployeeId.HasValue && request.EmployeeId.Value != Guid.Empty)
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

        var payslips = await query.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToListAsync(cancellationToken);
        var periodIds = payslips.Select(x => x.PayrollPeriodId).Distinct().ToList();
        var periods = await dbContext.PayrollPeriods.AsNoTracking().Where(x => periodIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
        return new ListPayslipsResult(payslips.Select(x => PayrollEnhancementMapper.ToDto(x, periods.GetValueOrDefault(x.PayrollPeriodId))).ToList());
    }
}

public class GetPayslipHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetPayslipQuery, GetPayslipResult>
{
    public async Task<GetPayslipResult> Handle(GetPayslipQuery request, CancellationToken cancellationToken)
    {
        var payslip = await dbContext.Payslips.AsNoTracking().Include(x => x.Lines).FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        var period = await dbContext.PayrollPeriods.AsNoTracking().FirstOrDefaultAsync(x => x.Id == payslip.PayrollPeriodId, cancellationToken);
        return new GetPayslipResult(PayrollEnhancementMapper.ToDto(payslip, period, true));
    }
}

public class PayslipActionHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<PayslipActionCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(PayslipActionCommand request, CancellationToken cancellationToken)
    {
        var payslip = await dbContext.Payslips.Include(x => x.Lines).FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);

        switch (request.Action.ToLowerInvariant())
        {
            case "recalculate":
                if (payslip.Status is PayslipStatus.Approved or PayslipStatus.Paid or PayslipStatus.Closed)
                    throw new InvalidOperationException("Approved, paid, or closed payslips cannot be recalculated");
                await PayrollEnhancementLogic.RecalculatePayslipAsync(dbContext, payslip, userId, cancellationToken);
                payslip.Status = PayslipStatus.Calculated;
                break;
            case "approve":
                if (payslip.Status != PayslipStatus.Calculated)
                    throw new InvalidOperationException("Only calculated payslips can be approved");
                await PayrollEnhancementLogic.PostLoanRepaymentsAsync(dbContext, payslip, userId, cancellationToken);
                payslip.Status = PayslipStatus.Approved;
                payslip.ApprovedAt = DateTime.UtcNow;
                payslip.ApprovedBy = userId;
                break;
            case "paid":
                if (payslip.Status != PayslipStatus.Approved)
                    throw new InvalidOperationException("Only approved payslips can be marked paid");
                payslip.Status = PayslipStatus.Paid;
                payslip.PaidAt = DateTime.UtcNow;
                payslip.PaidBy = userId;
                break;
            case "cancel":
                if (payslip.Status is PayslipStatus.Paid or PayslipStatus.Closed)
                    throw new InvalidOperationException("Paid or closed payslips cannot be cancelled");
                payslip.Status = PayslipStatus.Cancelled;
                break;
            default:
                throw new InvalidOperationException("Unsupported payslip action");
        }

        payslip.ModifiedAt = DateTime.UtcNow;
        payslip.ModifiedBy = userId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(payslip.Id, payslip.Status.ToString(), "Payslip updated");
    }
}

public class ListPayrollInputsHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListPayrollInputsQuery, ListPayrollInputsResult>
{
    public async Task<ListPayrollInputsResult> Handle(ListPayrollInputsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.PayrollInputs.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.PayrollPeriodId.HasValue && request.PayrollPeriodId.Value != Guid.Empty)
            query = query.Where(x => x.PayrollPeriodId == request.PayrollPeriodId.Value);
        if (request.EmployeeId.HasValue && request.EmployeeId.Value != Guid.Empty)
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

        var inputs = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        var periodIds = inputs.Where(x => x.PayrollPeriodId.HasValue).Select(x => x.PayrollPeriodId!.Value).Distinct().ToList();
        var periods = await dbContext.PayrollPeriods.AsNoTracking().Where(x => periodIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

        return new ListPayrollInputsResult(inputs.Select(x => new PayrollInputDto
        {
            Id = x.Id,
            CompanyId = x.CompanyId,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.EmployeeId.ToString("N")[..8],
            PayrollPeriodId = x.PayrollPeriodId,
            PeriodName = x.PayrollPeriodId.HasValue && periods.TryGetValue(x.PayrollPeriodId.Value, out var period) ? $"{period.Year}-{period.Month:00}" : null,
            InputType = x.InputType,
            Amount = x.Amount,
            Notes = x.Notes,
            IsProcessed = x.IsProcessed,
            StatusLabel = x.IsProcessed ? "Processed" : "Open"
        }).ToList());
    }
}

public class UpsertPayrollInputHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertPayrollInputCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(UpsertPayrollInputCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Input;
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var input = dto.Id == Guid.Empty
            ? new PayrollInput { Id = Guid.NewGuid(), CompanyId = dto.CompanyId, CreatedAt = DateTime.UtcNow, CreatedBy = userId }
            : await dbContext.PayrollInputs.FirstAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);

        if (input.IsProcessed)
            throw new InvalidOperationException("Processed payroll inputs cannot be edited");

        input.EmployeeId = dto.EmployeeId;
        input.PayrollPeriodId = dto.PayrollPeriodId;
        input.InputType = dto.InputType;
        input.Amount = dto.Amount;
        input.Notes = dto.Notes;
        input.ModifiedAt = DateTime.UtcNow;
        input.ModifiedBy = userId;

        if (dto.Id == Guid.Empty)
            await dbContext.PayrollInputs.AddAsync(input, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(input.Id, "Open", "Payroll input saved");
    }
}

public class DeletePayrollInputHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeletePayrollInputCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(DeletePayrollInputCommand request, CancellationToken cancellationToken)
    {
        var input = await dbContext.PayrollInputs.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (input.IsProcessed)
            throw new InvalidOperationException("Processed payroll inputs cannot be deleted");

        input.IsDeleted = true;
        input.DeletedAt = DateTime.UtcNow;
        input.DeletedBy = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(input.Id, "Deleted", "Payroll input deleted");
    }
}

internal static partial class PayrollEnhancementLogic
{
    public static async Task RecalculateEntryAsync(PayrollDbContext dbContext, PayrollEntry entry, string userId, CancellationToken cancellationToken)
    {
        var payslips = await dbContext.Payslips.Include(x => x.Lines).Where(x => x.PayrollEntryId == entry.Id && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var payslip in payslips)
            await RecalculatePayslipAsync(dbContext, payslip, userId, cancellationToken);

        entry.EmployeeCount = payslips.Count;
        entry.GrossAmount = payslips.Sum(x => x.GrossAmount);
        entry.DeductionAmount = payslips.Sum(x => x.TotalDeductions + x.TotalLoans);
        entry.NetAmount = payslips.Sum(x => x.NetAmount);
    }

    public static async Task RecalculatePayslipAsync(PayrollDbContext dbContext, Payslip payslip, string userId, CancellationToken cancellationToken)
    {
        var period = await dbContext.PayrollPeriods.FirstAsync(x => x.Id == payslip.PayrollPeriodId && !x.IsDeleted, cancellationToken);
        var assignment = await dbContext.SalaryStructureAssignments
            .Where(x => x.CompanyId == payslip.CompanyId && x.EmployeeId == payslip.EmployeeId && x.IsActive && !x.IsDeleted && x.EffectiveFrom <= period.EndDate && (!x.EffectiveTo.HasValue || x.EffectiveTo >= period.StartDate))
            .OrderByDescending(x => x.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("No active salary structure assignment was found for this employee");

        var structure = await dbContext.SalaryStructures.Include(x => x.Lines).FirstAsync(x => x.Id == assignment.SalaryStructureId && x.IsActive && !x.IsDeleted, cancellationToken);
        var componentIds = structure.Lines.Select(x => x.ComponentId).Distinct().ToList();
        var components = await dbContext.Components.AsNoTracking().Where(x => componentIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
        var inputs = await dbContext.PayrollInputs.Where(x => x.CompanyId == payslip.CompanyId && x.EmployeeId == payslip.EmployeeId && !x.IsDeleted && !x.IsProcessed && (!x.PayrollPeriodId.HasValue || x.PayrollPeriodId == period.Id)).ToListAsync(cancellationToken);
        var loans = await dbContext.EmployeeLoans.Where(x => x.CompanyId == payslip.CompanyId && x.EmployeeId == payslip.EmployeeId && x.Status == EmployeeLoanStatus.Approved && !x.IsDeleted).ToListAsync(cancellationToken);

        var lines = new List<PayslipLine>();
        decimal basic = 0;
        decimal allowances = 0;
        decimal benefits = 0;
        decimal deductions = 0;
        decimal extraInputs = 0;
        decimal loanTotal = 0;

        foreach (var line in structure.Lines.OrderBy(x => x.DisplayOrder))
        {
            components.TryGetValue(line.ComponentId, out var component);
            var isDeduction = line.ComponentType == ComponentType.Deduction;
            if (line.ComponentType == ComponentType.Basic) basic += line.Amount;
            else if (line.ComponentType == ComponentType.Allowance) allowances += line.Amount;
            else if (line.ComponentType == ComponentType.Deduction) deductions += line.Amount;

            lines.Add(NewLine(payslip.Id, line.ComponentId, component?.Name ?? line.ComponentType.ToString(), component?.NameEng, ToInputType(line.ComponentType), line.Amount, isDeduction, "SalaryStructure", line.Id));
        }

        foreach (var input in inputs)
        {
            var isDeduction = input.InputType is PayrollInputType.Deduction or PayrollInputType.LoanRepayment or PayrollInputType.LeaveDeduction;
            if (isDeduction) deductions += input.Amount;
            else if (input.InputType == PayrollInputType.Benefit) benefits += input.Amount;
            else extraInputs += input.Amount;

            lines.Add(NewLine(payslip.Id, null, input.InputType.ToString(), input.InputType.ToString(), input.InputType, input.Amount, isDeduction, "PayrollInput", input.Id));
        }

        var workEntries = await dbContext.PayrollImportedWorkEntries
            .Where(x => x.CompanyId == payslip.CompanyId && x.EmployeeId == payslip.EmployeeId && x.PayrollPeriodId == period.Id && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var workEntry in workEntries)
        {
            var type = workEntry.EntryType.Trim();
            var normalized = type.ToLowerInvariant();
            var amount = workEntry.Amount;
            var isDeduction = normalized.Contains("absence") || normalized.Contains("unpaid") || normalized.Contains("deduction");
            var isEarning = normalized.Contains("overtime") || normalized.Contains("allowance");

            if (isDeduction)
            {
                deductions += amount;
            }
            else if (isEarning)
            {
                extraInputs += amount;
            }
            else
            {
                amount = 0;
            }

            lines.Add(NewLine(
                payslip.Id,
                null,
                string.IsNullOrWhiteSpace(type) ? "Work entry" : type,
                string.IsNullOrWhiteSpace(type) ? "Work entry" : type,
                isDeduction ? PayrollInputType.Deduction : PayrollInputType.Overtime,
                amount,
                isDeduction,
                "PayrollWorkEntry",
                workEntry.Id));
        }

        foreach (var loan in loans)
        {
            var amount = loan.GetInstallmentForPeriod(period.Month, period.Year);
            if (amount <= 0) continue;
            loanTotal += amount;
            lines.Add(NewLine(payslip.Id, null, "Loan repayment", "Loan repayment", PayrollInputType.LoanRepayment, amount, true, "EmployeeLoan", loan.Id));
        }

        await dbContext.PayslipLines.Where(x => x.PayslipId == payslip.Id).ExecuteDeleteAsync(cancellationToken);
        payslip.SetLines(lines);
        payslip.BasicAmount = basic;
        payslip.TotalAllowances = allowances;
        payslip.TotalBenefits = benefits;
        payslip.TotalInputs = extraInputs;
        payslip.TotalDeductions = deductions;
        payslip.TotalLoans = loanTotal;
        payslip.GrossAmount = basic + allowances + benefits + extraInputs;
        payslip.NetAmount = payslip.GrossAmount - deductions - loanTotal;
        payslip.Status = PayslipStatus.Calculated;
        payslip.ModifiedAt = DateTime.UtcNow;
        payslip.ModifiedBy = userId;
    }

    public static async Task PostLoanRepaymentsAsync(PayrollDbContext dbContext, Payslip payslip, string userId, CancellationToken cancellationToken)
    {
        var loanLines = payslip.Lines.Where(x => x.SourceType == "EmployeeLoan" && x.SourceDocumentId.HasValue).ToList();
        if (loanLines.Count == 0) return;
        var loanIds = loanLines.Select(x => x.SourceDocumentId!.Value).Distinct().ToList();
        var loans = await dbContext.EmployeeLoans.Where(x => loanIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
        foreach (var line in loanLines)
        {
            if (line.SourceDocumentId.HasValue && loans.TryGetValue(line.SourceDocumentId.Value, out var loan))
                loan.PostRepayment(line.Amount, userId);
        }
    }

    private static PayslipLine NewLine(Guid payslipId, Guid? componentId, string name, string? nameEng, PayrollInputType inputType, decimal amount, bool isDeduction, string sourceType, Guid sourceDocumentId) => new()
    {
        Id = Guid.NewGuid(),
        PayslipId = payslipId,
        ComponentId = componentId,
        Name = name,
        NameEng = nameEng,
        InputType = inputType,
        Amount = amount,
        IsDeduction = isDeduction,
        SourceType = sourceType,
        SourceDocumentId = sourceDocumentId
    };

    private static PayrollInputType ToInputType(ComponentType componentType) => componentType switch
    {
        ComponentType.Allowance => PayrollInputType.Allowance,
        ComponentType.Deduction => PayrollInputType.Deduction,
        _ => PayrollInputType.Allowance
    };
}

internal static class PayrollEnhancementHelpers
{
    public static string CurrentUser(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static PayrollActionResultDto ActionResult(Guid id, string status, string message) => new()
    {
        Id = id,
        Status = status,
        Message = message,
        IsSuccess = true
    };
}
