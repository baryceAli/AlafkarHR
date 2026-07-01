namespace Accounting.Accounting.Features;

public record CreateAccountCommand(AccountDto Account) : ICommand<CreateAccountResult>;
public record CreateAccountResult(Guid Id);
public record UpdateAccountCommand(Guid Id, AccountDto Account) : ICommand<UpdateAccountResult>;
public record UpdateAccountResult(Guid Id);
public record GetAccountsQuery(Guid CompanyId, Guid? BranchId, int PageIndex, int PageSize, string? SearchText) : IQuery<GetAccountsResult>;
public record GetAccountsResult(PaginatedResult<AccountDto> Accounts);
public record CreateFiscalPeriodCommand(FiscalPeriodDto Period) : ICommand<CreateFiscalPeriodResult>;
public record CreateFiscalPeriodResult(Guid Id);
public record CloseFiscalPeriodCommand(Guid Id) : ICommand<FiscalPeriodActionResult>;
public record LockFiscalPeriodCommand(Guid Id) : ICommand<FiscalPeriodActionResult>;
public record ReopenFiscalPeriodCommand(Guid Id) : ICommand<FiscalPeriodActionResult>;
public record YearEndCloseFiscalPeriodCommand(Guid Id) : ICommand<YearEndCloseFiscalPeriodResult>;
public record FiscalPeriodActionResult(Guid Id, FiscalPeriodStatus Status);
public record YearEndCloseFiscalPeriodResult(Guid Id, FiscalPeriodStatus Status, Guid JournalEntryId);
public record GetFiscalPeriodsQuery(Guid CompanyId) : IQuery<GetFiscalPeriodsResult>;
public record GetFiscalPeriodsResult(List<FiscalPeriodDto> Periods);
public record CreateTaxCodeCommand(TaxCodeDto TaxCode) : ICommand<CreateTaxCodeResult>;
public record CreateTaxCodeResult(Guid Id);
public record GetTaxCodesQuery(Guid CompanyId) : IQuery<GetTaxCodesResult>;
public record GetTaxCodesResult(List<TaxCodeDto> TaxCodes);
public record CreatePostingProfileCommand(PostingProfileDto Profile) : ICommand<CreatePostingProfileResult>;
public record CreatePostingProfileResult(Guid Id);
public record GetPostingProfilesQuery(Guid CompanyId) : IQuery<GetPostingProfilesResult>;
public record GetPostingProfilesResult(List<PostingProfileDto> Profiles);
public record GetBankAccountsQuery(Guid CompanyId, Guid? BranchId) : IQuery<GetBankAccountsResult>;
public record GetBankAccountsResult(List<BankAccountDto> BankAccounts);
public record UpsertBankAccountCommand(BankAccountDto BankAccount) : ICommand<UpsertBankAccountResult>;
public record UpsertBankAccountResult(Guid Id);
public record GetCashAccountsQuery(Guid CompanyId, Guid? BranchId) : IQuery<GetCashAccountsResult>;
public record GetCashAccountsResult(List<CashAccountDto> CashAccounts);
public record UpsertCashAccountCommand(CashAccountDto CashAccount) : ICommand<UpsertCashAccountResult>;
public record UpsertCashAccountResult(Guid Id);
public record GetCompanyAccountingSettingsQuery(Guid CompanyId) : IQuery<GetCompanyAccountingSettingsResult>;
public record GetCompanyAccountingSettingsResult(CompanyAccountingSettingsDto? Settings);
public record UpsertCompanyAccountingSettingsCommand(CompanyAccountingSettingsDto Settings) : ICommand<UpsertCompanyAccountingSettingsResult>;
public record UpsertCompanyAccountingSettingsResult(Guid Id);
public record GetAccountCodingSettingsQuery(Guid CompanyId) : IQuery<GetAccountCodingSettingsResult>;
public record GetAccountCodingSettingsResult(AccountCodingSettingsDto Settings);
public record UpsertAccountCodingSettingsCommand(AccountCodingSettingsDto Settings) : ICommand<UpsertAccountCodingSettingsResult>;
public record UpsertAccountCodingSettingsResult(Guid Id);
public record PreviewAccountRenumberCommand(AccountCodingSettingsDto Settings) : ICommand<PreviewAccountRenumberResult>;
public record PreviewAccountRenumberResult(AccountRenumberPreviewDto Preview);
public record ApplyAccountRenumberCommand(ApplyAccountRenumberDto Renumber) : ICommand<ApplyAccountRenumberResult>;
public record ApplyAccountRenumberResult(AccountRenumberPreviewDto Preview);
public record GetAccountingDocumentsQuery(AccountingDocumentType? Type, Guid? CompanyId, Guid? BranchId, int PageIndex, int PageSize, string? SearchText) : IQuery<GetAccountingDocumentsResult>;
public record GetAccountingDocumentsResult(PaginatedResult<AccountingDocumentDto> Documents);
public record GetJournalEntriesQuery(Guid? CompanyId, Guid? BranchId, int PageIndex, int PageSize, string? SearchText) : IQuery<GetJournalEntriesResult>;
public record GetJournalEntriesResult(PaginatedResult<JournalEntryDto> JournalEntries);
public record UpsertZatcaSettingsCommand(ZatcaSettingsDto Settings) : ICommand<UpsertZatcaSettingsResult>;
public record UpsertZatcaSettingsResult(Guid Id);
public record GetZatcaSettingsQuery(Guid CompanyId) : IQuery<GetZatcaSettingsResult>;
public record GetZatcaSettingsResult(ZatcaSettingsDto? Settings);
public record GetEInvoicesQuery(Guid? CompanyId, ZatcaSubmissionStatus? Status, int PageIndex, int PageSize) : IQuery<GetEInvoicesResult>;
public record GetEInvoicesResult(PaginatedResult<EInvoiceDto> Invoices);
public record SubmitEInvoiceCommand(Guid EInvoiceId) : ICommand<SubmitEInvoiceResult>;
public record SubmitEInvoiceResult(Guid SubmissionId, ZatcaSubmissionStatus Status);
public record GetAccountingDashboardQuery(Guid? CompanyId, Guid? BranchId) : IQuery<GetAccountingDashboardResult>;
public record GetAccountingDashboardResult(AccountingDashboardDto Dashboard);
public record GetAccountingTemplatesQuery(Guid? CompanyId) : IQuery<GetAccountingTemplatesResult>;
public record GetAccountingTemplatesResult(List<AccountingTemplateDto> Templates);
public record GetAccountingTemplateByIdQuery(Guid Id, Guid? CompanyId) : IQuery<GetAccountingTemplateByIdResult>;
public record GetAccountingTemplateByIdResult(AccountingTemplateDto Template);
public record UpsertAccountingTemplateCommand(AccountingTemplateDto Template) : ICommand<UpsertAccountingTemplateResult>;
public record UpsertAccountingTemplateResult(Guid Id);
public record DeleteAccountingTemplateCommand(Guid Id) : ICommand<DeleteAccountingTemplateResult>;
public record DeleteAccountingTemplateResult(Guid Id);
public record CaptureAccountingTemplateCommand(CaptureAccountingTemplateDto Template) : ICommand<UpsertAccountingTemplateResult>;
public record GetAccountingSetupStatusQuery(Guid CompanyId) : IQuery<GetAccountingSetupStatusResult>;
public record GetAccountingSetupStatusResult(AccountingSetupStatusDto Status);
public record ApplyAccountingTemplateCommand(ApplyAccountingTemplateDto Setup) : ICommand<ApplyAccountingTemplateResult>;
public record ApplyAccountingTemplateResult(ApplyAccountingTemplateResultDto Result);
public record CreateQuickJournalEntryCommand(QuickJournalEntryDto JournalEntry) : ICommand<CreateAndPostJournalEntryResult>;
public record CreateBankTransactionCommand(BankTransactionDto Transaction) : ICommand<CreateBankTransactionResult>;
public record CreateBankTransactionResult(Guid Id);
public record ReconcileBankTransactionCommand(ReconcileBankTransactionDto Reconciliation) : ICommand<ReconcileBankTransactionResult>;
public record ReconcileBankTransactionResult(Guid Id, BankTransactionStatus Status);
public record IgnoreBankTransactionCommand(Guid Id) : ICommand<BankTransactionActionResult>;
public record UnreconcileBankTransactionCommand(Guid Id) : ICommand<BankTransactionActionResult>;
public record BankTransactionActionResult(Guid Id, BankTransactionStatus Status);
public record GetBankTransactionsQuery(Guid CompanyId, Guid? BranchId, BankTransactionStatus? Status, int PageIndex, int PageSize, string? SearchText) : IQuery<GetBankTransactionsResult>;
public record GetBankTransactionsResult(PaginatedResult<BankTransactionDto> Transactions);
public record GetBankReconciliationSummaryQuery(Guid CompanyId, Guid? BranchId) : IQuery<GetBankReconciliationSummaryResult>;
public record GetBankReconciliationSummaryResult(BankReconciliationSummaryDto Summary);
public record GetBankReconciliationMatchesQuery(Guid BankTransactionId) : IQuery<GetBankReconciliationMatchesResult>;
public record GetBankReconciliationMatchesResult(List<BankReconciliationMatchDto> Matches);
public record GetAccountingReportQuery(AccountingReportType Type, Guid CompanyId, Guid? BranchId, DateTime? FromDate, DateTime? ToDate) : IQuery<GetAccountingReportResult>;
public record GetAccountingReportResult(AccountingReportDto Report);
public record ReverseAccountingDocumentCommand(Guid Id) : ICommand<ReverseAccountingDocumentResult>;
public record ReverseAccountingDocumentResult(Guid Id, AccountingDocumentStatus Status, Guid? ReversalJournalEntryId);
public record ReverseJournalEntryCommand(Guid Id) : ICommand<ReverseJournalEntryResult>;
public record ReverseJournalEntryResult(Guid Id, JournalEntryStatus Status, Guid ReversalJournalEntryId);

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Account.CompanyId).NotEmpty();
        RuleFor(x => x.Account.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Account.NameEng).NotEmpty().MaximumLength(200);
    }
}

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Account.CompanyId).NotEmpty();
        RuleFor(x => x.Account.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Account.NameEng).NotEmpty().MaximumLength(200);
    }
}

public class CreateAccountingDocumentCommandValidator : AbstractValidator<CreateAccountingDocumentCommand>
{
    public CreateAccountingDocumentCommandValidator()
    {
        RuleFor(x => x.Document.CompanyId).NotEmpty();
        RuleFor(x => x.Document.Lines).NotEmpty();
        RuleForEach(x => x.Document.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.Description).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        });
    }
}

public class CreateAndPostJournalEntryCommandValidator : AbstractValidator<CreateAndPostJournalEntryCommand>
{
    public CreateAndPostJournalEntryCommandValidator()
    {
        RuleFor(x => x.JournalEntry.CompanyId).NotEmpty();
        RuleFor(x => x.JournalEntry.Lines).NotEmpty();
        RuleForEach(x => x.JournalEntry.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x).Must(x => x.AccountId != Guid.Empty || x.AccountRole != AccountRole.None)
                .WithMessage("Journal line must contain an account id or account role.");
            line.RuleFor(x => x).Must(x => (x.Debit > 0 && x.Credit == 0) || (x.Credit > 0 && x.Debit == 0))
                .WithMessage("Journal line must contain either debit or credit.");
        });
    }
}

public class CreateQuickJournalEntryCommandValidator : AbstractValidator<CreateQuickJournalEntryCommand>
{
    public CreateQuickJournalEntryCommandValidator()
    {
        RuleFor(x => x.JournalEntry.CompanyId).NotEmpty();
        RuleFor(x => x.JournalEntry.DebitAccountId).NotEmpty();
        RuleFor(x => x.JournalEntry.CreditAccountId).NotEmpty();
        RuleFor(x => x.JournalEntry.Amount).GreaterThan(0);
        RuleFor(x => x.JournalEntry).Must(x => x.DebitAccountId != x.CreditAccountId)
            .WithMessage("Debit and credit accounts must be different.");
    }
}

public class CreateBankTransactionCommandValidator : AbstractValidator<CreateBankTransactionCommand>
{
    public CreateBankTransactionCommandValidator()
    {
        RuleFor(x => x.Transaction.CompanyId).NotEmpty();
        RuleFor(x => x.Transaction.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Transaction.Amount).NotEqual(0);
    }
}

