using Accounting.Contracts.Accounting.Features;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;

namespace Payroll.Salaries.Features.Enhancements;

public record ListSaudiPayrollInfoQuery(Guid CompanyId) : IQuery<ListSaudiPayrollInfoResult>;
public record ListSaudiPayrollInfoResult(List<SaudiPayrollInfoDto> SaudiPayrollInfoList);
public record GetSaudiPayrollInfoQuery(Guid CompanyId, Guid EmployeeId) : IQuery<GetSaudiPayrollInfoResult>;
public record GetSaudiPayrollInfoResult(SaudiPayrollInfoDto? SaudiPayrollInfo);
public record UpsertSaudiPayrollInfoCommand(SaudiPayrollInfoUpsertDto SaudiPayrollInfo) : ICommand<PayrollActionResultDto>;

public record ListWpsBatchesQuery(Guid CompanyId, Guid? PayrollPeriodId) : IQuery<ListWpsBatchesResult>;
public record ListWpsBatchesResult(List<WpsBatchDto> WpsBatchList);
public record CreateWpsBatchCommand(CreateWpsBatchDto Batch) : ICommand<PayrollActionResultDto>;
public record MarkWpsBatchExportedCommand(Guid Id) : ICommand<PayrollActionResultDto>;

public record ListEosProvisionSnapshotsQuery(Guid CompanyId, Guid? PayrollPeriodId, Guid? EmployeeId) : IQuery<ListEosProvisionSnapshotsResult>;
public record ListEosProvisionSnapshotsResult(List<EosProvisionSnapshotDto> EosSnapshotList);
public record CreateEosProvisionSnapshotCommand(CreateEosProvisionSnapshotDto Snapshot) : ICommand<PayrollActionResultDto>;

public record PostPayrollEntryAccountingCommand(Guid PayrollEntryId) : ICommand<PayrollActionResultDto>;

public record ListImportedPayrollWorkEntriesQuery(Guid CompanyId, Guid? PayrollPeriodId, Guid? EmployeeId) : IQuery<ListImportedPayrollWorkEntriesResult>;
public record ListImportedPayrollWorkEntriesResult(List<PayrollWorkEntryImportDto> WorkEntryList);
public record ImportPayrollWorkEntryCommand(PayrollWorkEntryImportDto WorkEntry) : ICommand<PayrollActionResultDto>;

internal static class PayrollSaudiComplianceMapper
{
    public static SaudiPayrollInfoDto ToDto(SaudiPayrollInfo item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        EmployeeId = item.EmployeeId,
        EmployeeName = item.EmployeeId.ToString("N")[..8],
        Iban = item.Iban,
        BankCode = item.BankCode,
        BankName = item.BankName,
        GosiNumber = item.GosiNumber,
        GosiEmployeePercentage = item.GosiEmployeePercentage,
        GosiEmployerPercentage = item.GosiEmployerPercentage,
        EosBasicSalary = item.EosBasicSalary,
        EosServiceStartDate = item.EosServiceStartDate,
        IncludeInWps = item.IncludeInWps,
        StatusLabel = item.IncludeInWps ? "WPS enabled" : "WPS excluded"
    };

    public static WpsBatchDto ToDto(WpsBatch batch, bool includeRows = false) => new()
    {
        Id = batch.Id,
        CompanyId = batch.CompanyId,
        PayrollPeriodId = batch.PayrollPeriodId,
        PayrollEntryId = batch.PayrollEntryId,
        BatchNumber = batch.BatchNumber,
        Status = batch.Status,
        StatusLabel = batch.Status.ToString(),
        EmployeeCount = batch.EmployeeCount,
        TotalAmount = batch.TotalAmount,
        CreatedAt = batch.CreatedAt ?? DateTime.MinValue,
        ExportedAt = batch.ExportedAt,
        Rows = includeRows ? batch.Rows.Select(x => new WpsBatchRowDto
        {
            Id = x.Id,
            WpsBatchId = x.WpsBatchId,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.EmployeeId.ToString("N")[..8],
            PayslipId = x.PayslipId,
            Iban = x.Iban,
            BankCode = x.BankCode,
            NetAmount = x.NetAmount,
            Remarks = x.Remarks
        }).ToList() : []
    };

    public static EosProvisionSnapshotDto ToDto(EosProvisionSnapshot snapshot) => new()
    {
        Id = snapshot.Id,
        CompanyId = snapshot.CompanyId,
        EmployeeId = snapshot.EmployeeId,
        EmployeeName = snapshot.EmployeeId.ToString("N")[..8],
        PayrollPeriodId = snapshot.PayrollPeriodId,
        ServiceStartDate = snapshot.ServiceStartDate,
        ServiceEndDate = snapshot.ServiceEndDate,
        GrossPayBasis = snapshot.GrossPayBasis,
        ProvisionAmount = snapshot.ProvisionAmount,
        Notes = snapshot.Notes,
        CreatedAt = snapshot.CreatedAt ?? DateTime.MinValue
    };
}

public class ListSaudiPayrollInfoHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListSaudiPayrollInfoQuery, ListSaudiPayrollInfoResult>
{
    public async Task<ListSaudiPayrollInfoResult> Handle(ListSaudiPayrollInfoQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.SaudiPayrollInfos.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderBy(x => x.EmployeeId)
            .ToListAsync(cancellationToken);
        return new(data.Select(PayrollSaudiComplianceMapper.ToDto).ToList());
    }
}

public class GetSaudiPayrollInfoHandler(PayrollDbContext dbContext)
    : IQueryHandler<GetSaudiPayrollInfoQuery, GetSaudiPayrollInfoResult>
{
    public async Task<GetSaudiPayrollInfoResult> Handle(GetSaudiPayrollInfoQuery request, CancellationToken cancellationToken)
    {
        var item = await dbContext.SaudiPayrollInfos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken);
        return new(item is null ? null : PayrollSaudiComplianceMapper.ToDto(item));
    }
}

public class UpsertSaudiPayrollInfoHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertSaudiPayrollInfoCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(UpsertSaudiPayrollInfoCommand request, CancellationToken cancellationToken)
    {
        var dto = request.SaudiPayrollInfo;
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var item = dto.Id == Guid.Empty
            ? await dbContext.SaudiPayrollInfos.FirstOrDefaultAsync(x => x.CompanyId == dto.CompanyId && x.EmployeeId == dto.EmployeeId && !x.IsDeleted, cancellationToken)
            : await dbContext.SaudiPayrollInfos.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);

        if (item is null)
        {
            item = new SaudiPayrollInfo { Id = Guid.NewGuid(), CompanyId = dto.CompanyId, EmployeeId = dto.EmployeeId, CreatedAt = DateTime.UtcNow, CreatedBy = userId };
            await dbContext.SaudiPayrollInfos.AddAsync(item, cancellationToken);
        }

        item.Iban = dto.Iban;
        item.BankCode = dto.BankCode;
        item.BankName = dto.BankName;
        item.GosiNumber = dto.GosiNumber;
        item.GosiEmployeePercentage = dto.GosiEmployeePercentage;
        item.GosiEmployerPercentage = dto.GosiEmployerPercentage;
        item.EosBasicSalary = dto.EosBasicSalary;
        item.EosServiceStartDate = dto.EosServiceStartDate;
        item.IncludeInWps = dto.IncludeInWps;
        item.ModifiedAt = DateTime.UtcNow;
        item.ModifiedBy = userId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(item.Id, "Saved", "Saudi payroll information saved");
    }
}

public class ListWpsBatchesHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListWpsBatchesQuery, ListWpsBatchesResult>
{
    public async Task<ListWpsBatchesResult> Handle(ListWpsBatchesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.WpsBatches.AsNoTracking().Include(x => x.Rows).Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.PayrollPeriodId.HasValue && request.PayrollPeriodId.Value != Guid.Empty)
            query = query.Where(x => x.PayrollPeriodId == request.PayrollPeriodId.Value);
        var data = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        return new(data.Select(x => PayrollSaudiComplianceMapper.ToDto(x, true)).ToList());
    }
}

public class CreateWpsBatchHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateWpsBatchCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(CreateWpsBatchCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Batch;
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var payslipQuery = dbContext.Payslips.AsNoTracking()
            .Where(x => x.CompanyId == dto.CompanyId && x.PayrollPeriodId == dto.PayrollPeriodId && !x.IsDeleted && (x.Status == PayslipStatus.Paid || x.Status == PayslipStatus.Closed));
        if (dto.PayrollEntryId.HasValue && dto.PayrollEntryId.Value != Guid.Empty)
            payslipQuery = payslipQuery.Where(x => x.PayrollEntryId == dto.PayrollEntryId.Value);

        var payslips = await payslipQuery.ToListAsync(cancellationToken);
        var employeeIds = payslips.Select(x => x.EmployeeId).Distinct().ToList();
        var saudi = await dbContext.SaudiPayrollInfos.AsNoTracking()
            .Where(x => x.CompanyId == dto.CompanyId && employeeIds.Contains(x.EmployeeId) && x.IncludeInWps && !x.IsDeleted && x.Iban != null && x.Iban != "")
            .ToDictionaryAsync(x => x.EmployeeId, cancellationToken);

        var batch = new WpsBatch
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            PayrollPeriodId = dto.PayrollPeriodId,
            PayrollEntryId = dto.PayrollEntryId,
            BatchNumber = $"WPS-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Status = WpsBatchStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        batch.SetRows(payslips
            .Where(x => saudi.ContainsKey(x.EmployeeId))
            .Select(x =>
            {
                var info = saudi[x.EmployeeId];
                return new WpsBatchRow
                {
                    Id = Guid.NewGuid(),
                    WpsBatchId = batch.Id,
                    EmployeeId = x.EmployeeId,
                    PayslipId = x.Id,
                    Iban = info.Iban!,
                    BankCode = info.BankCode,
                    NetAmount = x.NetAmount,
                    Remarks = $"{x.Year}-{x.Month:00}"
                };
            }));

        await dbContext.WpsBatches.AddAsync(batch, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new PayrollActionResultDto { Id = batch.Id, Status = batch.Status.ToString(), Message = "WPS batch created", ReferenceNumber = batch.BatchNumber };
    }
}

public class MarkWpsBatchExportedHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<MarkWpsBatchExportedCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(MarkWpsBatchExportedCommand request, CancellationToken cancellationToken)
    {
        var batch = await dbContext.WpsBatches.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        batch.Status = WpsBatchStatus.Exported;
        batch.ExportedAt = DateTime.UtcNow;
        batch.ExportedBy = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        batch.ModifiedAt = DateTime.UtcNow;
        batch.ModifiedBy = batch.ExportedBy;
        await dbContext.SaveChangesAsync(cancellationToken);
        return new PayrollActionResultDto { Id = batch.Id, Status = batch.Status.ToString(), Message = "WPS batch marked as exported", ReferenceNumber = batch.BatchNumber };
    }
}

public class ListEosProvisionSnapshotsHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListEosProvisionSnapshotsQuery, ListEosProvisionSnapshotsResult>
{
    public async Task<ListEosProvisionSnapshotsResult> Handle(ListEosProvisionSnapshotsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.EosProvisionSnapshots.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.PayrollPeriodId.HasValue && request.PayrollPeriodId.Value != Guid.Empty)
            query = query.Where(x => x.PayrollPeriodId == request.PayrollPeriodId.Value);
        if (request.EmployeeId.HasValue && request.EmployeeId.Value != Guid.Empty)
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        var data = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        return new(data.Select(PayrollSaudiComplianceMapper.ToDto).ToList());
    }
}

public class CreateEosProvisionSnapshotHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateEosProvisionSnapshotCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(CreateEosProvisionSnapshotCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Snapshot;
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var period = await dbContext.PayrollPeriods.FirstAsync(x => x.Id == dto.PayrollPeriodId && !x.IsDeleted, cancellationToken);
        var info = await dbContext.SaudiPayrollInfos.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == dto.CompanyId && x.EmployeeId == dto.EmployeeId && !x.IsDeleted, cancellationToken);
        var basis = dto.GrossPayBasis ?? info?.EosBasicSalary ?? await dbContext.Payslips
            .Where(x => x.CompanyId == dto.CompanyId && x.EmployeeId == dto.EmployeeId && x.PayrollPeriodId == dto.PayrollPeriodId && !x.IsDeleted)
            .Select(x => x.GrossAmount)
            .FirstOrDefaultAsync(cancellationToken);
        var start = info?.EosServiceStartDate ?? period.StartDate;
        var end = dto.ServiceEndDate ?? period.EndDate;
        var serviceYears = Math.Max((decimal)(end.Date - start.Date).TotalDays / 365m, 0m);
        var provision = basis * serviceYears / 2m;

        var snapshot = new EosProvisionSnapshot
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            EmployeeId = dto.EmployeeId,
            PayrollPeriodId = dto.PayrollPeriodId,
            ServiceStartDate = start,
            ServiceEndDate = end,
            GrossPayBasis = basis,
            ProvisionAmount = provision,
            Notes = "Foundation EOS provision snapshot",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        await dbContext.EosProvisionSnapshots.AddAsync(snapshot, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(snapshot.Id, "Created", "EOS provision snapshot created");
    }
}

public class PostPayrollEntryAccountingHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<PostPayrollEntryAccountingCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(PostPayrollEntryAccountingCommand request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.PayrollEntries.FirstAsync(x => x.Id == request.PayrollEntryId && !x.IsDeleted, cancellationToken);
        if (entry.AccountingJournalEntryId.HasValue)
            return new PayrollActionResultDto { Id = entry.Id, Status = "Posted", Message = "Payroll entry already posted", ReferenceId = entry.AccountingJournalEntryId, ReferenceNumber = entry.AccountingJournalNumber };

        var period = await dbContext.PayrollPeriods.FirstAsync(x => x.Id == entry.PayrollPeriodId && !x.IsDeleted, cancellationToken);
        var payslips = await dbContext.Payslips.AsNoTracking().Where(x => x.PayrollEntryId == entry.Id && !x.IsDeleted && (x.Status == PayslipStatus.Paid || x.Status == PayslipStatus.Closed || x.Status == PayslipStatus.Approved)).ToListAsync(cancellationToken);
        var gross = payslips.Sum(x => x.GrossAmount);
        var net = payslips.Sum(x => x.NetAmount);
        var deductions = payslips.Sum(x => x.TotalDeductions + x.TotalLoans);
        if (gross <= 0)
            throw new InvalidOperationException("Payroll entry has no payable payslips to post");

        var lines = new List<JournalEntryLineDto>
        {
            new() { AccountRole = AccountRole.Expense, Debit = gross, Description = $"Payroll {period.Year}-{period.Month:00}" }
        };
        if (net > 0)
            lines.Add(new() { AccountRole = AccountRole.Payable, Credit = net, Description = "Net salary payable" });
        if (deductions > 0)
            lines.Add(new() { AccountRole = AccountRole.Receivable, Credit = deductions, Description = "Payroll deductions and loan recovery" });

        var result = await sender.Send(new CreateAndPostJournalEntryCommand(new CreateJournalEntryDto
        {
            CompanyId = entry.CompanyId,
            EntryDate = DateTime.UtcNow,
            SourceModule = "Payroll",
            SourceDocumentId = entry.Id,
            SourceDocumentNumber = $"{period.Year}-{period.Month:00}-{entry.Id:N}",
            Memo = $"Payroll entry {period.Year}-{period.Month:00}",
            Lines = lines
        }), cancellationToken);

        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        entry.AccountingJournalEntryId = result.JournalEntryId;
        entry.AccountingJournalNumber = result.Number;
        entry.AccountingPostedAt = DateTime.UtcNow;
        entry.AccountingPostedBy = userId;
        entry.ModifiedAt = DateTime.UtcNow;
        entry.ModifiedBy = userId;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new PayrollActionResultDto { Id = entry.Id, Status = "Posted", Message = "Payroll entry posted to accounting", ReferenceId = result.JournalEntryId, ReferenceNumber = result.Number };
    }
}

public class ListImportedPayrollWorkEntriesHandler(PayrollDbContext dbContext)
    : IQueryHandler<ListImportedPayrollWorkEntriesQuery, ListImportedPayrollWorkEntriesResult>
{
    public async Task<ListImportedPayrollWorkEntriesResult> Handle(ListImportedPayrollWorkEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.PayrollImportedWorkEntries.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.PayrollPeriodId.HasValue && request.PayrollPeriodId.Value != Guid.Empty)
            query = query.Where(x => x.PayrollPeriodId == request.PayrollPeriodId.Value);
        if (request.EmployeeId.HasValue && request.EmployeeId.Value != Guid.Empty)
            query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

        var data = await query.OrderByDescending(x => x.WorkDate).ToListAsync(cancellationToken);
        return new(data.Select(x => new PayrollWorkEntryImportDto
        {
            Id = x.Id,
            CompanyId = x.CompanyId,
            EmployeeId = x.EmployeeId,
            PayrollPeriodId = x.PayrollPeriodId,
            SourceWorkEntryId = x.SourceWorkEntryId,
            WorkDate = x.WorkDate,
            EntryType = x.EntryType,
            Hours = x.Hours,
            Amount = x.Amount,
            Notes = x.Notes
        }).ToList());
    }
}

public class ImportPayrollWorkEntryHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ImportPayrollWorkEntryCommand, PayrollActionResultDto>
{
    public async Task<PayrollActionResultDto> Handle(ImportPayrollWorkEntryCommand request, CancellationToken cancellationToken)
    {
        var dto = request.WorkEntry;
        var userId = PayrollEnhancementHelpers.CurrentUser(httpContextAccessor);
        var item = dto.Id == Guid.Empty && dto.SourceWorkEntryId.HasValue
            ? await dbContext.PayrollImportedWorkEntries.FirstOrDefaultAsync(x => x.SourceWorkEntryId == dto.SourceWorkEntryId && !x.IsDeleted, cancellationToken)
            : dto.Id == Guid.Empty ? null : await dbContext.PayrollImportedWorkEntries.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);

        if (item is null)
        {
            item = new PayrollImportedWorkEntry { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
            await dbContext.PayrollImportedWorkEntries.AddAsync(item, cancellationToken);
        }

        item.CompanyId = dto.CompanyId;
        item.EmployeeId = dto.EmployeeId;
        item.PayrollPeriodId = dto.PayrollPeriodId;
        item.SourceWorkEntryId = dto.SourceWorkEntryId;
        item.WorkDate = dto.WorkDate;
        item.EntryType = dto.EntryType;
        item.Hours = dto.Hours;
        item.Amount = dto.Amount;
        item.Notes = dto.Notes;
        item.ModifiedAt = DateTime.UtcNow;
        item.ModifiedBy = userId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return PayrollEnhancementHelpers.ActionResult(item.Id, "Imported", "Payroll work entry imported");
    }
}