public class AccountingCommandHandlers(AccountingDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<CreateAccountCommand, CreateAccountResult>,
      ICommandHandler<UpdateAccountCommand, UpdateAccountResult>,
      ICommandHandler<CreateFiscalPeriodCommand, CreateFiscalPeriodResult>,
      ICommandHandler<CloseFiscalPeriodCommand, FiscalPeriodActionResult>,
      ICommandHandler<LockFiscalPeriodCommand, FiscalPeriodActionResult>,
      ICommandHandler<ReopenFiscalPeriodCommand, FiscalPeriodActionResult>,
      ICommandHandler<YearEndCloseFiscalPeriodCommand, YearEndCloseFiscalPeriodResult>,
      ICommandHandler<CreateTaxCodeCommand, CreateTaxCodeResult>,
      ICommandHandler<CreatePostingProfileCommand, CreatePostingProfileResult>,
      ICommandHandler<UpsertBankAccountCommand, UpsertBankAccountResult>,
      ICommandHandler<UpsertCashAccountCommand, UpsertCashAccountResult>,
      ICommandHandler<UpsertAccountingCashAccountCommand, UpsertAccountingCashAccountResult>,
      ICommandHandler<UpsertCompanyAccountingSettingsCommand, UpsertCompanyAccountingSettingsResult>,
      ICommandHandler<UpsertAccountCodingSettingsCommand, UpsertAccountCodingSettingsResult>,
      ICommandHandler<PreviewAccountRenumberCommand, PreviewAccountRenumberResult>,
      ICommandHandler<ApplyAccountRenumberCommand, ApplyAccountRenumberResult>,
      ICommandHandler<CreateAccountingDocumentCommand, CreateAccountingDocumentResult>,
      ICommandHandler<PostAccountingDocumentCommand, PostAccountingDocumentResult>,
      ICommandHandler<CreateAndPostJournalEntryCommand, CreateAndPostJournalEntryResult>,
      IQueryHandler<GetAccountingCashAccountScopeQuery, GetAccountingCashAccountScopeResult>,
      ICommandHandler<CreateQuickJournalEntryCommand, CreateAndPostJournalEntryResult>,
      ICommandHandler<RecordAccountingReceiptCommand, CreateAccountingDocumentResult>,
      ICommandHandler<GenerateZatcaInvoiceCommand, GenerateZatcaInvoiceResult>,
      ICommandHandler<UpsertZatcaSettingsCommand, UpsertZatcaSettingsResult>,
      ICommandHandler<SubmitEInvoiceCommand, SubmitEInvoiceResult>,
      ICommandHandler<UpsertAccountingTemplateCommand, UpsertAccountingTemplateResult>,
      ICommandHandler<DeleteAccountingTemplateCommand, DeleteAccountingTemplateResult>,
      ICommandHandler<CaptureAccountingTemplateCommand, UpsertAccountingTemplateResult>,
      ICommandHandler<ApplyAccountingTemplateCommand, ApplyAccountingTemplateResult>,
      ICommandHandler<EnsureBranchAccountingCommand, EnsureBranchAccountingResult>,
      ICommandHandler<CreateBankTransactionCommand, CreateBankTransactionResult>,
      ICommandHandler<ReconcileBankTransactionCommand, ReconcileBankTransactionResult>,
      ICommandHandler<IgnoreBankTransactionCommand, BankTransactionActionResult>,
      ICommandHandler<UnreconcileBankTransactionCommand, BankTransactionActionResult>,
      ICommandHandler<ReverseAccountingDocumentCommand, ReverseAccountingDocumentResult>,
      ICommandHandler<ReverseJournalEntryCommand, ReverseJournalEntryResult>
{
    public async Task<CreateAccountResult> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        await EnsureCanAccessBranchAsync(command.Account.CompanyId, command.Account.BranchId, cancellationToken);
        var codingSettings = await GetCodingSettingsDtoAsync(command.Account.CompanyId, cancellationToken);
        command.Account.Code = await GenerateAccountCodeAsync(command.Account, null, codingSettings, cancellationToken);
        await ValidateAccountAsync(command.Account, null, codingSettings, cancellationToken);
        var account = Account.Create(command.Account, UserId);
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateAccountResult(account.Id);
    }

    public async Task<UpdateAccountResult> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Account;
        dto.Id = command.Id;
        await EnsureCanAccessBranchAsync(dto.CompanyId, dto.BranchId, cancellationToken);

        var account = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == command.Id && x.CompanyId == dto.CompanyId, cancellationToken);
        if (account is null)
            throw new BadRequestException("Account was not found.");

        var structureChanged = AccountStructureChanged(dto, account);
        if (structureChanged && await dbContext.Accounts.AsNoTracking().AnyAsync(x => x.CompanyId == dto.CompanyId && x.ParentAccountId == account.Id, cancellationToken))
            throw new BadRequestException("Group accounts with child accounts must be renumbered through account coding settings.");

        var codingSettings = await GetCodingSettingsDtoAsync(dto.CompanyId, cancellationToken);
        dto.Code = structureChanged ? await GenerateAccountCodeAsync(dto, command.Id, codingSettings, cancellationToken) : account.Code;
        await ValidateAccountAsync(dto, command.Id, codingSettings, cancellationToken);
        account.Update(dto, UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateAccountResult(account.Id);
    }

    public async Task<CreateFiscalPeriodResult> Handle(CreateFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        var startDate = command.Period.StartDate.Date;
        var endDate = command.Period.EndDate.Date;
        var overlaps = await dbContext.FiscalPeriods.AsNoTracking()
            .AnyAsync(x => x.CompanyId == command.Period.CompanyId
                && x.StartDate <= endDate
                && x.EndDate >= startDate, cancellationToken);
        if (overlaps)
            throw new BadRequestException("Fiscal period dates overlap with an existing period.");

        var period = FiscalPeriod.Create(command.Period, UserId);
        await dbContext.FiscalPeriods.AddAsync(period, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateFiscalPeriodResult(period.Id);
    }

    public async Task<FiscalPeriodActionResult> Handle(CloseFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        var period = await dbContext.FiscalPeriods.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Fiscal period", command.Id);

        period.Close(UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FiscalPeriodActionResult(period.Id, period.Status);
    }

    public async Task<FiscalPeriodActionResult> Handle(LockFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        var period = await dbContext.FiscalPeriods.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Fiscal period", command.Id);

        period.Lock(UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FiscalPeriodActionResult(period.Id, period.Status);
    }

    public async Task<FiscalPeriodActionResult> Handle(ReopenFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        var period = await dbContext.FiscalPeriods.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Fiscal period", command.Id);

        period.Reopen(UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FiscalPeriodActionResult(period.Id, period.Status);
    }

    public async Task<YearEndCloseFiscalPeriodResult> Handle(YearEndCloseFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        var period = await dbContext.FiscalPeriods.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Fiscal period", command.Id);

        if (period.Status == FiscalPeriodStatus.Locked)
            throw new BadRequestException("Locked fiscal periods must be reopened before year-end close.");

        var existingClosing = await dbContext.JournalEntries.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == period.CompanyId
                && x.SourceModule == ClosingSourceModule
                && x.SourceDocumentId == period.Id
                && x.Status != JournalEntryStatus.Reversed, cancellationToken);
        if (existingClosing is not null)
            throw new BadRequestException("Fiscal period already has a year-end closing journal entry.");

        var settings = await dbContext.CompanyAccountingSettings.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == period.CompanyId, cancellationToken)
            ?? throw new BadRequestException("Company accounting settings are required before year-end close.");
        if (!settings.RetainedEarningsAccountId.HasValue || settings.RetainedEarningsAccountId.Value == Guid.Empty)
            throw new BadRequestException("Retained earnings account is required before year-end close.");

        var retainedEarningsAccount = await dbContext.Accounts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == settings.RetainedEarningsAccountId.Value && x.CompanyId == period.CompanyId, cancellationToken)
            ?? throw new BadRequestException("Retained earnings account was not found.");
        if (!retainedEarningsAccount.IsActive || !retainedEarningsAccount.IsPostingAccount)
            throw new BadRequestException("Retained earnings account must be an active posting account.");

        var balances = await GetClosingAccountBalancesAsync(period, cancellationToken);
        if (!balances.Any())
            throw new BadRequestException("No revenue or expense balances were found for this fiscal period.");

        var lines = BuildYearEndClosingLines(balances, retainedEarningsAccount.Id);
        if (!lines.Any())
            throw new BadRequestException("Revenue and expense account balances are already zero for this fiscal period.");

        var journalNumber = await GenerateJournalNumberAsync(period.CompanyId, period.EndDate, cancellationToken);
        var entry = JournalEntry.Create(
            period.CompanyId,
            null,
            journalNumber,
            period.EndDate,
            ClosingSourceModule,
            period.Id,
            period.Name,
            $"Year-end close {period.Name}",
            lines,
            UserId);
        entry.Post(UserId);

        await dbContext.JournalEntries.AddAsync(entry, cancellationToken);
        period.Close(UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new YearEndCloseFiscalPeriodResult(period.Id, period.Status, entry.Id);
    }

    public async Task<CreateTaxCodeResult> Handle(CreateTaxCodeCommand command, CancellationToken cancellationToken)
    {
        var taxCode = TaxCode.Create(command.TaxCode, UserId);
        await dbContext.TaxCodes.AddAsync(taxCode, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateTaxCodeResult(taxCode.Id);
    }

    public async Task<CreatePostingProfileResult> Handle(CreatePostingProfileCommand command, CancellationToken cancellationToken)
    {
        await EnsurePostingProfileAccountsAsync(command.Profile, cancellationToken);
        var profile = PostingProfile.Create(command.Profile, UserId);
        await dbContext.PostingProfiles.AddAsync(profile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreatePostingProfileResult(profile.Id);
    }

    public async Task<UpsertBankAccountResult> Handle(UpsertBankAccountCommand command, CancellationToken cancellationToken)
    {
        var dto = command.BankAccount;
        await EnsureCanAccessBranchAsync(dto.CompanyId, dto.BranchId, cancellationToken);
        var ledgerId = await ResolveOrCreateLedgerAccountAsync(dto.CompanyId, dto.BranchId, dto.LedgerAccountId, dto.DisplayName, AccountRole.Bank, AccountType.Asset, NormalBalance.Debit, cancellationToken);
        var journalId = await ResolveOrCreateJournalAsync(dto.CompanyId, dto.BranchId, dto.JournalId, dto.DisplayName, AccountingJournalType.Bank, ledgerId, cancellationToken);
        var bankAccount = dto.Id == Guid.Empty ? null : await dbContext.BankAccounts.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (bankAccount is null)
        {
            bankAccount = BankAccount.Create(dto, ledgerId, journalId, UserId);
            await dbContext.BankAccounts.AddAsync(bankAccount, cancellationToken);
        }
        else
        {
            bankAccount.Update(dto, ledgerId, journalId, UserId);
        }

        if (dto.IsDefault)
        {
            var others = await dbContext.BankAccounts.Where(x => x.CompanyId == dto.CompanyId && x.BranchId == dto.BranchId && x.Id != bankAccount.Id && x.IsDefault).ToListAsync(cancellationToken);
            foreach (var other in others)
                other.SetDefault(false, UserId);
            await UpsertDefaultPaymentAccountAsync(dto.CompanyId, bankAccount.LedgerAccountId, AccountRole.Bank, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertBankAccountResult(bankAccount.Id);
    }

    public async Task<UpsertCashAccountResult> Handle(UpsertCashAccountCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CashAccount;
        await EnsureCanAccessBranchAsync(dto.CompanyId, dto.BranchId, cancellationToken);
        var ledgerId = await ResolveOrCreateLedgerAccountAsync(dto.CompanyId, dto.BranchId, dto.LedgerAccountId, dto.DisplayName, AccountRole.Cash, AccountType.Asset, NormalBalance.Debit, cancellationToken);
        var journalId = await ResolveOrCreateJournalAsync(dto.CompanyId, dto.BranchId, dto.JournalId, dto.DisplayName, AccountingJournalType.Cash, ledgerId, cancellationToken);
        var cashAccount = dto.Id == Guid.Empty ? null : await dbContext.CashAccounts.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (cashAccount is null)
        {
            cashAccount = CashAccount.Create(dto, ledgerId, journalId, UserId);
            await dbContext.CashAccounts.AddAsync(cashAccount, cancellationToken);
        }
        else
        {
            cashAccount.Update(dto, ledgerId, journalId, UserId);
        }

        if (dto.IsDefault)
        {
            var others = await dbContext.CashAccounts.Where(x => x.CompanyId == dto.CompanyId && x.BranchId == dto.BranchId && x.Id != cashAccount.Id && x.IsDefault).ToListAsync(cancellationToken);
            foreach (var other in others)
                other.SetDefault(false, UserId);
            await UpsertDefaultPaymentAccountAsync(dto.CompanyId, cashAccount.LedgerAccountId, AccountRole.Cash, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertCashAccountResult(cashAccount.Id);
    }

    public async Task<UpsertAccountingCashAccountResult> Handle(UpsertAccountingCashAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await Handle(new UpsertCashAccountCommand(command.CashAccount), cancellationToken);
        return new UpsertAccountingCashAccountResult(result.Id);
    }

    public async Task<UpsertCompanyAccountingSettingsResult> Handle(UpsertCompanyAccountingSettingsCommand command, CancellationToken cancellationToken)
    {
        var settings = await dbContext.CompanyAccountingSettings.FirstOrDefaultAsync(x => x.CompanyId == command.Settings.CompanyId, cancellationToken);
        await EnsureOptionalPostingAccountsAsync(command.Settings.CompanyId, SettingsAccountIds(command.Settings), cancellationToken);
        if (settings is null)
        {
            settings = CompanyAccountingSettings.Upsert(command.Settings, UserId);
            await dbContext.CompanyAccountingSettings.AddAsync(settings, cancellationToken);
        }
        else
        {
            settings.Update(command.Settings, UserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertCompanyAccountingSettingsResult(settings.Id);
    }

    public async Task<UpsertAccountCodingSettingsResult> Handle(UpsertAccountCodingSettingsCommand command, CancellationToken cancellationToken)
    {
        ValidateCodingSettings(command.Settings);
        var settings = await dbContext.AccountCodingSettings.FirstOrDefaultAsync(x => x.CompanyId == command.Settings.CompanyId, cancellationToken);
        var currentSettings = settings?.ToDto() ?? AccountCodingSettings.Default(command.Settings.CompanyId);
        var accountsExist = await dbContext.Accounts.AsNoTracking().AnyAsync(x => x.CompanyId == command.Settings.CompanyId, cancellationToken);
        if (accountsExist && AccountCodePattern.StructuralCodingChanged(currentSettings, command.Settings))
            throw new BadRequestException("Account root code or suffix length changes must be applied through account coding preview and renumber.");

        if (settings is null)
        {
            settings = AccountCodingSettings.Upsert(command.Settings, UserId);
            await dbContext.AccountCodingSettings.AddAsync(settings, cancellationToken);
        }
        else
        {
            settings.Update(command.Settings, UserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAccountCodingSettingsResult(settings.Id);
    }

    public async Task<PreviewAccountRenumberResult> Handle(PreviewAccountRenumberCommand command, CancellationToken cancellationToken)
    {
        ValidateCodingSettings(command.Settings);
        return new PreviewAccountRenumberResult(await BuildRenumberPreviewAsync(command.Settings, cancellationToken));
    }

    public async Task<ApplyAccountRenumberResult> Handle(ApplyAccountRenumberCommand command, CancellationToken cancellationToken)
    {
        var requested = command.Renumber.Settings;
        ValidateCodingSettings(requested);
        var preview = await BuildRenumberPreviewAsync(requested, cancellationToken);
        if (!preview.CanApply)
            throw new BadRequestException(string.Join(" ", preview.Errors.DefaultIfEmpty("Account coding changes cannot be applied.")));

        var accountsById = await dbContext.Accounts
            .Where(x => x.CompanyId == requested.CompanyId)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        foreach (var line in preview.Lines)
        {
            if (!accountsById.TryGetValue(line.AccountId, out var account))
                throw new BadRequestException("Account coding preview is stale. Refresh the preview and apply again.");

            if (!string.Equals(account.Code, line.OldCode, StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Account coding preview is stale. Refresh the preview and apply again.");
        }

        var renumberedIds = preview.Lines.Select(x => x.AccountId).ToHashSet();
        var unchangedCodes = accountsById.Values
            .Where(x => !renumberedIds.Contains(x.Id))
            .Select(x => x.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (preview.Lines.Select(x => TemporaryRenumberCode(x.AccountId)).Any(unchangedCodes.Contains))
            throw new BadRequestException("Temporary account renumbering code would conflict with an existing account.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        foreach (var line in preview.Lines)
        {
            if (accountsById.TryGetValue(line.AccountId, out var account))
                account.ChangeCode(TemporaryRenumberCode(line.AccountId), UserId);
        }

        if (preview.Lines.Any())
            await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var line in preview.Lines)
        {
            if (accountsById.TryGetValue(line.AccountId, out var account))
                account.ChangeCode(line.NewCode, UserId);
        }

        var settings = await dbContext.AccountCodingSettings.FirstOrDefaultAsync(x => x.CompanyId == requested.CompanyId, cancellationToken);
        if (settings is null)
        {
            settings = AccountCodingSettings.Upsert(requested, UserId);
            await dbContext.AccountCodingSettings.AddAsync(settings, cancellationToken);
        }
        else
        {
            settings.Update(requested, UserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new ApplyAccountRenumberResult(preview);
    }

    public async Task<CreateAccountingDocumentResult> Handle(CreateAccountingDocumentCommand command, CancellationToken cancellationToken)
    {
        await EnsureCanAccessBranchAsync(command.Document.CompanyId, command.Document.BranchId, cancellationToken);
        await EnsureDocumentCashBankAccountsAsync(command.Document, cancellationToken);
        if (command.Document.SourceDocumentId.HasValue && !string.IsNullOrWhiteSpace(command.Document.SourceModule))
        {
            var sourceModule = command.Document.SourceModule.Trim();
            var existing = await dbContext.AccountingDocuments.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == command.Document.CompanyId
                    && x.Type == command.Document.Type
                    && x.SourceModule == sourceModule
                    && x.SourceDocumentId == command.Document.SourceDocumentId.Value, cancellationToken);

            if (existing is not null)
                return new CreateAccountingDocumentResult(existing.Id, existing.Number);
        }

        var number = string.IsNullOrWhiteSpace(command.Document.Number)
            ? await GenerateDocumentNumberAsync(command.Document.CompanyId, command.Document.Type, command.Document.DocumentDate, cancellationToken)
            : command.Document.Number.Trim();

        var document = AccountingDocument.Create(command.Document, number, UserId);
        await dbContext.AccountingDocuments.AddAsync(document, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateAccountingDocumentResult(document.Id, document.Number);
    }

    public async Task<PostAccountingDocumentResult> Handle(PostAccountingDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = await dbContext.AccountingDocuments.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Accounting document", command.Id);

        await EnsureCanAccessBranchAsync(document.CompanyId, document.BranchId, cancellationToken);
        if (document.Status == AccountingDocumentStatus.Posted && document.JournalEntryId.HasValue)
            return new PostAccountingDocumentResult(document.JournalEntryId.Value);

        await EnsureOpenFiscalPeriodAsync(document.CompanyId, document.DocumentDate, cancellationToken);
        var profile = await ResolvePostingProfileAsync(document.CompanyId, document.Type, cancellationToken);
        var settings = await dbContext.CompanyAccountingSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == document.CompanyId, cancellationToken);
        var journalNumber = await GenerateJournalNumberAsync(document.CompanyId, document.DocumentDate, cancellationToken);
        var lines = BuildJournalLines(document, profile, settings);
        await EnsurePostingAccountsAsync(document.CompanyId, document.BranchId, lines.Select(x => x.AccountId), cancellationToken);
        var entry = JournalEntry.Create(
            document.CompanyId,
            document.BranchId,
            journalNumber,
            document.DocumentDate,
            "Accounting",
            document.Id,
            document.Number,
            $"{document.Type} {document.Number}",
            lines,
            UserId);
        entry.Post(UserId);

        await dbContext.JournalEntries.AddAsync(entry, cancellationToken);
        document.Post(entry.Id, UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new PostAccountingDocumentResult(entry.Id);
    }

    public async Task<ReverseAccountingDocumentResult> Handle(ReverseAccountingDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = await dbContext.AccountingDocuments.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Accounting document", command.Id);

        await EnsureCanAccessBranchAsync(document.CompanyId, document.BranchId, cancellationToken);
        if (document.Status != AccountingDocumentStatus.Posted)
            throw new BadRequestException("Only posted accounting documents can be reversed.");

        Guid? reversalJournalEntryId = null;
        if (document.JournalEntryId.HasValue)
            reversalJournalEntryId = await ReverseJournalEntryAsync(document.JournalEntryId.Value, cancellationToken, allowAccountingDocumentJournal: true);

        document.Reverse(UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReverseAccountingDocumentResult(document.Id, document.Status, reversalJournalEntryId);
    }

    public async Task<CreateAndPostJournalEntryResult> Handle(CreateAndPostJournalEntryCommand command, CancellationToken cancellationToken)
    {
        if (command.JournalEntry.SourceDocumentId.HasValue && !string.IsNullOrWhiteSpace(command.JournalEntry.SourceModule))
        {
            var sourceModule = command.JournalEntry.SourceModule.Trim();
            var existing = await dbContext.JournalEntries.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == command.JournalEntry.CompanyId
                    && x.SourceModule == sourceModule
                    && x.SourceDocumentId == command.JournalEntry.SourceDocumentId.Value, cancellationToken);

            if (existing is not null)
                return new CreateAndPostJournalEntryResult(existing.Id, existing.Number);
        }
        else if (!string.IsNullOrWhiteSpace(command.JournalEntry.SourceModule) && !string.IsNullOrWhiteSpace(command.JournalEntry.SourceDocumentNumber))
        {
            var sourceModule = command.JournalEntry.SourceModule.Trim();
            var sourceDocumentNumber = command.JournalEntry.SourceDocumentNumber.Trim();
            var existing = await dbContext.JournalEntries.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == command.JournalEntry.CompanyId
                    && x.SourceModule == sourceModule
                    && x.SourceDocumentNumber == sourceDocumentNumber, cancellationToken);

            if (existing is not null)
                return new CreateAndPostJournalEntryResult(existing.Id, existing.Number);
        }

        await EnsureCanAccessBranchAsync(command.JournalEntry.CompanyId, command.JournalEntry.BranchId, cancellationToken);
        await EnsureOpenFiscalPeriodAsync(command.JournalEntry.CompanyId, command.JournalEntry.EntryDate, cancellationToken);
        var journalNumber = await GenerateJournalNumberAsync(command.JournalEntry.CompanyId, command.JournalEntry.EntryDate, cancellationToken);
        var lines = await ResolveJournalLinesAsync(command.JournalEntry.CompanyId, command.JournalEntry.BranchId, command.JournalEntry.Lines, cancellationToken);
        await EnsurePostingAccountsAsync(command.JournalEntry.CompanyId, command.JournalEntry.BranchId, lines.Select(x => x.AccountId), cancellationToken);
        var entry = JournalEntry.Create(
            command.JournalEntry.CompanyId,
            command.JournalEntry.BranchId,
            journalNumber,
            command.JournalEntry.EntryDate,
            command.JournalEntry.SourceModule,
            command.JournalEntry.SourceDocumentId,
            command.JournalEntry.SourceDocumentNumber,
            command.JournalEntry.Memo,
            lines,
            UserId);
        entry.Post(UserId);

        await dbContext.JournalEntries.AddAsync(entry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateAndPostJournalEntryResult(entry.Id, entry.Number);
    }

    public async Task<ReverseJournalEntryResult> Handle(ReverseJournalEntryCommand command, CancellationToken cancellationToken)
    {
        var reversalJournalEntryId = await ReverseJournalEntryAsync(command.Id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReverseJournalEntryResult(command.Id, JournalEntryStatus.Reversed, reversalJournalEntryId);
    }

    public async Task<GetAccountingCashAccountScopeResult> Handle(GetAccountingCashAccountScopeQuery query, CancellationToken cancellationToken)
    {
        var cashAccount = await dbContext.CashAccounts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.CashAccountId
                && x.CompanyId == query.CompanyId
                && x.BranchId == query.BranchId
                && x.IsActive, cancellationToken)
            ?? throw new BadRequestException("Cash account must be active and belong to the StoreFront branch.");

        return new GetAccountingCashAccountScopeResult(cashAccount.Id, cashAccount.LedgerAccountId);
    }

    public async Task<CreateAndPostJournalEntryResult> Handle(CreateQuickJournalEntryCommand command, CancellationToken cancellationToken)
    {
        var quick = command.JournalEntry;
        return await Handle(new CreateAndPostJournalEntryCommand(new CreateJournalEntryDto
        {
            CompanyId = quick.CompanyId,
            BranchId = quick.BranchId,
            EntryDate = quick.EntryDate == default ? DateTime.UtcNow.Date : quick.EntryDate.Date,
            SourceModule = "ManualJournal",
            SourceDocumentNumber = quick.ReferenceNumber,
            Memo = quick.Memo,
            Lines =
            [
                new JournalEntryLineDto
                {
                    AccountId = quick.DebitAccountId,
                    Debit = quick.Amount,
                    Description = quick.Memo
                },
                new JournalEntryLineDto
                {
                    AccountId = quick.CreditAccountId,
                    Credit = quick.Amount,
                    Description = quick.Memo
                }
            ]
        }), cancellationToken);
    }

    public async Task<CreateBankTransactionResult> Handle(CreateBankTransactionCommand command, CancellationToken cancellationToken)
    {
        await EnsureCanAccessBranchAsync(command.Transaction.CompanyId, command.Transaction.BranchId, cancellationToken);
        if (!command.Transaction.BankAccountId.HasValue && !command.Transaction.CashAccountId.HasValue)
            throw new BadRequestException("Select a bank or cash account.");

        if (command.Transaction.BankAccountId.HasValue)
        {
            var exists = await dbContext.BankAccounts.AsNoTracking()
                .AnyAsync(x => x.Id == command.Transaction.BankAccountId.Value && x.CompanyId == command.Transaction.CompanyId && x.BranchId == command.Transaction.BranchId && x.IsActive, cancellationToken);
            if (!exists)
                throw new BadRequestException("Bank account was not found.");
        }

        if (command.Transaction.CashAccountId.HasValue)
        {
            var exists = await dbContext.CashAccounts.AsNoTracking()
                .AnyAsync(x => x.Id == command.Transaction.CashAccountId.Value && x.CompanyId == command.Transaction.CompanyId && x.BranchId == command.Transaction.BranchId && x.IsActive, cancellationToken);
            if (!exists)
                throw new BadRequestException("Cash account was not found.");
        }

        var transaction = BankTransaction.Create(command.Transaction, UserId);
        await dbContext.BankTransactions.AddAsync(transaction, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateBankTransactionResult(transaction.Id);
    }

    public async Task<ReconcileBankTransactionResult> Handle(ReconcileBankTransactionCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Reconciliation;
        var transaction = await dbContext.BankTransactions.FirstOrDefaultAsync(x => x.Id == dto.BankTransactionId, cancellationToken)
            ?? throw new NotFoundException("Bank transaction", dto.BankTransactionId);
        await EnsureCanAccessBranchAsync(transaction.CompanyId, transaction.BranchId, cancellationToken);

        if (dto.JournalEntryId.HasValue)
        {
            var exists = await dbContext.JournalEntries.AsNoTracking()
                .AnyAsync(x => x.Id == dto.JournalEntryId.Value && x.CompanyId == transaction.CompanyId && x.BranchId == transaction.BranchId && x.Status == JournalEntryStatus.Posted, cancellationToken);
            if (!exists)
                throw new BadRequestException("Posted journal entry was not found.");
        }

        if (dto.AccountingDocumentId.HasValue)
        {
            var exists = await dbContext.AccountingDocuments.AsNoTracking()
                .AnyAsync(x => x.Id == dto.AccountingDocumentId.Value && x.CompanyId == transaction.CompanyId && x.BranchId == transaction.BranchId && x.Status == AccountingDocumentStatus.Posted, cancellationToken);
            if (!exists)
                throw new BadRequestException("Posted accounting document was not found.");
        }

        if (dto.WriteOffAccountId.HasValue)
            await EnsurePostingAccountsAsync(transaction.CompanyId, transaction.BranchId, [dto.WriteOffAccountId.Value], cancellationToken);

        transaction.Reconcile(dto.JournalEntryId, dto.AccountingDocumentId, dto.WriteOffAccountId, dto.ClearanceDate, UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReconcileBankTransactionResult(transaction.Id, transaction.Status);
    }

    public async Task<BankTransactionActionResult> Handle(IgnoreBankTransactionCommand command, CancellationToken cancellationToken)
    {
        var transaction = await dbContext.BankTransactions.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Bank transaction", command.Id);
        await EnsureCanAccessBranchAsync(transaction.CompanyId, transaction.BranchId, cancellationToken);
        transaction.Ignore(UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BankTransactionActionResult(transaction.Id, transaction.Status);
    }

    public async Task<BankTransactionActionResult> Handle(UnreconcileBankTransactionCommand command, CancellationToken cancellationToken)
    {
        var transaction = await dbContext.BankTransactions.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Bank transaction", command.Id);
        await EnsureCanAccessBranchAsync(transaction.CompanyId, transaction.BranchId, cancellationToken);
        transaction.Unreconcile(UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BankTransactionActionResult(transaction.Id, transaction.Status);
    }

    public async Task<CreateAccountingDocumentResult> Handle(RecordAccountingReceiptCommand command, CancellationToken cancellationToken)
    {
        var document = new AccountingDocumentDto
        {
            CompanyId = command.CompanyId,
            BranchId = command.BranchId,
            CashAccountId = command.CashAccountId,
            BankAccountId = command.BankAccountId,
            Type = AccountingDocumentType.CustomerReceipt,
            DocumentDate = command.ReceiptDate,
            PartyId = command.PartyId,
            PartyName = command.PartyName,
            SourceModule = string.IsNullOrWhiteSpace(command.SourceModule) ? "Payments" : command.SourceModule,
            SourceDocumentId = command.SourceDocumentId,
            SourceDocumentNumber = command.SourceDocumentNumber,
            Lines =
            [
                new AccountingDocumentLineDto
                {
                    Description = $"Receipt for {command.SourceDocumentNumber ?? command.PartyName ?? "customer"}",
                    Quantity = 1,
                    UnitPrice = command.Amount,
                    NetAmount = command.Amount,
                    TotalAmount = command.Amount
                }
            ]
        };

        var created = await Handle(new CreateAccountingDocumentCommand(document), cancellationToken);
        await Handle(new PostAccountingDocumentCommand(created.Id), cancellationToken);
        return created;
    }

    public async Task<GenerateZatcaInvoiceResult> Handle(GenerateZatcaInvoiceCommand command, CancellationToken cancellationToken)
    {
        var document = await dbContext.AccountingDocuments.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == command.AccountingDocumentId, cancellationToken)
            ?? throw new NotFoundException("Accounting document", command.AccountingDocumentId);

        if (document.Status != AccountingDocumentStatus.Posted)
            throw new BadRequestException("Only posted accounting documents can generate ZATCA invoices.");

        var existing = await dbContext.EInvoices.AsNoTracking().FirstOrDefaultAsync(x => x.AccountingDocumentId == document.Id, cancellationToken);
        if (existing is not null)
            return new GenerateZatcaInvoiceResult(existing.Id, existing.InvoiceHash, existing.QrPayload);

        var settings = await dbContext.ZatcaSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == document.CompanyId, cancellationToken)
            ?? throw new BadRequestException("ZATCA settings must be configured before generating e-invoices.");

        var lastInvoice = await dbContext.EInvoices.AsNoTracking()
            .Where(x => x.CompanyId == document.CompanyId)
            .OrderByDescending(x => x.Icv)
            .FirstOrDefaultAsync(cancellationToken);
        var icv = (lastInvoice?.Icv ?? 0) + 1;
        var previousHash = lastInvoice?.InvoiceHash;
        var xml = BuildZatcaXml(document, settings, command.InvoiceType, icv, previousHash);
        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(xml)));
        var qr = BuildQrPayload(settings.SellerName, settings.VatNumber, document.DocumentDate, document.TotalAmount, document.TaxAmount, hash);
        var invoice = EInvoice.Create(document, command.InvoiceType, icv, previousHash, xml, hash, qr, UserId);
        await dbContext.EInvoices.AddAsync(invoice, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new GenerateZatcaInvoiceResult(invoice.Id, invoice.InvoiceHash, invoice.QrPayload);
    }

    public async Task<UpsertZatcaSettingsResult> Handle(UpsertZatcaSettingsCommand command, CancellationToken cancellationToken)
    {
        var settings = await dbContext.ZatcaSettings.FirstOrDefaultAsync(x => x.CompanyId == command.Settings.CompanyId, cancellationToken);
        if (settings is null)
        {
            settings = ZatcaSettings.Upsert(command.Settings, UserId);
            await dbContext.ZatcaSettings.AddAsync(settings, cancellationToken);
        }
        else
        {
            settings.Update(command.Settings, UserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertZatcaSettingsResult(settings.Id);
    }

    public async Task<SubmitEInvoiceResult> Handle(SubmitEInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await dbContext.EInvoices.FirstOrDefaultAsync(x => x.Id == command.EInvoiceId, cancellationToken)
            ?? throw new NotFoundException("E-Invoice", command.EInvoiceId);

        var targetStatus = invoice.InvoiceType == ZatcaInvoiceType.StandardTaxInvoice || invoice.InvoiceType == ZatcaInvoiceType.DebitNote
            ? ZatcaSubmissionStatus.Cleared
            : ZatcaSubmissionStatus.Reported;
        var response = $"Local compliance queue accepted invoice {invoice.InvoiceNumber}. Configure live ZATCA transport before production.";
        var submission = EInvoiceSubmission.Create(invoice.Id, targetStatus, invoice.XmlPayload, response, null, 0, UserId);
        invoice.MarkSubmitted(targetStatus, UserId);
        await dbContext.EInvoiceSubmissions.AddAsync(submission, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SubmitEInvoiceResult(submission.Id, targetStatus);
    }

    public async Task<UpsertAccountingTemplateResult> Handle(UpsertAccountingTemplateCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Template;
        ValidateTemplate(dto);
        if (dto.Visibility == AccountingTemplateVisibility.Shared && !HasPermission(PermissionList.AccountingTemplatePermissions.Share))
            throw new BadRequestException("Shared template permission is required.");
        if (dto.Visibility == AccountingTemplateVisibility.Private && (!dto.CompanyId.HasValue || dto.CompanyId.Value == Guid.Empty))
            throw new BadRequestException("Company is required for private templates.");

        dto.Code = dto.Code.Trim().ToUpperInvariant();
        var scopedCompanyId = dto.Visibility == AccountingTemplateVisibility.Private ? dto.CompanyId : null;
        var duplicate = await dbContext.AccountingTemplates.AnyAsync(x => x.Id != dto.Id && x.Code == dto.Code && x.CompanyId == scopedCompanyId, cancellationToken);
        if (duplicate || string.Equals(dto.Code, SaudiAccountingTemplate.Code, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Template code already exists.");

        AccountingTemplate template;
        if (dto.Id == Guid.Empty)
        {
            template = AccountingTemplate.Create(dto, UserId);
            await dbContext.AccountingTemplates.AddAsync(template, cancellationToken);
        }
        else
        {
            template = await dbContext.AccountingTemplates
                .Include(x => x.Accounts)
                .Include(x => x.TaxCodes)
                .Include(x => x.PostingProfiles)
                .Include(x => x.Journals)
                .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken)
                ?? throw new NotFoundException("Accounting template", dto.Id);
            if (template.IsSystem)
                throw new BadRequestException("System templates cannot be edited.");
            template.Update(dto, UserId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpsertAccountingTemplateResult(template.Id);
    }

    public async Task<DeleteAccountingTemplateResult> Handle(DeleteAccountingTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await dbContext.AccountingTemplates.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException("Accounting template", command.Id);
        if (template.IsSystem)
            throw new BadRequestException("System templates cannot be deleted.");

        template.Deactivate(UserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteAccountingTemplateResult(command.Id);
    }

    public async Task<UpsertAccountingTemplateResult> Handle(CaptureAccountingTemplateCommand command, CancellationToken cancellationToken)
    {
        var request = command.Template;
        if (request.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (request.Visibility == AccountingTemplateVisibility.Shared && !HasPermission(PermissionList.AccountingTemplatePermissions.Share))
            throw new BadRequestException("Shared template permission is required.");

        var accounts = await dbContext.Accounts.AsNoTracking().Where(x => x.CompanyId == request.CompanyId).OrderBy(x => x.Code).ToListAsync(cancellationToken);
        if (!accounts.Any())
            throw new BadRequestException("Chart of accounts is required before capturing a template.");

        var keyByAccountId = accounts.ToDictionary(x => x.Id, x => string.IsNullOrWhiteSpace(x.TemplateKey) ? $"ACC_{x.Code}" : x.TemplateKey!);
        var dto = new AccountingTemplateDto
        {
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            CountryCode = request.CountryCode,
            CurrencyCode = request.CurrencyCode,
            Visibility = request.Visibility,
            CompanyId = request.Visibility == AccountingTemplateVisibility.Private ? request.CompanyId : null,
            IsActive = true,
            Accounts = accounts.Select(x => new AccountingTemplateAccountDto
            {
                TemplateKey = keyByAccountId[x.Id],
                Code = x.Code,
                Name = x.Name,
                NameEng = x.NameEng,
                Type = x.Type,
                NormalBalance = x.NormalBalance,
                Role = x.Role,
                ParentTemplateKey = x.ParentAccountId.HasValue && keyByAccountId.TryGetValue(x.ParentAccountId.Value, out var parentKey) ? parentKey : null,
                IsPostingAccount = x.IsPostingAccount
            }).ToList()
        };

        dto.TaxCodes = (await dbContext.TaxCodes.AsNoTracking().Where(x => x.CompanyId == request.CompanyId).OrderBy(x => x.Code).ToListAsync(cancellationToken))
            .Select(x => new AccountingTemplateTaxCodeDto
            {
                Code = x.Code,
                Name = x.Name,
                Rate = x.Rate,
                IsExempt = x.IsExempt,
                ZatcaCategoryCode = x.ZatcaCategoryCode,
                ExemptionReasonCode = x.ExemptionReasonCode,
                IsActive = x.IsActive
            }).ToList();

        dto.Journals = (await dbContext.AccountingJournals.AsNoTracking().Where(x => x.CompanyId == request.CompanyId).OrderBy(x => x.Code).ToListAsync(cancellationToken))
            .Select(x => new AccountingTemplateJournalDto
            {
                Code = x.Code,
                Name = x.Name,
                NameAr = x.NameAr,
                Type = x.Type,
                DefaultDebitAccountKey = x.DefaultDebitAccountId.HasValue && keyByAccountId.TryGetValue(x.DefaultDebitAccountId.Value, out var debitKey) ? debitKey : null,
                DefaultCreditAccountKey = x.DefaultCreditAccountId.HasValue && keyByAccountId.TryGetValue(x.DefaultCreditAccountId.Value, out var creditKey) ? creditKey : null,
                IsSystemJournal = x.IsSystemJournal,
                IsActive = x.IsActive
            }).ToList();

        dto.PostingProfiles = (await dbContext.PostingProfiles.AsNoTracking().Where(x => x.CompanyId == request.CompanyId).OrderBy(x => x.Type).ToListAsync(cancellationToken))
            .Where(x => keyByAccountId.ContainsKey(x.ReceivableAccountId)
                && keyByAccountId.ContainsKey(x.PayableAccountId)
                && keyByAccountId.ContainsKey(x.RevenueAccountId)
                && keyByAccountId.ContainsKey(x.ExpenseAccountId)
                && keyByAccountId.ContainsKey(x.OutputVatAccountId)
                && keyByAccountId.ContainsKey(x.InputVatAccountId)
                && keyByAccountId.ContainsKey(x.CashAccountId)
                && keyByAccountId.ContainsKey(x.BankAccountId))
            .Select(x => new AccountingTemplatePostingProfileDto
            {
                Type = x.Type,
                ReceivableAccountKey = keyByAccountId[x.ReceivableAccountId],
                PayableAccountKey = keyByAccountId[x.PayableAccountId],
                RevenueAccountKey = keyByAccountId[x.RevenueAccountId],
                ExpenseAccountKey = keyByAccountId[x.ExpenseAccountId],
                OutputVatAccountKey = keyByAccountId[x.OutputVatAccountId],
                InputVatAccountKey = keyByAccountId[x.InputVatAccountId],
                CashAccountKey = keyByAccountId[x.CashAccountId],
                BankAccountKey = keyByAccountId[x.BankAccountId],
                IsDefault = x.IsDefault
            }).ToList();

        return await Handle(new UpsertAccountingTemplateCommand(dto), cancellationToken);
    }

    public async Task<ApplyAccountingTemplateResult> Handle(ApplyAccountingTemplateCommand command, CancellationToken cancellationToken)
    {
        var setup = command.Setup;
        if (setup.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");

        var template = await ResolveTemplateAsync(setup, cancellationToken);
        ValidateTemplate(template);
        var codingSettings = await GetCodingSettingsDtoAsync(setup.CompanyId, cancellationToken);
        var generatedTemplateCodes = AccountCodePattern.GenerateTemplateCodes(template, codingSettings);
        var created = new ApplyAccountingTemplateResultDto();
        var existingAccounts = await dbContext.Accounts.Where(x => x.CompanyId == setup.CompanyId).ToListAsync(cancellationToken);
        var byTemplate = existingAccounts.Where(x => !string.IsNullOrWhiteSpace(x.TemplateKey)).ToDictionary(x => x.TemplateKey!, StringComparer.OrdinalIgnoreCase);
        var byCode = existingAccounts.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);
        var accountIdsByTemplate = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        foreach (var spec in template.Accounts.OrderBy(x => x.ParentTemplateKey is null ? 0 : 1).ThenBy(x => x.Code))
        {
            var generatedCode = generatedTemplateCodes[spec.TemplateKey];
            if (byTemplate.TryGetValue(spec.TemplateKey, out var existingByTemplate) || byCode.TryGetValue(generatedCode, out existingByTemplate))
            {
                accountIdsByTemplate[spec.TemplateKey] = existingByTemplate.Id;
                continue;
            }

            var account = Account.Create(new AccountDto
            {
                CompanyId = setup.CompanyId,
                Code = generatedCode,
                Name = spec.Name,
                NameEng = spec.NameEng,
                Type = spec.Type,
                NormalBalance = spec.NormalBalance,
                Role = spec.Role,
                TemplateKey = spec.TemplateKey,
                ParentAccountId = spec.ParentTemplateKey is not null && accountIdsByTemplate.TryGetValue(spec.ParentTemplateKey, out var parentId) ? parentId : null,
                IsPostingAccount = spec.IsPostingAccount,
                IsSystemAccount = template.IsSystem
            }, UserId);
            await dbContext.Accounts.AddAsync(account, cancellationToken);
            byTemplate[spec.TemplateKey] = account;
            byCode[generatedCode] = account;
            accountIdsByTemplate[spec.TemplateKey] = account.Id;
            created.AccountsCreated++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var roleAccounts = await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == setup.CompanyId && x.IsPostingAccount && x.Role != AccountRole.None)
            .GroupBy(x => x.Role)
            .ToDictionaryAsync(x => x.Key, x => x.OrderBy(a => a.Code).First().Id, cancellationToken);

        foreach (var taxCode in template.TaxCodes)
        {
            var exists = await dbContext.TaxCodes.AnyAsync(x => x.CompanyId == setup.CompanyId && x.Code == taxCode.Code, cancellationToken);
            if (exists)
                continue;
            await dbContext.TaxCodes.AddAsync(TaxCode.Create(new TaxCodeDto
            {
                CompanyId = setup.CompanyId,
                Code = taxCode.Code,
                Name = taxCode.Name,
                Rate = taxCode.Rate,
                IsExempt = taxCode.IsExempt,
                ZatcaCategoryCode = taxCode.ZatcaCategoryCode,
                ExemptionReasonCode = taxCode.ExemptionReasonCode,
                IsActive = taxCode.IsActive
            }, UserId), cancellationToken);
            created.TaxCodesCreated++;
        }

        foreach (var profile in template.PostingProfiles)
        {
            var exists = await dbContext.PostingProfiles.AnyAsync(x => x.CompanyId == setup.CompanyId && x.Type == profile.Type && x.IsDefault == profile.IsDefault, cancellationToken);
            if (exists)
                continue;
            await dbContext.PostingProfiles.AddAsync(PostingProfile.Create(new PostingProfileDto
            {
                CompanyId = setup.CompanyId,
                Type = profile.Type,
                ReceivableAccountId = ResolveTemplateAccountId(profile.ReceivableAccountKey, accountIdsByTemplate),
                PayableAccountId = ResolveTemplateAccountId(profile.PayableAccountKey, accountIdsByTemplate),
                RevenueAccountId = ResolveTemplateAccountId(profile.RevenueAccountKey, accountIdsByTemplate),
                ExpenseAccountId = ResolveTemplateAccountId(profile.ExpenseAccountKey, accountIdsByTemplate),
                OutputVatAccountId = ResolveTemplateAccountId(profile.OutputVatAccountKey, accountIdsByTemplate),
                InputVatAccountId = ResolveTemplateAccountId(profile.InputVatAccountKey, accountIdsByTemplate),
                CashAccountId = ResolveTemplateAccountId(profile.CashAccountKey, accountIdsByTemplate),
                BankAccountId = ResolveTemplateAccountId(profile.BankAccountKey, accountIdsByTemplate),
                IsDefault = profile.IsDefault
            }, UserId), cancellationToken);
            created.PostingProfilesCreated++;
        }

        if (setup.CreateDefaultJournals)
        {
            foreach (var journal in template.Journals)
            {
                var exists = await dbContext.AccountingJournals.AnyAsync(x => x.CompanyId == setup.CompanyId && x.Code == journal.Code, cancellationToken);
                if (exists)
                    continue;
                await dbContext.AccountingJournals.AddAsync(AccountingJournal.Create(new AccountingJournalDto
                {
                    CompanyId = setup.CompanyId,
                    Code = journal.Code,
                    Name = journal.Name,
                    NameAr = journal.NameAr,
                    Type = journal.Type,
                    DefaultDebitAccountId = ResolveOptionalTemplateAccountId(journal.DefaultDebitAccountKey, accountIdsByTemplate),
                    DefaultCreditAccountId = ResolveOptionalTemplateAccountId(journal.DefaultCreditAccountKey, accountIdsByTemplate),
                    IsSystemJournal = journal.IsSystemJournal,
                    IsActive = journal.IsActive
                }, UserId), cancellationToken);
                created.JournalsCreated++;
            }
        }

        await EnsureTemplateCompanyDefaultsAsync(setup.CompanyId, roleAccounts, cancellationToken);

        var fiscalStart = setup.FiscalYearStart == default
            ? new DateTime(DateTime.UtcNow.Year, template.FiscalYearStartMonth, Math.Min(template.FiscalYearStartDay, DateTime.DaysInMonth(DateTime.UtcNow.Year, template.FiscalYearStartMonth)))
            : setup.FiscalYearStart.Date;
        var fiscalEnd = fiscalStart.AddYears(1).AddDays(-1);
        var periodExists = await dbContext.FiscalPeriods.AnyAsync(x => x.CompanyId == setup.CompanyId && x.StartDate == fiscalStart && x.EndDate == fiscalEnd, cancellationToken);
        if (!periodExists)
        {
            await dbContext.FiscalPeriods.AddAsync(FiscalPeriod.Create(new FiscalPeriodDto
            {
                CompanyId = setup.CompanyId,
                Name = $"{fiscalStart:yyyy}",
                StartDate = fiscalStart,
                EndDate = fiscalEnd,
                Status = FiscalPeriodStatus.Open
            }, UserId), cancellationToken);
            created.FiscalPeriodsCreated++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var branches = await sender.Send(new GetCompanyBranchesForAccountingQuery(setup.CompanyId), cancellationToken);
        foreach (var branch in branches.Branches)
        {
            var ensured = await Handle(new EnsureBranchAccountingCommand(
                branch.CompanyId,
                branch.BranchId,
                branch.Code,
                branch.Name,
                branch.NameEng), cancellationToken);
            created.AccountsCreated += ensured.AccountGroupsCreated;
            created.JournalsCreated += ensured.JournalsCreated;
        }

        return new ApplyAccountingTemplateResult(created);
    }

    public async Task<EnsureBranchAccountingResult> Handle(EnsureBranchAccountingCommand command, CancellationToken cancellationToken)
    {
        if (command.CompanyId == Guid.Empty || command.BranchId == Guid.Empty)
            throw new BadRequestException("Company and branch are required.");

        var topGroups = await dbContext.Accounts
            .Where(x => x.CompanyId == command.CompanyId && x.ParentAccountId == null && !x.IsPostingAccount && x.IsActive)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
        if (!topGroups.Any())
            return new EnsureBranchAccountingResult(0, 0);

        var codingSettings = await GetCodingSettingsDtoAsync(command.CompanyId, cancellationToken);
        var createdGroups = 0;
        var branchName = string.IsNullOrWhiteSpace(command.BranchNameEng) ? command.BranchName : command.BranchNameEng;
        foreach (var group in topGroups)
        {
            var existingGroup = await dbContext.Accounts.FirstOrDefaultAsync(x =>
                x.CompanyId == command.CompanyId &&
                x.BranchId == command.BranchId &&
                x.ParentAccountId == group.Id &&
                !x.IsPostingAccount, cancellationToken);
            if (existingGroup is not null)
            {
                if (existingGroup.IsSystemAccount)
                    existingGroup.Rename($"{group.Name}-{command.BranchName}", $"{group.NameEng}-{branchName}", UserId);
                continue;
            }

            var code = await NextAccountCodeAsync(command.CompanyId, group.Code, codingSettings.ChildGroupSuffixLength, cancellationToken);
            await dbContext.Accounts.AddAsync(Account.Create(new AccountDto
            {
                CompanyId = command.CompanyId,
                BranchId = command.BranchId,
                Code = code,
                Name = $"{group.Name}-{command.BranchName}",
                NameEng = $"{group.NameEng}-{branchName}",
                Type = group.Type,
                NormalBalance = group.NormalBalance,
                Role = AccountRole.None,
                ParentAccountId = group.Id,
                IsPostingAccount = false,
                IsSystemAccount = true,
                IsActive = true
            }, UserId), cancellationToken);
            createdGroups++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var journalsCreated = 0;
        var branchGroups = await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == command.CompanyId && x.BranchId == command.BranchId && !x.IsPostingAccount && x.IsActive)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
        var assetGroup = branchGroups.FirstOrDefault(x => x.Type == AccountType.Asset);
        if (assetGroup is not null)
        {
            journalsCreated += await EnsureDefaultBranchJournalAsync(command.CompanyId, command.BranchId, command.BranchCode, command.BranchName, AccountingJournalType.Cash, assetGroup, codingSettings, cancellationToken);
            journalsCreated += await EnsureDefaultBranchJournalAsync(command.CompanyId, command.BranchId, command.BranchCode, command.BranchName, AccountingJournalType.Bank, assetGroup, codingSettings, cancellationToken);
            await EnsureDefaultBranchPaymentAccountsAsync(command.CompanyId, command.BranchId, command.BranchName, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new EnsureBranchAccountingResult(createdGroups, journalsCreated);
    }

    private const string ClosingSourceModule = "AccountingClosing";

    private string UserId => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

    private async Task EnsureCanAccessBranchAsync(Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (access.CanViewAllBranches)
            return;

        if (!branchId.HasValue || !access.BranchIds.Contains(branchId.Value))
            throw new UnauthorizedAccessException("User is not allowed to access this branch accounting data.");
    }

    private async Task EnsureOpenFiscalPeriodAsync(Guid companyId, DateTime postingDate, CancellationToken cancellationToken)
    {
        var date = (postingDate == default ? DateTime.UtcNow : postingDate).Date;
        var period = await dbContext.FiscalPeriods.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.StartDate <= date && x.EndDate >= date)
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (period is null)
            throw new BadRequestException($"No fiscal period exists for posting date {date:yyyy-MM-dd}.");

        if (period.Status != FiscalPeriodStatus.Open)
            throw new BadRequestException($"Fiscal period {period.Name} is {period.Status} and cannot accept postings.");
    }

    private async Task<Guid> ReverseJournalEntryAsync(Guid journalEntryId, CancellationToken cancellationToken, bool allowAccountingDocumentJournal = false)
    {
        var entry = await dbContext.JournalEntries.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == journalEntryId, cancellationToken)
            ?? throw new NotFoundException("Journal entry", journalEntryId);

        await EnsureCanAccessBranchAsync(entry.CompanyId, entry.BranchId, cancellationToken);
        var existingReversal = await dbContext.JournalEntries.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == entry.CompanyId
                && x.SourceModule == "AccountingReversal"
                && x.SourceDocumentId == entry.Id
                && x.Status == JournalEntryStatus.Posted, cancellationToken);
        if (existingReversal is not null)
        {
            entry.Reverse(UserId);
            return existingReversal.Id;
        }

        if (entry.Status != JournalEntryStatus.Posted)
            throw new BadRequestException("Only posted journal entries can be reversed.");

        if (!allowAccountingDocumentJournal
            && string.Equals(entry.SourceModule, "Accounting", StringComparison.OrdinalIgnoreCase)
            && entry.SourceDocumentId.HasValue)
            throw new BadRequestException("Reverse the accounting document instead of reversing its generated journal entry directly.");

        var reversalDate = DateTime.UtcNow.Date;
        await EnsureOpenFiscalPeriodAsync(entry.CompanyId, reversalDate, cancellationToken);
        var reversalNumber = await GenerateJournalNumberAsync(entry.CompanyId, reversalDate, cancellationToken);
        var reversalLines = entry.Lines
            .Select(line => new JournalEntryLineDto
            {
                AccountId = line.AccountId,
                Debit = line.Credit,
                Credit = line.Debit,
                Description = $"Reversal of {entry.Number}: {line.Description}".Trim()
            })
            .ToList();

        var reversal = JournalEntry.Create(
            entry.CompanyId,
            entry.BranchId,
            reversalNumber,
            reversalDate,
            "AccountingReversal",
            entry.Id,
            entry.Number,
            $"Reversal of {entry.Number}",
            reversalLines,
            UserId);
        reversal.Post(UserId);
        entry.Reverse(UserId);
        await dbContext.JournalEntries.AddAsync(reversal, cancellationToken);
        return reversal.Id;
    }

    private async Task<List<ClosingAccountBalance>> GetClosingAccountBalancesAsync(FiscalPeriod period, CancellationToken cancellationToken)
    {
        var entries = await dbContext.JournalEntries
            .Include(x => x.Lines)
            .AsNoTracking()
            .Where(x => x.CompanyId == period.CompanyId
                && x.Status == JournalEntryStatus.Posted
                && x.EntryDate >= period.StartDate
                && x.EntryDate <= period.EndDate
                && x.SourceModule != ClosingSourceModule)
            .ToListAsync(cancellationToken);

        var lineBalances = entries
            .SelectMany(x => x.Lines)
            .GroupBy(x => x.AccountId)
            .Select(x => new { AccountId = x.Key, Balance = x.Sum(line => line.Debit - line.Credit) })
            .Where(x => x.Balance != 0)
            .ToList();

        if (!lineBalances.Any())
            return [];

        var accountIds = lineBalances.Select(x => x.AccountId).ToList();
        var accounts = await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == period.CompanyId
                && accountIds.Contains(x.Id)
                && (x.Type == AccountType.Revenue || x.Type == AccountType.Expense))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var balances = new List<ClosingAccountBalance>();
        foreach (var balance in lineBalances)
        {
            if (!accounts.TryGetValue(balance.AccountId, out var account))
                continue;

            if (!account.IsActive || !account.IsPostingAccount)
                throw new BadRequestException($"Account {account.Code} must be active and postable before year-end close.");

            balances.Add(new ClosingAccountBalance(account.Id, account.Code, balance.Balance));
        }

        return balances;
    }

    private static List<JournalEntryLineDto> BuildYearEndClosingLines(List<ClosingAccountBalance> balances, Guid retainedEarningsAccountId)
    {
        var lines = new List<JournalEntryLineDto>();
        foreach (var balance in balances)
        {
            var amount = Math.Abs(decimal.Round(balance.Balance, 2));
            if (amount == 0)
                continue;

            lines.Add(new JournalEntryLineDto
            {
                AccountId = balance.AccountId,
                Debit = balance.Balance < 0 ? amount : 0,
                Credit = balance.Balance > 0 ? amount : 0,
                Description = $"Year-end close {balance.Code}"
            });
        }

        var totalDebit = lines.Sum(x => x.Debit);
        var totalCredit = lines.Sum(x => x.Credit);
        var difference = decimal.Round(totalDebit - totalCredit, 2);
        if (difference > 0)
        {
            lines.Add(new JournalEntryLineDto
            {
                AccountId = retainedEarningsAccountId,
                Credit = difference,
                Description = "Year-end close to retained earnings"
            });
        }
        else if (difference < 0)
        {
            lines.Add(new JournalEntryLineDto
            {
                AccountId = retainedEarningsAccountId,
                Debit = Math.Abs(difference),
                Description = "Year-end close to retained earnings"
            });
        }

        return lines;
    }

    private sealed record ClosingAccountBalance(Guid AccountId, string Code, decimal Balance);

    private bool HasPermission(string permission) => httpContextAccessor.HttpContext?.User.Claims.Any(c => c.Value == permission) == true;

    private async Task<AccountingTemplateDto> ResolveTemplateAsync(ApplyAccountingTemplateDto setup, CancellationToken cancellationToken)
    {
        if (setup.TemplateId.HasValue && setup.TemplateId.Value != Guid.Empty)
        {
            var template = await dbContext.AccountingTemplates
                .Include(x => x.Accounts)
                .Include(x => x.TaxCodes)
                .Include(x => x.PostingProfiles)
                .Include(x => x.Journals)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == setup.TemplateId.Value
                    && x.IsActive
                    && (x.Visibility == AccountingTemplateVisibility.Shared || x.CompanyId == setup.CompanyId), cancellationToken)
                ?? throw new NotFoundException("Accounting template", setup.TemplateId.Value);
            return template.ToDto();
        }

        if (string.Equals(setup.TemplateCode, SaudiAccountingTemplate.Code, StringComparison.OrdinalIgnoreCase))
            return SaudiAccountingTemplate.Template;

        var code = setup.TemplateCode.Trim().ToUpperInvariant();
        var custom = await dbContext.AccountingTemplates
            .Include(x => x.Accounts)
            .Include(x => x.TaxCodes)
            .Include(x => x.PostingProfiles)
            .Include(x => x.Journals)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code
                && x.IsActive
                && (x.Visibility == AccountingTemplateVisibility.Shared || x.CompanyId == setup.CompanyId), cancellationToken)
            ?? throw new BadRequestException("Accounting template is not available.");

        return custom.ToDto();
    }

    private static void ValidateTemplate(AccountingTemplateDto template)
    {
        if (string.IsNullOrWhiteSpace(template.Code))
            throw new BadRequestException("Template code is required.");
        if (string.IsNullOrWhiteSpace(template.Name))
            throw new BadRequestException("Template name is required.");
        if (!template.Accounts.Any())
            throw new BadRequestException("Template must include at least one account.");

        var accountKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var accountCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var account in template.Accounts)
        {
            if (string.IsNullOrWhiteSpace(account.TemplateKey) || string.IsNullOrWhiteSpace(account.Code) || string.IsNullOrWhiteSpace(account.Name) || string.IsNullOrWhiteSpace(account.NameEng))
                throw new BadRequestException("Every template account requires key, code, Arabic name, and English name.");
            if (string.IsNullOrWhiteSpace(account.ParentTemplateKey) && account.IsPostingAccount)
                throw new BadRequestException($"Top-level template account '{account.TemplateKey}' must be a group account.");
            if (!accountKeys.Add(account.TemplateKey.Trim()))
                throw new BadRequestException($"Duplicate account template key '{account.TemplateKey}'.");
            if (!accountCodes.Add(account.Code.Trim()))
                throw new BadRequestException($"Duplicate account code '{account.Code}'.");
        }

        foreach (var account in template.Accounts.Where(x => !string.IsNullOrWhiteSpace(x.ParentTemplateKey)))
        {
            if (!accountKeys.Contains(account.ParentTemplateKey!))
                throw new BadRequestException($"Parent account key '{account.ParentTemplateKey}' was not found.");
        }

        foreach (var profile in template.PostingProfiles)
        {
            foreach (var key in new[] { profile.ReceivableAccountKey, profile.PayableAccountKey, profile.RevenueAccountKey, profile.ExpenseAccountKey, profile.OutputVatAccountKey, profile.InputVatAccountKey, profile.CashAccountKey, profile.BankAccountKey })
            {
                if (!accountKeys.Contains(key))
                    throw new BadRequestException($"Posting profile account key '{key}' was not found.");
            }
        }

        foreach (var journal in template.Journals)
        {
            if (!string.IsNullOrWhiteSpace(journal.DefaultDebitAccountKey) && !accountKeys.Contains(journal.DefaultDebitAccountKey))
                throw new BadRequestException($"Journal debit account key '{journal.DefaultDebitAccountKey}' was not found.");
            if (!string.IsNullOrWhiteSpace(journal.DefaultCreditAccountKey) && !accountKeys.Contains(journal.DefaultCreditAccountKey))
                throw new BadRequestException($"Journal credit account key '{journal.DefaultCreditAccountKey}' was not found.");
        }
    }

    private static Guid ResolveTemplateAccountId(string key, IReadOnlyDictionary<string, Guid> accountIdsByTemplate)
    {
        if (!accountIdsByTemplate.TryGetValue(key, out var accountId))
            throw new BadRequestException($"Template account key '{key}' was not created.");
        return accountId;
    }

    private static Guid? ResolveOptionalTemplateAccountId(string? key, IReadOnlyDictionary<string, Guid> accountIdsByTemplate) =>
        string.IsNullOrWhiteSpace(key) ? null : ResolveTemplateAccountId(key, accountIdsByTemplate);

    private async Task<AccountCodingSettingsDto> GetCodingSettingsDtoAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var settings = await dbContext.AccountCodingSettings.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        return settings?.ToDto() ?? AccountCodingSettings.Default(companyId);
    }

    private static void ValidateCodingSettings(AccountCodingSettingsDto settings)
    {
        if (settings.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");

        var roots = new[] { settings.AssetRootCode, settings.LiabilityRootCode, settings.EquityRootCode, settings.RevenueRootCode, settings.ExpenseRootCode }
            .Select(x => x?.Trim() ?? string.Empty)
            .ToList();
        if (roots.Any(x => string.IsNullOrWhiteSpace(x) || !x.All(char.IsDigit)))
            throw new BadRequestException("Root account codes must contain digits only.");
        if (roots.Distinct(StringComparer.OrdinalIgnoreCase).Count() != roots.Count)
            throw new BadRequestException("Root account codes must be unique.");
        if (settings.ChildGroupSuffixLength is < 1 or > 8 || settings.ChildLedgerSuffixLength is < 1 or > 8)
            throw new BadRequestException("Account suffix lengths must be between 1 and 8 digits.");
    }

    private async Task<string> GenerateAccountCodeAsync(AccountDto dto, Guid? editingAccountId, AccountCodingSettingsDto settings, CancellationToken cancellationToken)
    {
        if (!dto.ParentAccountId.HasValue || dto.ParentAccountId.Value == Guid.Empty)
        {
            if (dto.IsPostingAccount)
                throw new BadRequestException("Top-level accounts must be group accounts.");

            return AccountCodePattern.RootCode(settings, dto.Type);
        }

        var parent = await dbContext.Accounts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == dto.CompanyId && x.Id == dto.ParentAccountId.Value && x.IsActive, cancellationToken)
            ?? throw new BadRequestException("Parent account must belong to the company and be active.");
        var suffixLength = dto.IsPostingAccount ? settings.ChildLedgerSuffixLength : settings.ChildGroupSuffixLength;
        return await NextAccountCodeAsync(dto.CompanyId, parent.Code, suffixLength, cancellationToken, editingAccountId);
    }

    private static bool AccountStructureChanged(AccountDto dto, Account account) =>
        dto.ParentAccountId != account.ParentAccountId
        || dto.BranchId != account.BranchId
        || dto.Type != account.Type
        || dto.IsPostingAccount != account.IsPostingAccount;

    private async Task<AccountRenumberPreviewDto> BuildRenumberPreviewAsync(AccountCodingSettingsDto requestedSettings, CancellationToken cancellationToken)
    {
        var accounts = await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == requestedSettings.CompanyId)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
        var hasPostingActivity = await HasPostingActivityAsync(requestedSettings.CompanyId, cancellationToken);
        var plannedCodes = AccountCodePattern.PlanRenumberCodes(
            accounts.Select(x => new AccountCodePattern.AccountNode(x.Id, x.ParentAccountId, x.Code, x.Type, x.IsPostingAccount)).ToList(),
            requestedSettings);

        var lines = accounts
            .Where(x => !string.Equals(x.Code, plannedCodes[x.Id], StringComparison.OrdinalIgnoreCase))
            .Select(x => new AccountRenumberPreviewLineDto
            {
                AccountId = x.Id,
                Name = x.Name,
                NameEng = x.NameEng,
                Type = x.Type,
                OldCode = x.Code,
                NewCode = plannedCodes[x.Id]
            })
            .OrderBy(x => x.OldCode)
            .ToList();

        var errors = new List<string>();
        if (hasPostingActivity && lines.Any())
            errors.Add("Posted accounting activity exists; account code renumbering is locked.");

        var duplicates = plannedCodes.Values
            .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();
        if (duplicates.Any())
            errors.Add($"Generated account codes would conflict: {string.Join(", ", duplicates)}.");

        return new AccountRenumberPreviewDto
        {
            CompanyId = requestedSettings.CompanyId,
            HasPostingActivity = hasPostingActivity,
            CanApply = !errors.Any(),
            Errors = errors,
            Lines = lines
        };

    }

    private static string TemporaryRenumberCode(Guid accountId) => $"__REN-{accountId:N}";

    private async Task<bool> HasPostingActivityAsync(Guid companyId, CancellationToken cancellationToken) =>
        await dbContext.JournalEntries.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && x.Status == JournalEntryStatus.Posted, cancellationToken)
        || await dbContext.AccountingDocuments.AsNoTracking().AnyAsync(x => x.CompanyId == companyId && x.Status == AccountingDocumentStatus.Posted, cancellationToken);

    private async Task ValidateAccountAsync(AccountDto dto, Guid? editingAccountId, AccountCodingSettingsDto settings, CancellationToken cancellationToken)
    {
        var code = dto.Code.Trim();
        var duplicateCodeExists = await dbContext.Accounts.IgnoreQueryFilters().AsNoTracking()
            .AnyAsync(x => x.CompanyId == dto.CompanyId && x.Code == code && (!editingAccountId.HasValue || x.Id != editingAccountId.Value), cancellationToken);
        if (duplicateCodeExists)
            throw new BadRequestException("Account code already exists for this company.");

        if (!dto.ParentAccountId.HasValue || dto.ParentAccountId.Value == Guid.Empty)
        {
            if (dto.BranchId.HasValue)
                throw new BadRequestException("Branch accounts must be created under a branch account group.");
            return;
        }

        var parent = await dbContext.Accounts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == dto.CompanyId && x.Id == dto.ParentAccountId.Value && x.IsActive, cancellationToken);
        if (parent is null)
            throw new BadRequestException("Parent account must belong to the company and be active.");
        if (parent.IsPostingAccount)
            throw new BadRequestException("Posting accounts cannot have child accounts.");
        if (dto.BranchId.HasValue && parent.BranchId.HasValue && parent.BranchId != dto.BranchId)
            throw new BadRequestException("Branch account must stay inside the same branch account subtree.");
        if (!dto.BranchId.HasValue && parent.BranchId.HasValue)
            throw new BadRequestException("Company-wide accounts cannot be created under a branch account group.");

        ValidateChildAccountCode(dto, parent.Code, settings);

        if (editingAccountId.HasValue)
        {
            if (dto.ParentAccountId.Value == editingAccountId.Value)
                throw new BadRequestException("Account cannot be its own parent.");

            await EnsureParentIsNotDescendantAsync(dto.CompanyId, editingAccountId.Value, dto.ParentAccountId.Value, cancellationToken);
        }
    }

    private async Task EnsureParentIsNotDescendantAsync(Guid companyId, Guid accountId, Guid parentAccountId, CancellationToken cancellationToken)
    {
        var links = await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .Select(x => new { x.Id, x.ParentAccountId })
            .ToListAsync(cancellationToken);
        var parentsByAccountId = links.ToDictionary(x => x.Id, x => x.ParentAccountId);
        var currentId = parentAccountId;
        var visited = new HashSet<Guid>();

        while (visited.Add(currentId) && parentsByAccountId.TryGetValue(currentId, out var nextParentId))
        {
            if (currentId == accountId)
                throw new BadRequestException("Parent account cannot be a descendant of the account.");

            if (!nextParentId.HasValue || nextParentId.Value == Guid.Empty)
                return;

            currentId = nextParentId.Value;
        }
    }

    private static void ValidateChildAccountCode(AccountDto dto, string parentCode, AccountCodingSettingsDto settings)
    {
        var suffixLength = dto.IsPostingAccount ? settings.ChildLedgerSuffixLength : settings.ChildGroupSuffixLength;
        var accountKind = dto.IsPostingAccount ? "ledger" : "group";
        var code = dto.Code.Trim();

        if (!code.StartsWith(parentCode, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException($"Child {accountKind} account code must start with parent account code '{parentCode}'.");

        var suffix = code[parentCode.Length..];
        if (suffix.Length != suffixLength || !suffix.All(char.IsDigit) || int.Parse(suffix) <= 0)
            throw new BadRequestException($"Child {accountKind} account code must use parent code plus {suffixLength} digits greater than zero.");
    }

    private async Task<Guid> ResolveOrCreateLedgerAccountAsync(Guid companyId, Guid? branchId, Guid? accountId, string displayName, AccountRole role, AccountType type, NormalBalance balance, CancellationToken cancellationToken)
    {
        if (accountId.HasValue && accountId.Value != Guid.Empty)
        {
            await EnsurePostingAccountsAsync(companyId, branchId, [accountId.Value], cancellationToken);
            return accountId.Value;
        }

        var existing = await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.BranchId == branchId && x.Role == role && x.IsPostingAccount && x.IsActive)
            .OrderBy(x => x.Code)
            .FirstOrDefaultAsync(x => x.NameEng == displayName || x.Name == displayName, cancellationToken);
        if (existing is not null)
            return existing.Id;

        var parent = await ResolveDefaultAssetLedgerParentAsync(companyId, branchId, cancellationToken);
        var codingSettings = await GetCodingSettingsDtoAsync(companyId, cancellationToken);
        var nextCode = await NextAccountCodeAsync(companyId, parent?.Code ?? AccountCodePattern.RootCode(codingSettings, AccountType.Asset), codingSettings.ChildLedgerSuffixLength, cancellationToken);
        var account = Account.Create(new AccountDto
        {
            CompanyId = companyId,
            BranchId = branchId,
            Code = nextCode,
            Name = displayName,
            NameEng = displayName,
            Type = type,
            NormalBalance = balance,
            Role = role,
            ParentAccountId = parent?.Id,
            IsPostingAccount = true,
            IsSystemAccount = false
        }, UserId);
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return account.Id;
    }

    private async Task<Guid> ResolveOrCreateJournalAsync(Guid companyId, Guid? branchId, Guid? journalId, string displayName, AccountingJournalType type, Guid ledgerAccountId, CancellationToken cancellationToken)
    {
        if (journalId.HasValue && journalId.Value != Guid.Empty)
        {
            var exists = await dbContext.AccountingJournals.AnyAsync(x => x.CompanyId == companyId && x.BranchId == branchId && x.Id == journalId.Value && x.IsActive, cancellationToken);
            if (!exists)
                throw new BadRequestException("Selected journal must belong to the company and be active.");
            return journalId.Value;
        }

        var codePrefix = type == AccountingJournalType.Bank ? "BNK" : "CSH";
        var code = await NextJournalCodeAsync(companyId, codePrefix, cancellationToken);
        var journal = AccountingJournal.Create(new AccountingJournalDto
        {
            CompanyId = companyId,
            BranchId = branchId,
            Code = code,
            Name = displayName,
            NameAr = displayName,
            Type = type,
            DefaultDebitAccountId = ledgerAccountId,
            DefaultCreditAccountId = ledgerAccountId,
            IsSystemJournal = false,
            IsActive = true
        }, UserId);
        await dbContext.AccountingJournals.AddAsync(journal, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return journal.Id;
    }

    private async Task<int> EnsureDefaultBranchJournalAsync(Guid companyId, Guid branchId, string branchCode, string branchName, AccountingJournalType type, Account assetGroup, AccountCodingSettingsDto codingSettings, CancellationToken cancellationToken)
    {
        var existing = await dbContext.AccountingJournals.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.BranchId == branchId && x.Type == type, cancellationToken);
        if (existing is not null)
        {
            if (existing.IsSystemJournal)
                existing.Rename($"{branchName} {(type == AccountingJournalType.Bank ? "Bank" : "Cash")}", $"{branchName} {(type == AccountingJournalType.Bank ? "Bank" : "Cash")}", UserId);
            return 0;
        }

        var prefix = $"{(type == AccountingJournalType.Bank ? "BNK" : "CSH")}-{NormalizeBranchCode(branchCode)}";
        var ledgerCode = await NextAccountCodeAsync(companyId, assetGroup.Code, codingSettings.ChildLedgerSuffixLength, cancellationToken);
        var displayName = $"{branchName} {(type == AccountingJournalType.Bank ? "Bank" : "Cash")}";
        var ledger = Account.Create(new AccountDto
        {
            CompanyId = companyId,
            BranchId = branchId,
            Code = ledgerCode,
            Name = displayName,
            NameEng = displayName,
            Type = AccountType.Asset,
            NormalBalance = NormalBalance.Debit,
            Role = type == AccountingJournalType.Bank ? AccountRole.Bank : AccountRole.Cash,
            ParentAccountId = assetGroup.Id,
            IsPostingAccount = true,
            IsSystemAccount = true,
            IsActive = true
        }, UserId);
        await dbContext.Accounts.AddAsync(ledger, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var journal = AccountingJournal.Create(new AccountingJournalDto
        {
            CompanyId = companyId,
            BranchId = branchId,
            Code = await NextJournalCodeAsync(companyId, prefix, cancellationToken),
            Name = displayName,
            NameAr = displayName,
            Type = type,
            DefaultDebitAccountId = ledger.Id,
            DefaultCreditAccountId = ledger.Id,
            IsSystemJournal = true,
            IsActive = true
        }, UserId);
        await dbContext.AccountingJournals.AddAsync(journal, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return 1;
    }

    private async Task EnsureDefaultBranchPaymentAccountsAsync(Guid companyId, Guid branchId, string branchName, CancellationToken cancellationToken)
    {
        var cashJournal = await dbContext.AccountingJournals.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.BranchId == branchId && x.Type == AccountingJournalType.Cash && x.IsActive, cancellationToken)
            ?? throw new BadRequestException($"Branch {branchName} cash journal was not found.");
        var bankJournal = await dbContext.AccountingJournals.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.BranchId == branchId && x.Type == AccountingJournalType.Bank && x.IsActive, cancellationToken)
            ?? throw new BadRequestException($"Branch {branchName} bank journal was not found.");

        var cashLedgerId = cashJournal.DefaultDebitAccountId ?? cashJournal.DefaultCreditAccountId
            ?? throw new BadRequestException($"Branch {branchName} cash journal must have a default ledger account.");
        var bankLedgerId = bankJournal.DefaultDebitAccountId ?? bankJournal.DefaultCreditAccountId
            ?? throw new BadRequestException($"Branch {branchName} bank journal must have a default ledger account.");

        var cashAccounts = await dbContext.CashAccounts
            .Where(x => x.CompanyId == companyId && x.BranchId == branchId && x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
        if (cashAccounts.Any())
        {
            var defaultCash = cashAccounts.FirstOrDefault(x => x.IsDefault) ?? cashAccounts.First();
            foreach (var account in cashAccounts.Where(x => x.Id != defaultCash.Id && x.IsDefault))
                account.SetDefault(false, UserId);
            if (!defaultCash.IsDefault)
                defaultCash.SetDefault(true, UserId);
        }
        else
        {
            await dbContext.CashAccounts.AddAsync(CashAccount.Create(new CashAccountDto
            {
                CompanyId = companyId,
                BranchId = branchId,
                DisplayName = $"{branchName} Cash",
                CurrencyCode = "SAR",
                LedgerAccountId = cashLedgerId,
                JournalId = cashJournal.Id,
                IsDefault = true,
                IsActive = true
            }, cashLedgerId, cashJournal.Id, UserId), cancellationToken);
        }

        var bankAccounts = await dbContext.BankAccounts
            .Where(x => x.CompanyId == companyId && x.BranchId == branchId && x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
        if (bankAccounts.Any())
        {
            var defaultBank = bankAccounts.FirstOrDefault(x => x.IsDefault) ?? bankAccounts.First();
            foreach (var account in bankAccounts.Where(x => x.Id != defaultBank.Id && x.IsDefault))
                account.SetDefault(false, UserId);
            if (!defaultBank.IsDefault)
                defaultBank.SetDefault(true, UserId);
        }
        else
        {
            await dbContext.BankAccounts.AddAsync(BankAccount.Create(new BankAccountDto
            {
                CompanyId = companyId,
                BranchId = branchId,
                DisplayName = $"{branchName} Bank",
                BankName = "Company Bank",
                CurrencyCode = "SAR",
                LedgerAccountId = bankLedgerId,
                JournalId = bankJournal.Id,
                IsDefault = true,
                IsActive = true
            }, bankLedgerId, bankJournal.Id, UserId), cancellationToken);
        }
    }

    private static string NormalizeBranchCode(string branchCode)
    {
        var normalized = new string((branchCode ?? string.Empty).Where(char.IsLetterOrDigit).Take(8).ToArray()).ToUpperInvariant();
        return string.IsNullOrWhiteSpace(normalized) ? "BR" : normalized;
    }

    private async Task<Account?> ResolveDefaultAssetLedgerParentAsync(Guid companyId, Guid? branchId, CancellationToken cancellationToken) =>
        await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.BranchId == branchId && x.IsActive && !x.IsPostingAccount)
            .OrderByDescending(x => x.TemplateKey == "SA_CURRENT_ASSETS")
            .ThenByDescending(x => x.Code == "100001")
            .ThenByDescending(x => x.Type == AccountType.Asset)
            .ThenBy(x => x.Code)
            .FirstOrDefaultAsync(x => branchId.HasValue
                ? x.Type == AccountType.Asset
                : x.TemplateKey == "SA_CURRENT_ASSETS" || x.Code == "100001", cancellationToken);

    private async Task<string> NextAccountCodeAsync(Guid companyId, string prefix, int suffixLength, CancellationToken cancellationToken, Guid? editingAccountId = null)
    {
        var codes = await dbContext.Accounts.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && x.Code.StartsWith(prefix)
                && x.Code.Length == prefix.Length + suffixLength
                && (!editingAccountId.HasValue || x.Id != editingAccountId.Value))
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);
        var next = codes.Select(x => int.TryParse(x[prefix.Length..], out var value) ? value : 0).DefaultIfEmpty(0).Max() + 1;
        return $"{prefix}{next.ToString($"D{suffixLength}")}";
    }

    private async Task<string> NextJournalCodeAsync(Guid companyId, string prefix, CancellationToken cancellationToken)
    {
        var codes = await dbContext.AccountingJournals.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.Code.StartsWith(prefix))
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);
        var next = codes.Select(x => int.TryParse(x[prefix.Length..], out var value) ? value : 0).DefaultIfEmpty(0).Max() + 1;
        return $"{prefix}{next:D2}";
    }

    private async Task UpsertDefaultPaymentAccountAsync(Guid companyId, Guid accountId, AccountRole role, CancellationToken cancellationToken)
    {
        var settings = await dbContext.CompanyAccountingSettings.FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        var dto = settings?.ToDto() ?? new CompanyAccountingSettingsDto { CompanyId = companyId };
        if (role == AccountRole.Bank)
            dto.BankAccountId = accountId;
        if (role == AccountRole.Cash)
            dto.CashAccountId = accountId;

        if (settings is null)
            await dbContext.CompanyAccountingSettings.AddAsync(CompanyAccountingSettings.Upsert(dto, UserId), cancellationToken);
        else
            settings.Update(dto, UserId);
    }

    private async Task EnsureOptionalPostingAccountsAsync(Guid companyId, IEnumerable<Guid?> accountIds, CancellationToken cancellationToken) =>
        await EnsurePostingAccountsAsync(companyId, null, accountIds.Where(x => x.HasValue).Select(x => x!.Value), cancellationToken);

    private async Task<List<JournalEntryLineDto>> ResolveJournalLinesAsync(Guid companyId, Guid? branchId, IEnumerable<JournalEntryLineDto> lines, CancellationToken cancellationToken)
    {
        var sourceLines = lines.ToList();
        var roles = sourceLines.Where(x => x.AccountId == Guid.Empty && x.AccountRole != AccountRole.None).Select(x => x.AccountRole).Distinct().ToList();
        var resolved = new Dictionary<AccountRole, Guid>();

        if (roles.Count > 0)
        {
            var settings = await dbContext.CompanyAccountingSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
            foreach (var role in roles)
            {
                var settingsAccountId = ResolveSettingsAccountId(settings, role);
                if (settingsAccountId.HasValue
                    && settingsAccountId.Value != Guid.Empty
                    && await IsPostingAccountAvailableForBranchAsync(companyId, branchId, settingsAccountId.Value, cancellationToken))
                {
                    resolved[role] = settingsAccountId.Value;
                }
            }

            var unresolvedRoles = roles.Where(x => !resolved.ContainsKey(x)).ToList();
            if (unresolvedRoles.Count > 0)
            {
                var accountRoles = await dbContext.Accounts.AsNoTracking()
                    .Where(x => x.CompanyId == companyId
                        && x.IsActive
                        && x.IsPostingAccount
                        && unresolvedRoles.Contains(x.Role)
                        && (x.BranchId == branchId || x.BranchId == null))
                    .GroupBy(x => x.Role)
                    .Select(x => new { Role = x.Key, Id = x.OrderByDescending(a => a.BranchId == branchId).ThenBy(a => a.Code).Select(a => a.Id).First() })
                    .ToListAsync(cancellationToken);

                foreach (var account in accountRoles)
                    resolved[account.Role] = account.Id;
            }
        }

        return sourceLines.Select(line =>
        {
            if (line.AccountId != Guid.Empty)
                return line;

            if (!resolved.TryGetValue(line.AccountRole, out var accountId))
                throw new BadRequestException($"No posting account is configured for role '{line.AccountRole}'.");

            return new JournalEntryLineDto
            {
                Id = line.Id,
                AccountId = accountId,
                AccountRole = line.AccountRole,
                Debit = line.Debit,
                Credit = line.Credit,
                Description = line.Description
            };
        }).ToList();
    }

    private static Guid? ResolveSettingsAccountId(CompanyAccountingSettings? settings, AccountRole role)
    {
        if (settings is null)
            return null;

        var dto = settings.ToDto();
        return role switch
        {
            AccountRole.Receivable => dto.ReceivableAccountId,
            AccountRole.Payable => dto.PayableAccountId,
            AccountRole.Revenue => dto.RevenueAccountId,
            AccountRole.Expense => dto.ExpenseAccountId,
            AccountRole.Cogs => dto.CogsAccountId,
            AccountRole.Inventory => dto.InventoryAccountId,
            AccountRole.InputVat => dto.InputVatAccountId,
            AccountRole.OutputVat => dto.OutputVatAccountId,
            AccountRole.VatSettlement => dto.VatSettlementAccountId,
            AccountRole.Cash => dto.CashAccountId,
            AccountRole.Bank => dto.BankAccountId,
            AccountRole.Rounding => dto.RoundingAccountId,
            AccountRole.Suspense => dto.SuspenseAccountId,
            AccountRole.RetainedEarnings => dto.RetainedEarningsAccountId,
            _ => null
        };
    }

    private static IEnumerable<Guid?> SettingsAccountIds(CompanyAccountingSettingsDto settings) =>
    [
        settings.ReceivableAccountId,
        settings.PayableAccountId,
        settings.RevenueAccountId,
        settings.ExpenseAccountId,
        settings.CogsAccountId,
        settings.InventoryAccountId,
        settings.InputVatAccountId,
        settings.OutputVatAccountId,
        settings.VatSettlementAccountId,
        settings.CashAccountId,
        settings.BankAccountId,
        settings.RoundingAccountId,
        settings.SuspenseAccountId,
        settings.RetainedEarningsAccountId
    ];

    private async Task EnsurePostingProfileAccountsAsync(PostingProfileDto profile, CancellationToken cancellationToken) =>
        await EnsurePostingAccountsAsync(profile.CompanyId, null, [
            profile.ReceivableAccountId,
            profile.PayableAccountId,
            profile.RevenueAccountId,
            profile.ExpenseAccountId,
            profile.OutputVatAccountId,
            profile.InputVatAccountId,
            profile.CashAccountId,
            profile.BankAccountId
        ], cancellationToken);

    private async Task EnsurePostingAccountsAsync(Guid companyId, Guid? branchId, IEnumerable<Guid> accountIds, CancellationToken cancellationToken)
    {
        var ids = accountIds.Where(x => x != Guid.Empty).Distinct().ToList();
        var valid = await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == companyId
                && ids.Contains(x.Id)
                && x.IsActive
                && x.IsPostingAccount
                && (x.BranchId == branchId || x.BranchId == null))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (valid.Count != ids.Count)
            throw new BadRequestException("Only active ledger/posting accounts for the selected branch can be used for posting.");
    }

    private async Task<bool> IsPostingAccountAvailableForBranchAsync(Guid companyId, Guid? branchId, Guid accountId, CancellationToken cancellationToken) =>
        await dbContext.Accounts.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId
                && x.Id == accountId
                && x.IsActive
                && x.IsPostingAccount
                && (x.BranchId == branchId || x.BranchId == null), cancellationToken);

    private static List<PostingProfileDto> BuildDefaultProfiles(Guid companyId, IReadOnlyDictionary<AccountRole, Guid> accounts) =>
    [
        Profile(companyId, PostingProfileType.Sales, accounts[AccountRole.Receivable], accounts[AccountRole.Payable], accounts[AccountRole.Revenue], accounts[AccountRole.Expense], accounts[AccountRole.OutputVat], accounts[AccountRole.InputVat], accounts[AccountRole.Cash], accounts[AccountRole.Bank]),
        Profile(companyId, PostingProfileType.Purchases, accounts[AccountRole.Receivable], accounts[AccountRole.Payable], accounts[AccountRole.Revenue], accounts[AccountRole.Expense], accounts[AccountRole.OutputVat], accounts[AccountRole.InputVat], accounts[AccountRole.Cash], accounts[AccountRole.Bank]),
        Profile(companyId, PostingProfileType.CustomerReceipt, accounts[AccountRole.Receivable], accounts[AccountRole.Payable], accounts[AccountRole.Revenue], accounts[AccountRole.Expense], accounts[AccountRole.OutputVat], accounts[AccountRole.InputVat], accounts[AccountRole.Cash], accounts[AccountRole.Bank]),
        Profile(companyId, PostingProfileType.SupplierPayment, accounts[AccountRole.Receivable], accounts[AccountRole.Payable], accounts[AccountRole.Revenue], accounts[AccountRole.Expense], accounts[AccountRole.OutputVat], accounts[AccountRole.InputVat], accounts[AccountRole.Cash], accounts[AccountRole.Bank])
    ];

    private static PostingProfileDto Profile(Guid companyId, PostingProfileType type, Guid ar, Guid ap, Guid revenue, Guid expense, Guid outputVat, Guid inputVat, Guid cash, Guid bank) => new()
    {
        CompanyId = companyId,
        Type = type,
        ReceivableAccountId = ar,
        PayableAccountId = ap,
        RevenueAccountId = revenue,
        ExpenseAccountId = expense,
        OutputVatAccountId = outputVat,
        InputVatAccountId = inputVat,
        CashAccountId = cash,
        BankAccountId = bank,
        IsDefault = true
    };

    private async Task EnsureTemplateCompanyDefaultsAsync(Guid companyId, IReadOnlyDictionary<AccountRole, Guid> accounts, CancellationToken cancellationToken)
    {
        var settings = await dbContext.CompanyAccountingSettings.FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        var dto = settings?.ToDto() ?? new CompanyAccountingSettingsDto { CompanyId = companyId };
        dto.ReceivableAccountId ??= accounts.GetValueOrDefault(AccountRole.Receivable);
        dto.PayableAccountId ??= accounts.GetValueOrDefault(AccountRole.Payable);
        dto.RevenueAccountId ??= accounts.GetValueOrDefault(AccountRole.Revenue);
        dto.ExpenseAccountId ??= accounts.GetValueOrDefault(AccountRole.Expense);
        dto.CogsAccountId ??= accounts.GetValueOrDefault(AccountRole.Cogs);
        dto.InventoryAccountId ??= accounts.GetValueOrDefault(AccountRole.Inventory);
        dto.InputVatAccountId ??= accounts.GetValueOrDefault(AccountRole.InputVat);
        dto.OutputVatAccountId ??= accounts.GetValueOrDefault(AccountRole.OutputVat);
        dto.VatSettlementAccountId ??= accounts.GetValueOrDefault(AccountRole.VatSettlement);
        dto.CashAccountId ??= accounts.GetValueOrDefault(AccountRole.Cash);
        dto.BankAccountId ??= accounts.GetValueOrDefault(AccountRole.Bank);
        dto.RoundingAccountId ??= accounts.GetValueOrDefault(AccountRole.Rounding);
        dto.SuspenseAccountId ??= accounts.GetValueOrDefault(AccountRole.Suspense);
        dto.RetainedEarningsAccountId ??= accounts.GetValueOrDefault(AccountRole.RetainedEarnings);

        if (settings is null)
            await dbContext.CompanyAccountingSettings.AddAsync(CompanyAccountingSettings.Upsert(dto, UserId), cancellationToken);
        else
            settings.Update(dto, UserId);

        var cashJournal = await dbContext.AccountingJournals.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Type == AccountingJournalType.Cash, cancellationToken);
        var bankJournal = await dbContext.AccountingJournals.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Type == AccountingJournalType.Bank, cancellationToken);
        if (!await dbContext.CashAccounts.AnyAsync(x => x.CompanyId == companyId && x.IsDefault, cancellationToken) && dto.CashAccountId.HasValue && cashJournal is not null)
        {
            await dbContext.CashAccounts.AddAsync(CashAccount.Create(new CashAccountDto
            {
                CompanyId = companyId,
                DisplayName = "Main Cash",
                LedgerAccountId = dto.CashAccountId,
                JournalId = cashJournal.Id,
                IsDefault = true,
                IsActive = true
            }, dto.CashAccountId.Value, cashJournal.Id, UserId), cancellationToken);
        }

        if (!await dbContext.BankAccounts.AnyAsync(x => x.CompanyId == companyId && x.IsDefault, cancellationToken) && dto.BankAccountId.HasValue && bankJournal is not null)
        {
            await dbContext.BankAccounts.AddAsync(BankAccount.Create(new BankAccountDto
            {
                CompanyId = companyId,
                DisplayName = "Main Bank",
                BankName = "Company Bank",
                LedgerAccountId = dto.BankAccountId,
                JournalId = bankJournal.Id,
                IsDefault = true,
                IsActive = true
            }, dto.BankAccountId.Value, bankJournal.Id, UserId), cancellationToken);
        }
    }

    private async Task<string> GenerateDocumentNumberAsync(Guid companyId, AccountingDocumentType type, DateTime date, CancellationToken cancellationToken)
    {
        var prefix = $"{DocumentPrefix(type)}-{(date == default ? DateTime.UtcNow : date):yyMM}-";
        var numbers = await dbContext.AccountingDocuments.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.Type == type && x.Number.StartsWith(prefix))
            .Select(x => x.Number)
            .ToListAsync(cancellationToken);
        var next = numbers.Select(x => int.TryParse(x[prefix.Length..], out var value) ? value : 0).DefaultIfEmpty(0).Max() + 1;
        return $"{prefix}{next:D5}";
    }

    private async Task<string> GenerateJournalNumberAsync(Guid companyId, DateTime date, CancellationToken cancellationToken)
    {
        var prefix = $"JE-{(date == default ? DateTime.UtcNow : date):yyMM}-";
        var numbers = await dbContext.JournalEntries.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.Number.StartsWith(prefix))
            .Select(x => x.Number)
            .ToListAsync(cancellationToken);
        var next = numbers.Select(x => int.TryParse(x[prefix.Length..], out var value) ? value : 0).DefaultIfEmpty(0).Max() + 1;
        return $"{prefix}{next:D5}";
    }

    private static string DocumentPrefix(AccountingDocumentType type) => type switch
    {
        AccountingDocumentType.SalesInvoice => "SI",
        AccountingDocumentType.SalesCreditNote => "SCN",
        AccountingDocumentType.SalesDebitNote => "SDN",
        AccountingDocumentType.SupplierInvoice => "PINV",
        AccountingDocumentType.CustomerReceipt => "RCT",
        AccountingDocumentType.SupplierPayment => "PAY",
        AccountingDocumentType.SupplierCreditNote => "PCN",
        _ => "DOC"
    };

    private async Task<PostingProfile> ResolvePostingProfileAsync(Guid companyId, AccountingDocumentType type, CancellationToken cancellationToken)
    {
        var profileType = type switch
        {
            AccountingDocumentType.SupplierInvoice => PostingProfileType.Purchases,
            AccountingDocumentType.SupplierCreditNote => PostingProfileType.Purchases,
            AccountingDocumentType.CustomerReceipt => PostingProfileType.CustomerReceipt,
            AccountingDocumentType.SupplierPayment => PostingProfileType.SupplierPayment,
            _ => PostingProfileType.Sales
        };

        return await dbContext.PostingProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Type == profileType && x.IsDefault, cancellationToken)
            ?? throw new BadRequestException($"Default posting profile '{profileType}' is required before posting.");
    }

    private static List<JournalEntryLineDto> BuildJournalLines(AccountingDocument document, PostingProfile profile, CompanyAccountingSettings? settings) =>
        NonZeroLines(document.Type switch
        {
            AccountingDocumentType.SupplierInvoice =>
            [
                new() { AccountId = profile.ExpenseAccountId, Debit = document.Subtotal, Description = document.Number },
                new() { AccountId = profile.InputVatAccountId, Debit = document.TaxAmount, Description = document.Number },
                new() { AccountId = profile.PayableAccountId, Credit = document.TotalAmount, Description = document.Number }
            ],
            AccountingDocumentType.SupplierCreditNote =>
            [
                new() { AccountId = profile.PayableAccountId, Debit = document.TotalAmount, Description = document.Number },
                new() { AccountId = profile.ExpenseAccountId, Credit = document.Subtotal, Description = document.Number },
                new() { AccountId = profile.InputVatAccountId, Credit = document.TaxAmount, Description = document.Number }
            ],
            AccountingDocumentType.CustomerReceipt =>
            [
                new() { AccountId = document.CashAccountId ?? document.BankAccountId ?? settings?.CashAccountId ?? settings?.BankAccountId ?? profile.CashAccountId, Debit = document.TotalAmount, Description = document.Number },
                new() { AccountId = profile.ReceivableAccountId, Credit = document.TotalAmount, Description = document.Number }
            ],
            AccountingDocumentType.SupplierPayment =>
            [
                new() { AccountId = profile.PayableAccountId, Debit = document.TotalAmount, Description = document.Number },
                new() { AccountId = settings?.BankAccountId ?? profile.BankAccountId, Credit = document.TotalAmount, Description = document.Number }
            ],
            AccountingDocumentType.SalesCreditNote =>
            [
                new() { AccountId = profile.RevenueAccountId, Debit = document.Subtotal, Description = document.Number },
                new() { AccountId = profile.OutputVatAccountId, Debit = document.TaxAmount, Description = document.Number },
                new() { AccountId = profile.ReceivableAccountId, Credit = document.TotalAmount, Description = document.Number }
            ],
            _ =>
            [
                new() { AccountId = profile.ReceivableAccountId, Debit = document.TotalAmount, Description = document.Number },
                new() { AccountId = profile.RevenueAccountId, Credit = document.Subtotal, Description = document.Number },
                new() { AccountId = profile.OutputVatAccountId, Credit = document.TaxAmount, Description = document.Number }
            ]
        });

    private async Task EnsureDocumentCashBankAccountsAsync(AccountingDocumentDto document, CancellationToken cancellationToken)
    {
        if (!document.CashAccountId.HasValue && !document.BankAccountId.HasValue)
            return;

        if (document.CashAccountId.HasValue)
        {
            var exists = await dbContext.CashAccounts.AsNoTracking()
                .AnyAsync(x => x.Id == document.CashAccountId.Value
                    && x.CompanyId == document.CompanyId
                    && x.BranchId == document.BranchId
                    && x.IsActive, cancellationToken);
            if (!exists)
                throw new BadRequestException("Cash account must be active and belong to the document branch.");
        }

        if (document.BankAccountId.HasValue)
        {
            var exists = await dbContext.BankAccounts.AsNoTracking()
                .AnyAsync(x => x.Id == document.BankAccountId.Value
                    && x.CompanyId == document.CompanyId
                    && x.BranchId == document.BranchId
                    && x.IsActive, cancellationToken);
            if (!exists)
                throw new BadRequestException("Bank account must be active and belong to the document branch.");
        }
    }

    private static List<JournalEntryLineDto> NonZeroLines(IEnumerable<JournalEntryLineDto> lines) =>
        lines.Where(x => x.Debit > 0 || x.Credit > 0).ToList();

    private static string BuildZatcaXml(AccountingDocument document, ZatcaSettings settings, ZatcaInvoiceType type, long icv, string? previousHash)
    {
        var invoice = new XElement("Invoice",
            new XElement("ProfileId", "reporting:1.0"),
            new XElement("UUID", Guid.NewGuid()),
            new XElement("ICV", icv),
            new XElement("PreviousInvoiceHash", previousHash ?? string.Empty),
            new XElement("InvoiceNumber", document.Number),
            new XElement("InvoiceType", type.ToString()),
            new XElement("IssueDate", document.DocumentDate.ToString("yyyy-MM-dd")),
            new XElement("Seller",
                new XElement("Name", settings.SellerName),
                new XElement("VatNumber", settings.VatNumber),
                new XElement("City", settings.City),
                new XElement("CountryCode", settings.CountryCode)),
            new XElement("Buyer",
                new XElement("Name", document.PartyName ?? string.Empty),
                new XElement("VatNumber", document.PartyVatNumber ?? string.Empty)),
            new XElement("Totals",
                new XElement("TaxExclusiveAmount", document.Subtotal),
                new XElement("TaxAmount", document.TaxAmount),
                new XElement("TaxInclusiveAmount", document.TotalAmount)),
            new XElement("Lines",
                document.Lines.Select(line => new XElement("Line",
                    new XElement("LineNumber", line.LineNumber),
                    new XElement("Description", line.Description),
                    new XElement("Quantity", line.Quantity),
                    new XElement("NetAmount", line.NetAmount),
                    new XElement("TaxRate", line.TaxRate),
                    new XElement("TaxAmount", line.TaxAmount),
                    new XElement("TotalAmount", line.TotalAmount)))));

        return new XDocument(new XDeclaration("1.0", "utf-8", null), invoice).ToString(SaveOptions.DisableFormatting);
    }

    private static string BuildQrPayload(string sellerName, string vatNumber, DateTime invoiceDate, decimal totalAmount, decimal vatAmount, string invoiceHash)
    {
        var text = string.Join("|", sellerName, vatNumber, invoiceDate.ToString("O"), totalAmount.ToString("0.00"), vatAmount.ToString("0.00"), invoiceHash);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
    }
}

public class AccountingQueryHandlers(AccountingDbContext dbContext, ISender sender)
    : IQueryHandler<GetAccountsQuery, GetAccountsResult>,
      IQueryHandler<GetFiscalPeriodsQuery, GetFiscalPeriodsResult>,
      IQueryHandler<GetTaxCodesQuery, GetTaxCodesResult>,
      IQueryHandler<GetPostingProfilesQuery, GetPostingProfilesResult>,
      IQueryHandler<GetBankAccountsQuery, GetBankAccountsResult>,
      IQueryHandler<GetCashAccountsQuery, GetCashAccountsResult>,
      IQueryHandler<GetAccountingCashAccountsQuery, GetAccountingCashAccountsResult>,
      IQueryHandler<GetCompanyAccountingSettingsQuery, GetCompanyAccountingSettingsResult>,
      IQueryHandler<GetAccountCodingSettingsQuery, GetAccountCodingSettingsResult>,
      IQueryHandler<GetAccountingDocumentsQuery, GetAccountingDocumentsResult>,
      IQueryHandler<GetJournalEntriesQuery, GetJournalEntriesResult>,
      IQueryHandler<GetZatcaSettingsQuery, GetZatcaSettingsResult>,
      IQueryHandler<GetEInvoicesQuery, GetEInvoicesResult>,
      IQueryHandler<GetAccountingDashboardQuery, GetAccountingDashboardResult>,
      IQueryHandler<GetAccountingTemplatesQuery, GetAccountingTemplatesResult>,
      IQueryHandler<GetAccountingTemplateByIdQuery, GetAccountingTemplateByIdResult>,
      IQueryHandler<GetAccountingSetupStatusQuery, GetAccountingSetupStatusResult>,
      IQueryHandler<GetBankTransactionsQuery, GetBankTransactionsResult>,
      IQueryHandler<GetBankReconciliationSummaryQuery, GetBankReconciliationSummaryResult>,
      IQueryHandler<GetBankReconciliationMatchesQuery, GetBankReconciliationMatchesResult>,
      IQueryHandler<GetAccountingReportQuery, GetAccountingReportResult>
{
    public async Task<GetAccountingTemplatesResult> Handle(GetAccountingTemplatesQuery query, CancellationToken cancellationToken)
    {
        var templates = new List<AccountingTemplateDto> { SaudiAccountingTemplate.Template };
        var custom = await dbContext.AccountingTemplates
            .Include(x => x.Accounts)
            .Include(x => x.TaxCodes)
            .Include(x => x.PostingProfiles)
            .Include(x => x.Journals)
            .AsNoTracking()
            .Where(x => x.IsActive && (x.Visibility == AccountingTemplateVisibility.Shared || (query.CompanyId.HasValue && x.CompanyId == query.CompanyId.Value)))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        templates.AddRange(custom.Select(x => x.ToDto(false)));
        return new GetAccountingTemplatesResult(templates);
    }

    public async Task<GetAccountingTemplateByIdResult> Handle(GetAccountingTemplateByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
            return new GetAccountingTemplateByIdResult(SaudiAccountingTemplate.Template);

        var template = await dbContext.AccountingTemplates
            .Include(x => x.Accounts)
            .Include(x => x.TaxCodes)
            .Include(x => x.PostingProfiles)
            .Include(x => x.Journals)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.Id
                && x.IsActive
                && (x.Visibility == AccountingTemplateVisibility.Shared || !query.CompanyId.HasValue || x.CompanyId == query.CompanyId.Value), cancellationToken)
            ?? throw new NotFoundException("Accounting template", query.Id);
        return new GetAccountingTemplateByIdResult(template.ToDto());
    }

    public async Task<GetAccountingSetupStatusResult> Handle(GetAccountingSetupStatusQuery query, CancellationToken cancellationToken)
    {
        var accounts = await dbContext.Accounts.AsNoTracking().Where(x => x.CompanyId == query.CompanyId).ToListAsync(cancellationToken);
        var templateKeys = accounts.Select(x => x.TemplateKey).Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var codes = accounts.Select(x => x.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingAccounts = SaudiAccountingTemplate.Accounts.Count(x => !templateKeys.Contains(x.TemplateKey) && !codes.Contains(x.Code));
        var taxCodes = await dbContext.TaxCodes.AsNoTracking().Where(x => x.CompanyId == query.CompanyId).Select(x => x.Code).ToListAsync(cancellationToken);
        var profiles = await dbContext.PostingProfiles.AsNoTracking().Where(x => x.CompanyId == query.CompanyId && x.IsDefault).Select(x => x.Type).ToListAsync(cancellationToken);
        var journals = await dbContext.AccountingJournals.AsNoTracking().Where(x => x.CompanyId == query.CompanyId).Select(x => x.Code).ToListAsync(cancellationToken);
        var accountingSettings = await dbContext.CompanyAccountingSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == query.CompanyId, cancellationToken);
        var companyDefaultsComplete = accountingSettings is not null && SettingsAccountIds(accountingSettings.ToDto()).All(x => x.HasValue && x.Value != Guid.Empty);
        var defaultBankExists = await dbContext.BankAccounts.AsNoTracking().AnyAsync(x => x.CompanyId == query.CompanyId && x.IsDefault && x.IsActive, cancellationToken);
        var defaultCashExists = await dbContext.CashAccounts.AsNoTracking().AnyAsync(x => x.CompanyId == query.CompanyId && x.IsDefault && x.IsActive, cancellationToken);
        var fiscalPeriodExists = await dbContext.FiscalPeriods.AsNoTracking().AnyAsync(x => x.CompanyId == query.CompanyId && x.Status == FiscalPeriodStatus.Open, cancellationToken);
        var settings = await dbContext.ZatcaSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == query.CompanyId, cancellationToken);
        var zatcaComplete = settings is not null
            && !string.IsNullOrWhiteSpace(settings.SellerName)
            && !string.IsNullOrWhiteSpace(settings.SellerNameAr)
            && !string.IsNullOrWhiteSpace(settings.VatNumber)
            && !string.IsNullOrWhiteSpace(settings.BuildingNumber)
            && !string.IsNullOrWhiteSpace(settings.City)
            && string.Equals(settings.CountryCode, "SA", StringComparison.OrdinalIgnoreCase);

        var taxSet = taxCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var profileSet = profiles.ToHashSet();
        var journalSet = journals.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var defaultTaxCodesExist = SaudiAccountingTemplate.RequiredTaxCodes.All(taxSet.Contains);
        var postingProfilesExist = SaudiAccountingTemplate.RequiredPostingProfiles.All(profileSet.Contains);
        var journalsExist = SaudiAccountingTemplate.RequiredJournalCodes.All(journalSet.Contains);
        var missingItems = new List<string>();
        if (missingAccounts > 0) missingItems.Add($"{missingAccounts} minimum accounts");
        if (!defaultTaxCodesExist) missingItems.Add("default VAT tax codes");
        if (!companyDefaultsComplete) missingItems.Add("company accounting defaults");
        if (!defaultBankExists) missingItems.Add("default bank account");
        if (!defaultCashExists) missingItems.Add("default cash account");
        if (!postingProfilesExist) missingItems.Add("default posting profiles");
        if (!journalsExist) missingItems.Add("default journals");
        if (!fiscalPeriodExists) missingItems.Add("open fiscal period");
        if (!zatcaComplete) missingItems.Add("ZATCA seller settings");

        return new GetAccountingSetupStatusResult(new AccountingSetupStatusDto
        {
            CompanyId = query.CompanyId,
            ChartExists = accounts.Any(),
            MinimumAccountsMissing = missingAccounts,
            DefaultTaxCodesExist = defaultTaxCodesExist,
            CompanyDefaultsComplete = companyDefaultsComplete,
            DefaultBankAccountExists = defaultBankExists,
            DefaultCashAccountExists = defaultCashExists,
            PostingProfilesExist = postingProfilesExist,
            JournalsExist = journalsExist,
            FiscalPeriodExists = fiscalPeriodExists,
            ZatcaSettingsComplete = zatcaComplete,
            ReadyToPost = missingAccounts == 0 && defaultTaxCodesExist && companyDefaultsComplete && defaultBankExists && defaultCashExists && postingProfilesExist && journalsExist && fiscalPeriodExists,
            MissingItems = missingItems
        });
    }

    public async Task<GetAccountsResult> Handle(GetAccountsQuery query, CancellationToken cancellationToken)
    {
        var accounts = dbContext.Accounts.AsNoTracking().Where(x => x.CompanyId == query.CompanyId);
        accounts = await ApplyBranchAccessAsync(accounts, query.CompanyId, query.BranchId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(query.SearchText))
            accounts = accounts.Where(x => x.Code.Contains(query.SearchText) || x.Name.Contains(query.SearchText) || x.NameEng.Contains(query.SearchText));

        var count = await accounts.LongCountAsync(cancellationToken);
        var pageIndex = query.PageIndex <= 0 ? 1 : query.PageIndex;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var data = await accounts.OrderBy(x => x.Code).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new GetAccountsResult(new PaginatedResult<AccountDto>(pageIndex, pageSize, count, data.Select(x => x.ToDto())));
    }

    public async Task<GetFiscalPeriodsResult> Handle(GetFiscalPeriodsQuery query, CancellationToken cancellationToken) =>
        new((await dbContext.FiscalPeriods.AsNoTracking().Where(x => x.CompanyId == query.CompanyId).OrderByDescending(x => x.StartDate).ToListAsync(cancellationToken)).Select(x => x.ToDto()).ToList());

    public async Task<GetTaxCodesResult> Handle(GetTaxCodesQuery query, CancellationToken cancellationToken) =>
        new((await dbContext.TaxCodes.AsNoTracking().Where(x => x.CompanyId == query.CompanyId).OrderBy(x => x.Code).ToListAsync(cancellationToken)).Select(x => x.ToDto()).ToList());

    public async Task<GetPostingProfilesResult> Handle(GetPostingProfilesQuery query, CancellationToken cancellationToken) =>
        new((await dbContext.PostingProfiles.AsNoTracking().Where(x => x.CompanyId == query.CompanyId).OrderBy(x => x.Type).ToListAsync(cancellationToken)).Select(x => x.ToDto()).ToList());

    public async Task<GetBankAccountsResult> Handle(GetBankAccountsQuery query, CancellationToken cancellationToken)
    {
        var bankAccounts = dbContext.BankAccounts.AsNoTracking().Where(x => x.CompanyId == query.CompanyId);
        bankAccounts = await ApplyBranchAccessAsync(bankAccounts, query.CompanyId, query.BranchId, cancellationToken);
        return new((await bankAccounts.OrderByDescending(x => x.IsDefault).ThenBy(x => x.DisplayName).ToListAsync(cancellationToken)).Select(x => x.ToDto()).ToList());
    }

    public async Task<GetCashAccountsResult> Handle(GetCashAccountsQuery query, CancellationToken cancellationToken)
    {
        var cashAccounts = dbContext.CashAccounts.AsNoTracking().Where(x => x.CompanyId == query.CompanyId);
        cashAccounts = await ApplyBranchAccessAsync(cashAccounts, query.CompanyId, query.BranchId, cancellationToken);
        return new((await cashAccounts.OrderByDescending(x => x.IsDefault).ThenBy(x => x.DisplayName).ToListAsync(cancellationToken)).Select(x => x.ToDto()).ToList());
    }

    public async Task<GetAccountingCashAccountsResult> Handle(GetAccountingCashAccountsQuery query, CancellationToken cancellationToken)
    {
        var result = await Handle(new GetCashAccountsQuery(query.CompanyId, query.BranchId), cancellationToken);
        return new GetAccountingCashAccountsResult(result.CashAccounts);
    }

    public async Task<GetCompanyAccountingSettingsResult> Handle(GetCompanyAccountingSettingsQuery query, CancellationToken cancellationToken)
    {
        var settings = await dbContext.CompanyAccountingSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == query.CompanyId, cancellationToken);
        return new GetCompanyAccountingSettingsResult(settings?.ToDto());
    }

    public async Task<GetAccountCodingSettingsResult> Handle(GetAccountCodingSettingsQuery query, CancellationToken cancellationToken)
    {
        var settings = await dbContext.AccountCodingSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == query.CompanyId, cancellationToken);
        return new GetAccountCodingSettingsResult(settings?.ToDto() ?? AccountCodingSettings.Default(query.CompanyId));
    }

    public async Task<GetAccountingDocumentsResult> Handle(GetAccountingDocumentsQuery query, CancellationToken cancellationToken)
    {
        var documents = dbContext.AccountingDocuments.Include(x => x.Lines).AsNoTracking();
        if (query.CompanyId.HasValue)
        {
            documents = documents.Where(x => x.CompanyId == query.CompanyId.Value);
            documents = await ApplyBranchAccessAsync(documents, query.CompanyId.Value, query.BranchId, cancellationToken);
        }
        if (query.Type.HasValue)
            documents = documents.Where(x => x.Type == query.Type.Value);
        if (!string.IsNullOrWhiteSpace(query.SearchText))
            documents = documents.Where(x => x.Number.Contains(query.SearchText) || (x.PartyName != null && x.PartyName.Contains(query.SearchText)));

        var count = await documents.LongCountAsync(cancellationToken);
        var pageIndex = query.PageIndex <= 0 ? 1 : query.PageIndex;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var data = await documents.OrderByDescending(x => x.DocumentDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new GetAccountingDocumentsResult(new PaginatedResult<AccountingDocumentDto>(pageIndex, pageSize, count, data.Select(x => x.ToDto())));
    }

    public async Task<GetJournalEntriesResult> Handle(GetJournalEntriesQuery query, CancellationToken cancellationToken)
    {
        var entries = dbContext.JournalEntries.Include(x => x.Lines).AsNoTracking();
        if (query.CompanyId.HasValue)
        {
            entries = entries.Where(x => x.CompanyId == query.CompanyId.Value);
            entries = await ApplyBranchAccessAsync(entries, query.CompanyId.Value, query.BranchId, cancellationToken);
        }
        if (!string.IsNullOrWhiteSpace(query.SearchText))
            entries = entries.Where(x => x.Number.Contains(query.SearchText) || (x.SourceDocumentNumber != null && x.SourceDocumentNumber.Contains(query.SearchText)));

        var count = await entries.LongCountAsync(cancellationToken);
        var pageIndex = query.PageIndex <= 0 ? 1 : query.PageIndex;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var data = await entries.OrderByDescending(x => x.EntryDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new GetJournalEntriesResult(new PaginatedResult<JournalEntryDto>(pageIndex, pageSize, count, data.Select(x => x.ToDto())));
    }

    public async Task<GetZatcaSettingsResult> Handle(GetZatcaSettingsQuery query, CancellationToken cancellationToken)
    {
        var settings = await dbContext.ZatcaSettings.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == query.CompanyId, cancellationToken);
        var device = await dbContext.ZatcaDevices.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == query.CompanyId && x.IsActive, cancellationToken);
        return new GetZatcaSettingsResult(settings?.ToDto(device?.Name));
    }

    public async Task<GetEInvoicesResult> Handle(GetEInvoicesQuery query, CancellationToken cancellationToken)
    {
        var invoices = dbContext.EInvoices.AsNoTracking();
        if (query.CompanyId.HasValue)
            invoices = invoices.Where(x => x.CompanyId == query.CompanyId.Value);
        if (query.Status.HasValue)
            invoices = invoices.Where(x => x.SubmissionStatus == query.Status.Value);

        var count = await invoices.LongCountAsync(cancellationToken);
        var pageIndex = query.PageIndex <= 0 ? 1 : query.PageIndex;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var data = await invoices.OrderByDescending(x => x.CreatedAt).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new GetEInvoicesResult(new PaginatedResult<EInvoiceDto>(pageIndex, pageSize, count, data.Select(x => x.ToDto())));
    }

    public async Task<GetAccountingDashboardResult> Handle(GetAccountingDashboardQuery query, CancellationToken cancellationToken)
    {
        var accounts = dbContext.Accounts.AsNoTracking();
        var periods = dbContext.FiscalPeriods.AsNoTracking();
        var documents = dbContext.AccountingDocuments.AsNoTracking();
        var invoices = dbContext.EInvoices.AsNoTracking();
        var bankTransactions = dbContext.BankTransactions.AsNoTracking();
        if (query.CompanyId.HasValue)
        {
            accounts = accounts.Where(x => x.CompanyId == query.CompanyId.Value);
            periods = periods.Where(x => x.CompanyId == query.CompanyId.Value);
            documents = documents.Where(x => x.CompanyId == query.CompanyId.Value);
            invoices = invoices.Where(x => x.CompanyId == query.CompanyId.Value);
            bankTransactions = bankTransactions.Where(x => x.CompanyId == query.CompanyId.Value);
            accounts = await ApplyBranchAccessAsync(accounts, query.CompanyId.Value, query.BranchId, cancellationToken);
            documents = await ApplyBranchAccessAsync(documents, query.CompanyId.Value, query.BranchId, cancellationToken);
            bankTransactions = await ApplyBranchAccessAsync(bankTransactions, query.CompanyId.Value, query.BranchId, cancellationToken);
        }

        return new GetAccountingDashboardResult(new AccountingDashboardDto
        {
            Accounts = await accounts.CountAsync(cancellationToken),
            OpenPeriods = await periods.CountAsync(x => x.Status == FiscalPeriodStatus.Open, cancellationToken),
            DraftDocuments = await documents.CountAsync(x => x.Status == AccountingDocumentStatus.Draft, cancellationToken),
            PostedDocuments = await documents.CountAsync(x => x.Status == AccountingDocumentStatus.Posted, cancellationToken),
            UnreconciledBankTransactions = await bankTransactions.CountAsync(x => x.Status == BankTransactionStatus.Unreconciled, cancellationToken),
            PendingZatcaSubmissions = await invoices.CountAsync(x => x.SubmissionStatus == ZatcaSubmissionStatus.Pending || x.SubmissionStatus == ZatcaSubmissionStatus.RetryScheduled, cancellationToken),
            FailedZatcaSubmissions = await invoices.CountAsync(x => x.SubmissionStatus == ZatcaSubmissionStatus.Failed, cancellationToken),
            OutputVat = await documents.Where(x => x.Type == AccountingDocumentType.SalesInvoice && x.Status == AccountingDocumentStatus.Posted).SumAsync(x => x.TaxAmount, cancellationToken),
            InputVat = await documents.Where(x => x.Type == AccountingDocumentType.SupplierInvoice && x.Status == AccountingDocumentStatus.Posted).SumAsync(x => x.TaxAmount, cancellationToken)
        });
    }

    public async Task<GetBankTransactionsResult> Handle(GetBankTransactionsQuery query, CancellationToken cancellationToken)
    {
        var transactions = dbContext.BankTransactions.AsNoTracking().Where(x => x.CompanyId == query.CompanyId);
        transactions = await ApplyBranchAccessAsync(transactions, query.CompanyId, query.BranchId, cancellationToken);
        if (query.Status.HasValue)
            transactions = transactions.Where(x => x.Status == query.Status.Value);
        if (!string.IsNullOrWhiteSpace(query.SearchText))
            transactions = transactions.Where(x => x.Description.Contains(query.SearchText) || (x.ReferenceNumber != null && x.ReferenceNumber.Contains(query.SearchText)));

        var count = await transactions.LongCountAsync(cancellationToken);
        var pageIndex = query.PageIndex <= 0 ? 1 : query.PageIndex;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var data = await transactions
            .OrderBy(x => x.Status)
            .ThenByDescending(x => x.TransactionDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new GetBankTransactionsResult(new PaginatedResult<BankTransactionDto>(pageIndex, pageSize, count, data.Select(x => x.ToDto())));
    }

    public async Task<GetBankReconciliationSummaryResult> Handle(GetBankReconciliationSummaryQuery query, CancellationToken cancellationToken)
    {
        var transactionQuery = dbContext.BankTransactions.AsNoTracking()
            .Where(x => x.CompanyId == query.CompanyId);
        transactionQuery = await ApplyBranchAccessAsync(transactionQuery, query.CompanyId, query.BranchId, cancellationToken);
        var transactions = await transactionQuery
            .ToListAsync(cancellationToken);

        return new GetBankReconciliationSummaryResult(new BankReconciliationSummaryDto
        {
            UnreconciledCount = transactions.Count(x => x.Status == BankTransactionStatus.Unreconciled),
            UnreconciledInflow = transactions.Where(x => x.Status == BankTransactionStatus.Unreconciled && x.Amount > 0).Sum(x => x.Amount),
            UnreconciledOutflow = Math.Abs(transactions.Where(x => x.Status == BankTransactionStatus.Unreconciled && x.Amount < 0).Sum(x => x.Amount)),
            ReconciledCount = transactions.Count(x => x.Status == BankTransactionStatus.Reconciled)
        });
    }

    public async Task<GetBankReconciliationMatchesResult> Handle(GetBankReconciliationMatchesQuery query, CancellationToken cancellationToken)
    {
        var transaction = await dbContext.BankTransactions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == query.BankTransactionId, cancellationToken)
            ?? throw new NotFoundException("Bank transaction", query.BankTransactionId);
        await EnsureCanReadBranchAsync(transaction.CompanyId, transaction.BranchId, cancellationToken);

        var amount = Math.Abs(transaction.Amount);
        var fromDate = transaction.TransactionDate.Date.AddDays(-14);
        var toDate = transaction.TransactionDate.Date.AddDays(14);

        var documents = await dbContext.AccountingDocuments.AsNoTracking()
            .Where(x => x.CompanyId == transaction.CompanyId
                && x.BranchId == transaction.BranchId
                && x.Status == AccountingDocumentStatus.Posted
                && x.DocumentDate >= fromDate
                && x.DocumentDate <= toDate
                && (x.TotalAmount == amount || x.Number == transaction.ReferenceNumber))
            .OrderByDescending(x => x.DocumentDate)
            .Take(10)
            .ToListAsync(cancellationToken);

        var journals = await dbContext.JournalEntries.AsNoTracking()
            .Where(x => x.CompanyId == transaction.CompanyId
                && x.BranchId == transaction.BranchId
                && x.Status == JournalEntryStatus.Posted
                && x.EntryDate >= fromDate
                && x.EntryDate <= toDate
                && (x.TotalDebit == amount || x.Number == transaction.ReferenceNumber || x.SourceDocumentNumber == transaction.ReferenceNumber))
            .OrderByDescending(x => x.EntryDate)
            .Take(10)
            .ToListAsync(cancellationToken);

        var matches = documents.Select(x => new BankReconciliationMatchDto
            {
                Id = x.Id,
                Number = x.Number,
                Source = x.Type.ToString(),
                Date = x.DocumentDate,
                PartyOrMemo = x.PartyName,
                Amount = x.TotalAmount,
                IsJournalEntry = false
            })
            .Concat(journals.Select(x => new BankReconciliationMatchDto
            {
                Id = x.Id,
                Number = x.Number,
                Source = x.SourceModule ?? "Journal",
                Date = x.EntryDate,
                PartyOrMemo = x.Memo,
                Amount = x.TotalDebit,
                IsJournalEntry = true
            }))
            .OrderByDescending(x => x.Number == transaction.ReferenceNumber)
            .ThenBy(x => Math.Abs(x.Amount - amount))
            .ThenByDescending(x => x.Date)
            .Take(12)
            .ToList();

        return new GetBankReconciliationMatchesResult(matches);
    }

    public async Task<GetAccountingReportResult> Handle(GetAccountingReportQuery query, CancellationToken cancellationToken)
    {
        var fromDate = query.FromDate?.Date;
        var toDate = query.ToDate?.Date;
        var entries = dbContext.JournalEntries.Include(x => x.Lines).AsNoTracking()
            .Where(x => x.CompanyId == query.CompanyId && x.Status == JournalEntryStatus.Posted);
        entries = await ApplyBranchAccessAsync(entries, query.CompanyId, query.BranchId, cancellationToken);
        if (fromDate.HasValue)
            entries = entries.Where(x => x.EntryDate >= fromDate.Value);
        if (toDate.HasValue)
            entries = entries.Where(x => x.EntryDate <= toDate.Value);

        var entryList = await entries.OrderBy(x => x.EntryDate).ToListAsync(cancellationToken);
        var accountIds = entryList.SelectMany(x => x.Lines).Select(x => x.AccountId).Distinct().ToList();
        var accounts = await dbContext.Accounts.AsNoTracking()
            .Where(x => x.CompanyId == query.CompanyId && accountIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var rows = query.Type switch
        {
            AccountingReportType.TrialBalance => BuildTrialBalanceRows(entryList, accounts),
            AccountingReportType.AgedReceivables => await BuildAgedDocumentRowsAsync(query.CompanyId, query.BranchId, AccountingDocumentType.SalesInvoice, cancellationToken),
            AccountingReportType.AgedPayables => await BuildAgedDocumentRowsAsync(query.CompanyId, query.BranchId, AccountingDocumentType.SupplierInvoice, cancellationToken),
            AccountingReportType.TaxSummary => await BuildTaxSummaryRowsAsync(query.CompanyId, query.BranchId, fromDate, toDate, cancellationToken),
            AccountingReportType.BalanceSheet => BuildStatementRows(entryList, accounts, [AccountType.Asset, AccountType.Liability, AccountType.Equity]),
            AccountingReportType.ProfitAndLoss => BuildStatementRows(entryList, accounts, [AccountType.Revenue, AccountType.Expense]),
            AccountingReportType.CashFlow => BuildCashFlowRows(entryList, accounts),
            AccountingReportType.VatReturn => await BuildVatReturnRowsAsync(query.CompanyId, query.BranchId, fromDate, toDate, cancellationToken),
            AccountingReportType.AuditTrail => BuildAuditTrailRows(entryList),
            _ => BuildGeneralLedgerRows(entryList, accounts)
        };

        return new GetAccountingReportResult(new AccountingReportDto
        {
            Type = query.Type,
            CompanyId = query.CompanyId,
            BranchId = query.BranchId,
            FromDate = fromDate,
            ToDate = toDate,
            Rows = rows,
            TotalDebit = rows.Sum(x => x.Debit),
            TotalCredit = rows.Sum(x => x.Credit),
            Balance = rows.Sum(x => x.Debit - x.Credit)
        });
    }

    private static List<AccountingReportRowDto> BuildGeneralLedgerRows(List<JournalEntry> entries, Dictionary<Guid, Account> accounts)
    {
        var running = new Dictionary<Guid, decimal>();
        var rows = new List<AccountingReportRowDto>();
        foreach (var entry in entries)
        {
            foreach (var line in entry.Lines)
            {
                accounts.TryGetValue(line.AccountId, out var account);
                running[line.AccountId] = running.GetValueOrDefault(line.AccountId) + line.Debit - line.Credit;
                rows.Add(new AccountingReportRowDto
                {
                    Date = entry.EntryDate,
                    Code = account?.Code ?? string.Empty,
                    Name = account?.NameEng ?? account?.Name ?? line.AccountId.ToString(),
                    Source = entry.SourceDocumentNumber ?? entry.Number,
                    Party = entry.Memo,
                    Debit = line.Debit,
                    Credit = line.Credit,
                    Balance = running[line.AccountId]
                });
            }
        }

        return rows;
    }

    private static List<AccountingReportRowDto> BuildTrialBalanceRows(List<JournalEntry> entries, Dictionary<Guid, Account> accounts) =>
        entries.SelectMany(x => x.Lines)
            .GroupBy(x => x.AccountId)
            .Select(x =>
            {
                accounts.TryGetValue(x.Key, out var account);
                var debit = x.Sum(line => line.Debit);
                var credit = x.Sum(line => line.Credit);
                return new AccountingReportRowDto
                {
                    Code = account?.Code ?? string.Empty,
                    Name = account?.NameEng ?? account?.Name ?? x.Key.ToString(),
                    Debit = debit,
                    Credit = credit,
                    Balance = debit - credit
                };
            })
            .OrderBy(x => x.Code)
            .ToList();

    private static List<AccountingReportRowDto> BuildStatementRows(List<JournalEntry> entries, Dictionary<Guid, Account> accounts, AccountType[] types) =>
        entries.SelectMany(x => x.Lines)
            .GroupBy(x => x.AccountId)
            .Select(x =>
            {
                accounts.TryGetValue(x.Key, out var account);
                if (account is null || !types.Contains(account.Type))
                    return null;

                var debit = x.Sum(line => line.Debit);
                var credit = x.Sum(line => line.Credit);
                var balance = account.NormalBalance == NormalBalance.Debit ? debit - credit : credit - debit;
                return new AccountingReportRowDto
                {
                    Code = account.Code,
                    Name = account.NameEng,
                    Source = account.Type.ToString(),
                    Debit = debit,
                    Credit = credit,
                    Balance = balance
                };
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .OrderBy(x => x.Code)
            .ToList();

    private static List<AccountingReportRowDto> BuildCashFlowRows(List<JournalEntry> entries, Dictionary<Guid, Account> accounts)
    {
        var cashAccountIds = accounts
            .Where(x => x.Value.Role is AccountRole.Cash or AccountRole.Bank)
            .Select(x => x.Key)
            .ToHashSet();

        return entries
            .SelectMany(entry => entry.Lines
                .Where(line => cashAccountIds.Contains(line.AccountId))
                .Select(line =>
                {
                    accounts.TryGetValue(line.AccountId, out var account);
                    return new AccountingReportRowDto
                    {
                        Date = entry.EntryDate,
                        Code = account?.Code ?? string.Empty,
                        Name = account?.NameEng ?? account?.Name ?? line.AccountId.ToString(),
                        Source = entry.SourceDocumentNumber ?? entry.Number,
                        Party = entry.Memo,
                        Debit = line.Debit,
                        Credit = line.Credit,
                        Balance = line.Debit - line.Credit
                    };
                }))
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Code)
            .ToList();
    }

    private static List<AccountingReportRowDto> BuildAuditTrailRows(List<JournalEntry> entries) =>
        entries.Select(x => new AccountingReportRowDto
            {
                Date = x.EntryDate,
                Code = x.Number,
                Name = x.Status.ToString(),
                Source = x.SourceModule,
                Party = x.SourceDocumentNumber ?? x.Memo,
                Debit = x.TotalDebit,
                Credit = x.TotalCredit,
                Balance = x.TotalDebit - x.TotalCredit
            })
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Code)
            .ToList();

    private async Task<List<AccountingReportRowDto>> BuildAgedDocumentRowsAsync(Guid companyId, Guid? branchId, AccountingDocumentType type, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var documentQuery = dbContext.AccountingDocuments.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.Type == type && x.Status == AccountingDocumentStatus.Posted);
        documentQuery = await ApplyBranchAccessAsync(documentQuery, companyId, branchId, cancellationToken);
        var documents = await documentQuery
            .OrderBy(x => x.DocumentDate)
            .ToListAsync(cancellationToken);

        return documents.Select(x => new AccountingReportRowDto
        {
            Date = x.DocumentDate,
            Code = x.Number,
            Name = x.PartyName ?? "-",
            Source = x.Type.ToString(),
            Debit = type == AccountingDocumentType.SalesInvoice ? x.TotalAmount : 0,
            Credit = type == AccountingDocumentType.SupplierInvoice ? x.TotalAmount : 0,
            Balance = x.TotalAmount,
            Party = $"{Math.Max(0, (today - x.DocumentDate.Date).Days)} days"
        }).ToList();
    }

    private async Task<List<AccountingReportRowDto>> BuildTaxSummaryRowsAsync(Guid companyId, Guid? branchId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        var documents = dbContext.AccountingDocuments.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.Status == AccountingDocumentStatus.Posted);
        documents = await ApplyBranchAccessAsync(documents, companyId, branchId, cancellationToken);
        if (fromDate.HasValue)
            documents = documents.Where(x => x.DocumentDate >= fromDate.Value);
        if (toDate.HasValue)
            documents = documents.Where(x => x.DocumentDate <= toDate.Value);

        return await documents
            .GroupBy(x => x.Type)
            .Select(x => new AccountingReportRowDto
            {
                Code = x.Key.ToString(),
                Name = x.Key.ToString(),
                Debit = x.Where(d => d.Type == AccountingDocumentType.SupplierInvoice).Sum(d => d.TaxAmount),
                Credit = x.Where(d => d.Type == AccountingDocumentType.SalesInvoice).Sum(d => d.TaxAmount),
                TaxAmount = x.Sum(d => d.TaxAmount),
                Balance = x.Sum(d => d.TaxAmount)
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<List<AccountingReportRowDto>> BuildVatReturnRowsAsync(Guid companyId, Guid? branchId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        var rows = await BuildTaxSummaryRowsAsync(companyId, branchId, fromDate, toDate, cancellationToken);
        var outputVat = rows.Sum(x => x.Credit);
        var inputVat = rows.Sum(x => x.Debit);
        return
        [
            new AccountingReportRowDto { Code = "OUTPUT_VAT", Name = "Output VAT", Credit = outputVat, TaxAmount = outputVat, Balance = outputVat },
            new AccountingReportRowDto { Code = "INPUT_VAT", Name = "Input VAT", Debit = inputVat, TaxAmount = inputVat, Balance = inputVat },
            new AccountingReportRowDto { Code = "VAT_DUE", Name = "Net VAT Due", Credit = Math.Max(0, outputVat - inputVat), Debit = Math.Max(0, inputVat - outputVat), Balance = outputVat - inputVat }
        ];
    }

    private async Task EnsureCanReadBranchAsync(Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (access.CanViewAllBranches)
            return;

        if (!branchId.HasValue || !access.BranchIds.Contains(branchId.Value))
            throw new UnauthorizedAccessException("User is not allowed to access this branch accounting data.");
    }

    private async Task<IQueryable<Account>> ApplyBranchAccessAsync(IQueryable<Account> query, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (access.CanViewAllBranches)
            return branchId.HasValue ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value) : query;

        if (branchId.HasValue && !access.BranchIds.Contains(branchId.Value))
            return query.Where(x => false);

        return branchId.HasValue
            ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value)
            : query.Where(x => !x.BranchId.HasValue || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
    }

    private async Task<IQueryable<AccountingDocument>> ApplyBranchAccessAsync(IQueryable<AccountingDocument> query, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (access.CanViewAllBranches)
            return branchId.HasValue ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value) : query;

        if (branchId.HasValue && !access.BranchIds.Contains(branchId.Value))
            return query.Where(x => false);

        return branchId.HasValue
            ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value)
            : query.Where(x => !x.BranchId.HasValue || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
    }

    private async Task<IQueryable<JournalEntry>> ApplyBranchAccessAsync(IQueryable<JournalEntry> query, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (access.CanViewAllBranches)
            return branchId.HasValue ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value) : query;

        if (branchId.HasValue && !access.BranchIds.Contains(branchId.Value))
            return query.Where(x => false);

        return branchId.HasValue
            ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value)
            : query.Where(x => !x.BranchId.HasValue || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
    }

    private async Task<IQueryable<BankAccount>> ApplyBranchAccessAsync(IQueryable<BankAccount> query, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (access.CanViewAllBranches)
            return branchId.HasValue ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value) : query;

        if (branchId.HasValue && !access.BranchIds.Contains(branchId.Value))
            return query.Where(x => false);

        return branchId.HasValue
            ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value)
            : query.Where(x => !x.BranchId.HasValue || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
    }

    private async Task<IQueryable<CashAccount>> ApplyBranchAccessAsync(IQueryable<CashAccount> query, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (access.CanViewAllBranches)
            return branchId.HasValue ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value) : query;

        if (branchId.HasValue && !access.BranchIds.Contains(branchId.Value))
            return query.Where(x => false);

        return branchId.HasValue
            ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value)
            : query.Where(x => !x.BranchId.HasValue || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
    }

    private async Task<IQueryable<BankTransaction>> ApplyBranchAccessAsync(IQueryable<BankTransaction> query, Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (access.CanViewAllBranches)
            return branchId.HasValue ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value) : query;

        if (branchId.HasValue && !access.BranchIds.Contains(branchId.Value))
            return query.Where(x => false);

        return branchId.HasValue
            ? query.Where(x => !x.BranchId.HasValue || x.BranchId == branchId.Value)
            : query.Where(x => !x.BranchId.HasValue || (x.BranchId.HasValue && access.BranchIds.Contains(x.BranchId.Value)));
    }

    private static IEnumerable<Guid?> SettingsAccountIds(CompanyAccountingSettingsDto settings) =>
    [
        settings.ReceivableAccountId,
        settings.PayableAccountId,
        settings.RevenueAccountId,
        settings.ExpenseAccountId,
        settings.CogsAccountId,
        settings.InventoryAccountId,
        settings.InputVatAccountId,
        settings.OutputVatAccountId,
        settings.VatSettlementAccountId,
        settings.CashAccountId,
        settings.BankAccountId,
        settings.RoundingAccountId,
        settings.SuspenseAccountId,
        settings.RetainedEarningsAccountId
    ];
}
