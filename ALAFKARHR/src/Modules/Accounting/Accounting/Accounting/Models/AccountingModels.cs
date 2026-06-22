namespace Accounting.Accounting.Models;

public class Account : Aggregate<Guid>
{
    private Account() { }

    public Guid CompanyId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public NormalBalance NormalBalance { get; private set; }
    public AccountRole Role { get; private set; }
    public string? TemplateKey { get; private set; }
    public Guid? ParentAccountId { get; private set; }
    public bool IsPostingAccount { get; private set; }
    public bool IsSystemAccount { get; private set; }
    public bool IsActive { get; private set; }

    public static Account Create(AccountDto dto, string userId) =>
        new()
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            NameEng = dto.NameEng.Trim(),
            Type = dto.Type,
            NormalBalance = dto.NormalBalance,
            Role = dto.Role,
            TemplateKey = string.IsNullOrWhiteSpace(dto.TemplateKey) ? null : dto.TemplateKey.Trim(),
            ParentAccountId = dto.ParentAccountId,
            IsPostingAccount = dto.IsPostingAccount,
            IsSystemAccount = dto.IsSystemAccount,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void Update(AccountDto dto, string userId)
    {
        if (!IsActive)
            throw new BadRequestException("Inactive account cannot be edited.");

        Code = dto.Code.Trim();
        Name = dto.Name.Trim();
        NameEng = dto.NameEng.Trim();
        Type = dto.Type;
        NormalBalance = dto.NormalBalance;
        Role = dto.Role;
        TemplateKey = string.IsNullOrWhiteSpace(dto.TemplateKey) ? null : dto.TemplateKey.Trim();
        ParentAccountId = dto.ParentAccountId;
        IsPostingAccount = dto.IsPostingAccount;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Deactivate(string userId)
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public AccountDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        Code = Code,
        Name = Name,
        NameEng = NameEng,
        Type = Type,
        NormalBalance = NormalBalance,
        Role = Role,
        TemplateKey = TemplateKey,
        ParentAccountId = ParentAccountId,
        IsPostingAccount = IsPostingAccount,
        IsSystemAccount = IsSystemAccount,
        IsActive = IsActive
    };
}

public class AccountingJournal : Aggregate<Guid>
{
    private AccountingJournal() { }

    public Guid CompanyId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameAr { get; private set; } = string.Empty;
    public AccountingJournalType Type { get; private set; }
    public Guid? DefaultDebitAccountId { get; private set; }
    public Guid? DefaultCreditAccountId { get; private set; }
    public string? ZatcaDeviceSerial { get; private set; }
    public bool IsSystemJournal { get; private set; }
    public bool IsActive { get; private set; }

    public static AccountingJournal Create(AccountingJournalDto dto, string userId) =>
        new()
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            NameAr = dto.NameAr.Trim(),
            Type = dto.Type,
            DefaultDebitAccountId = dto.DefaultDebitAccountId,
            DefaultCreditAccountId = dto.DefaultCreditAccountId,
            ZatcaDeviceSerial = string.IsNullOrWhiteSpace(dto.ZatcaDeviceSerial) ? null : dto.ZatcaDeviceSerial.Trim(),
            IsSystemJournal = dto.IsSystemJournal,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public AccountingJournalDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        Code = Code,
        Name = Name,
        NameAr = NameAr,
        Type = Type,
        DefaultDebitAccountId = DefaultDebitAccountId,
        DefaultCreditAccountId = DefaultCreditAccountId,
        ZatcaDeviceSerial = ZatcaDeviceSerial,
        IsSystemJournal = IsSystemJournal,
        IsActive = IsActive
    };
}

public class FiscalPeriod : Aggregate<Guid>
{
    private FiscalPeriod() { }

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public FiscalPeriodStatus Status { get; private set; }

    public static FiscalPeriod Create(FiscalPeriodDto dto, string userId)
    {
        if (dto.EndDate.Date < dto.StartDate.Date)
            throw new BadRequestException("Fiscal period end date cannot be before start date.");

        return new FiscalPeriod
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            StartDate = dto.StartDate.Date,
            EndDate = dto.EndDate.Date,
            Status = FiscalPeriodStatus.Open,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void Close(string userId)
    {
        Status = FiscalPeriodStatus.Closed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public FiscalPeriodDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        Name = Name,
        StartDate = StartDate,
        EndDate = EndDate,
        Status = Status
    };
}

public class TaxCode : Aggregate<Guid>
{
    private TaxCode() { }

    public Guid CompanyId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Rate { get; private set; }
    public bool IsExempt { get; private set; }
    public string? ZatcaCategoryCode { get; private set; }
    public string? ExemptionReasonCode { get; private set; }
    public bool IsActive { get; private set; }

    public static TaxCode Create(TaxCodeDto dto, string userId) =>
        new()
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            Rate = dto.Rate,
            IsExempt = dto.IsExempt,
            ZatcaCategoryCode = dto.ZatcaCategoryCode,
            ExemptionReasonCode = dto.ExemptionReasonCode,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public TaxCodeDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        Code = Code,
        Name = Name,
        Rate = Rate,
        IsExempt = IsExempt,
        ZatcaCategoryCode = ZatcaCategoryCode,
        ExemptionReasonCode = ExemptionReasonCode,
        IsActive = IsActive
    };
}

public class PostingProfile : Aggregate<Guid>
{
    private PostingProfile() { }

    public Guid CompanyId { get; private set; }
    public PostingProfileType Type { get; private set; }
    public Guid ReceivableAccountId { get; private set; }
    public Guid PayableAccountId { get; private set; }
    public Guid RevenueAccountId { get; private set; }
    public Guid ExpenseAccountId { get; private set; }
    public Guid OutputVatAccountId { get; private set; }
    public Guid InputVatAccountId { get; private set; }
    public Guid CashAccountId { get; private set; }
    public Guid BankAccountId { get; private set; }
    public bool IsDefault { get; private set; }

    public static PostingProfile Create(PostingProfileDto dto, string userId) =>
        new()
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            Type = dto.Type,
            ReceivableAccountId = dto.ReceivableAccountId,
            PayableAccountId = dto.PayableAccountId,
            RevenueAccountId = dto.RevenueAccountId,
            ExpenseAccountId = dto.ExpenseAccountId,
            OutputVatAccountId = dto.OutputVatAccountId,
            InputVatAccountId = dto.InputVatAccountId,
            CashAccountId = dto.CashAccountId,
            BankAccountId = dto.BankAccountId,
            IsDefault = dto.IsDefault,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public PostingProfileDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        Type = Type,
        ReceivableAccountId = ReceivableAccountId,
        PayableAccountId = PayableAccountId,
        RevenueAccountId = RevenueAccountId,
        ExpenseAccountId = ExpenseAccountId,
        OutputVatAccountId = OutputVatAccountId,
        InputVatAccountId = InputVatAccountId,
        CashAccountId = CashAccountId,
        BankAccountId = BankAccountId,
        IsDefault = IsDefault
    };
}

public class BankAccount : Aggregate<Guid>
{
    private BankAccount() { }

    public Guid CompanyId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string BankName { get; private set; } = string.Empty;
    public string? AccountNumber { get; private set; }
    public string? Iban { get; private set; }
    public string? BranchCode { get; private set; }
    public string? Swift { get; private set; }
    public string CurrencyCode { get; private set; } = "SAR";
    public Guid LedgerAccountId { get; private set; }
    public Guid JournalId { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }

    public static BankAccount Create(BankAccountDto dto, Guid ledgerAccountId, Guid journalId, string userId) =>
        new()
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            DisplayName = dto.DisplayName.Trim(),
            BankName = dto.BankName.Trim(),
            AccountNumber = Clean(dto.AccountNumber),
            Iban = Clean(dto.Iban),
            BranchCode = Clean(dto.BranchCode),
            Swift = Clean(dto.Swift),
            CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? "SAR" : dto.CurrencyCode.Trim().ToUpperInvariant(),
            LedgerAccountId = ledgerAccountId,
            JournalId = journalId,
            IsDefault = dto.IsDefault,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void Update(BankAccountDto dto, Guid ledgerAccountId, Guid journalId, string userId)
    {
        DisplayName = dto.DisplayName.Trim();
        BankName = dto.BankName.Trim();
        AccountNumber = Clean(dto.AccountNumber);
        Iban = Clean(dto.Iban);
        BranchCode = Clean(dto.BranchCode);
        Swift = Clean(dto.Swift);
        CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? "SAR" : dto.CurrencyCode.Trim().ToUpperInvariant();
        LedgerAccountId = ledgerAccountId;
        JournalId = journalId;
        IsDefault = dto.IsDefault;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void SetDefault(bool isDefault, string userId)
    {
        IsDefault = isDefault;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public BankAccountDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        DisplayName = DisplayName,
        BankName = BankName,
        AccountNumber = AccountNumber,
        Iban = Iban,
        BranchCode = BranchCode,
        Swift = Swift,
        CurrencyCode = CurrencyCode,
        LedgerAccountId = LedgerAccountId,
        JournalId = JournalId,
        IsDefault = IsDefault,
        IsActive = IsActive
    };

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class CashAccount : Aggregate<Guid>
{
    private CashAccount() { }

    public Guid CompanyId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = "SAR";
    public Guid LedgerAccountId { get; private set; }
    public Guid JournalId { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }

    public static CashAccount Create(CashAccountDto dto, Guid ledgerAccountId, Guid journalId, string userId) =>
        new()
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            DisplayName = dto.DisplayName.Trim(),
            CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? "SAR" : dto.CurrencyCode.Trim().ToUpperInvariant(),
            LedgerAccountId = ledgerAccountId,
            JournalId = journalId,
            IsDefault = dto.IsDefault,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void Update(CashAccountDto dto, Guid ledgerAccountId, Guid journalId, string userId)
    {
        DisplayName = dto.DisplayName.Trim();
        CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? "SAR" : dto.CurrencyCode.Trim().ToUpperInvariant();
        LedgerAccountId = ledgerAccountId;
        JournalId = journalId;
        IsDefault = dto.IsDefault;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void SetDefault(bool isDefault, string userId)
    {
        IsDefault = isDefault;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public CashAccountDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        DisplayName = DisplayName,
        CurrencyCode = CurrencyCode,
        LedgerAccountId = LedgerAccountId,
        JournalId = JournalId,
        IsDefault = IsDefault,
        IsActive = IsActive
    };
}

public class CompanyAccountingSettings : Aggregate<Guid>
{
    private CompanyAccountingSettings() { }

    public Guid CompanyId { get; private set; }
    public Guid? ReceivableAccountId { get; private set; }
    public Guid? PayableAccountId { get; private set; }
    public Guid? RevenueAccountId { get; private set; }
    public Guid? ExpenseAccountId { get; private set; }
    public Guid? CogsAccountId { get; private set; }
    public Guid? InventoryAccountId { get; private set; }
    public Guid? InputVatAccountId { get; private set; }
    public Guid? OutputVatAccountId { get; private set; }
    public Guid? VatSettlementAccountId { get; private set; }
    public Guid? CashAccountId { get; private set; }
    public Guid? BankAccountId { get; private set; }
    public Guid? RoundingAccountId { get; private set; }
    public Guid? SuspenseAccountId { get; private set; }
    public Guid? RetainedEarningsAccountId { get; private set; }
    public int FiscalYearStartMonth { get; private set; } = 1;
    public int FiscalYearStartDay { get; private set; } = 1;

    public static CompanyAccountingSettings Upsert(CompanyAccountingSettingsDto dto, string userId) =>
        new CompanyAccountingSettings
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        }.Apply(dto, userId, false);

    public void Update(CompanyAccountingSettingsDto dto, string userId) => Apply(dto, userId, true);

    private CompanyAccountingSettings Apply(CompanyAccountingSettingsDto dto, string userId, bool modified)
    {
        ReceivableAccountId = dto.ReceivableAccountId;
        PayableAccountId = dto.PayableAccountId;
        RevenueAccountId = dto.RevenueAccountId;
        ExpenseAccountId = dto.ExpenseAccountId;
        CogsAccountId = dto.CogsAccountId;
        InventoryAccountId = dto.InventoryAccountId;
        InputVatAccountId = dto.InputVatAccountId;
        OutputVatAccountId = dto.OutputVatAccountId;
        VatSettlementAccountId = dto.VatSettlementAccountId;
        CashAccountId = dto.CashAccountId;
        BankAccountId = dto.BankAccountId;
        RoundingAccountId = dto.RoundingAccountId;
        SuspenseAccountId = dto.SuspenseAccountId;
        RetainedEarningsAccountId = dto.RetainedEarningsAccountId;
        FiscalYearStartMonth = dto.FiscalYearStartMonth is >= 1 and <= 12 ? dto.FiscalYearStartMonth : 1;
        FiscalYearStartDay = dto.FiscalYearStartDay is >= 1 and <= 31 ? dto.FiscalYearStartDay : 1;
        if (modified)
        {
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = userId;
        }
        return this;
    }

    public CompanyAccountingSettingsDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        ReceivableAccountId = ReceivableAccountId,
        PayableAccountId = PayableAccountId,
        RevenueAccountId = RevenueAccountId,
        ExpenseAccountId = ExpenseAccountId,
        CogsAccountId = CogsAccountId,
        InventoryAccountId = InventoryAccountId,
        InputVatAccountId = InputVatAccountId,
        OutputVatAccountId = OutputVatAccountId,
        VatSettlementAccountId = VatSettlementAccountId,
        CashAccountId = CashAccountId,
        BankAccountId = BankAccountId,
        RoundingAccountId = RoundingAccountId,
        SuspenseAccountId = SuspenseAccountId,
        RetainedEarningsAccountId = RetainedEarningsAccountId,
        FiscalYearStartMonth = FiscalYearStartMonth,
        FiscalYearStartDay = FiscalYearStartDay
    };
}

public class AccountingTemplate : Aggregate<Guid>
{
    private readonly List<AccountingTemplateAccountLine> _accounts = [];
    private readonly List<AccountingTemplateTaxCodeLine> _taxCodes = [];
    private readonly List<AccountingTemplatePostingProfileLine> _postingProfiles = [];
    private readonly List<AccountingTemplateJournalLine> _journals = [];

    private AccountingTemplate() { }

    public Guid? CompanyId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameAr { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = "SA";
    public string CurrencyCode { get; private set; } = "SAR";
    public AccountingTemplateVisibility Visibility { get; private set; }
    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; }
    public int FiscalYearStartMonth { get; private set; } = 1;
    public int FiscalYearStartDay { get; private set; } = 1;
    public IReadOnlyCollection<AccountingTemplateAccountLine> Accounts => _accounts.Where(x => !x.IsDeleted).OrderBy(x => x.Code).ToList();
    public IReadOnlyCollection<AccountingTemplateTaxCodeLine> TaxCodes => _taxCodes.Where(x => !x.IsDeleted).OrderBy(x => x.Code).ToList();
    public IReadOnlyCollection<AccountingTemplatePostingProfileLine> PostingProfiles => _postingProfiles.Where(x => !x.IsDeleted).OrderBy(x => x.Type).ToList();
    public IReadOnlyCollection<AccountingTemplateJournalLine> Journals => _journals.Where(x => !x.IsDeleted).OrderBy(x => x.Code).ToList();

    public static AccountingTemplate Create(AccountingTemplateDto dto, string userId)
    {
        var template = new AccountingTemplate
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        template.Apply(dto, userId, false);
        return template;
    }

    public void Update(AccountingTemplateDto dto, string userId) => Apply(dto, userId, true);

    public void Deactivate(string userId)
    {
        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private void Apply(AccountingTemplateDto dto, string userId, bool modified)
    {
        Code = dto.Code.Trim().ToUpperInvariant();
        Name = dto.Name.Trim();
        NameAr = dto.NameAr.Trim();
        CountryCode = string.IsNullOrWhiteSpace(dto.CountryCode) ? "SA" : dto.CountryCode.Trim().ToUpperInvariant();
        CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? "SAR" : dto.CurrencyCode.Trim().ToUpperInvariant();
        Visibility = dto.Visibility;
        CompanyId = dto.Visibility == AccountingTemplateVisibility.Private ? dto.CompanyId : null;
        IsSystem = dto.IsSystem;
        IsActive = dto.IsActive;
        FiscalYearStartMonth = dto.FiscalYearStartMonth is >= 1 and <= 12 ? dto.FiscalYearStartMonth : 1;
        FiscalYearStartDay = dto.FiscalYearStartDay is >= 1 and <= 31 ? dto.FiscalYearStartDay : 1;

        _accounts.Clear();
        _accounts.AddRange(dto.Accounts.Select(AccountingTemplateAccountLine.Create));
        _taxCodes.Clear();
        _taxCodes.AddRange(dto.TaxCodes.Select(AccountingTemplateTaxCodeLine.Create));
        _postingProfiles.Clear();
        _postingProfiles.AddRange(dto.PostingProfiles.Select(AccountingTemplatePostingProfileLine.Create));
        _journals.Clear();
        _journals.AddRange(dto.Journals.Select(AccountingTemplateJournalLine.Create));

        if (modified)
        {
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = userId;
        }
    }

    public AccountingTemplateDto ToDto(bool includeLines = true)
    {
        var dto = new AccountingTemplateDto
        {
            Id = Id,
            Code = Code,
            Name = Name,
            NameAr = NameAr,
            CountryCode = CountryCode,
            CurrencyCode = CurrencyCode,
            Visibility = Visibility,
            CompanyId = CompanyId,
            IsSystem = IsSystem,
            IsActive = IsActive,
            FiscalYearStartMonth = FiscalYearStartMonth,
            FiscalYearStartDay = FiscalYearStartDay,
            AccountsCount = Accounts.Count,
            TaxCodesCount = TaxCodes.Count,
            PostingProfilesCount = PostingProfiles.Count,
            JournalsCount = Journals.Count
        };

        if (includeLines)
        {
            dto.Accounts = Accounts.Select(x => x.ToDto()).ToList();
            dto.TaxCodes = TaxCodes.Select(x => x.ToDto()).ToList();
            dto.PostingProfiles = PostingProfiles.Select(x => x.ToDto()).ToList();
            dto.Journals = Journals.Select(x => x.ToDto()).ToList();
        }

        return dto;
    }
}

public class AccountingTemplateAccountLine : Entity<Guid>
{
    private AccountingTemplateAccountLine() { }

    public string TemplateKey { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public NormalBalance NormalBalance { get; private set; }
    public AccountRole Role { get; private set; }
    public string? ParentTemplateKey { get; private set; }
    public bool IsPostingAccount { get; private set; }

    public static AccountingTemplateAccountLine Create(AccountingTemplateAccountDto dto) => new()
    {
        Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
        TemplateKey = dto.TemplateKey.Trim(),
        Code = dto.Code.Trim(),
        Name = dto.Name.Trim(),
        NameEng = dto.NameEng.Trim(),
        Type = dto.Type,
        NormalBalance = dto.NormalBalance,
        Role = dto.Role,
        ParentTemplateKey = string.IsNullOrWhiteSpace(dto.ParentTemplateKey) ? null : dto.ParentTemplateKey.Trim(),
        IsPostingAccount = dto.IsPostingAccount,
        CreatedAt = DateTime.UtcNow
    };

    public AccountingTemplateAccountDto ToDto() => new()
    {
        Id = Id,
        TemplateKey = TemplateKey,
        Code = Code,
        Name = Name,
        NameEng = NameEng,
        Type = Type,
        NormalBalance = NormalBalance,
        Role = Role,
        ParentTemplateKey = ParentTemplateKey,
        IsPostingAccount = IsPostingAccount
    };
}

public class AccountingTemplateTaxCodeLine : Entity<Guid>
{
    private AccountingTemplateTaxCodeLine() { }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Rate { get; private set; }
    public bool IsExempt { get; private set; }
    public string? ZatcaCategoryCode { get; private set; }
    public string? ExemptionReasonCode { get; private set; }
    public bool IsActive { get; private set; }

    public static AccountingTemplateTaxCodeLine Create(AccountingTemplateTaxCodeDto dto) => new()
    {
        Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
        Code = dto.Code.Trim(),
        Name = dto.Name.Trim(),
        Rate = dto.Rate,
        IsExempt = dto.IsExempt,
        ZatcaCategoryCode = string.IsNullOrWhiteSpace(dto.ZatcaCategoryCode) ? null : dto.ZatcaCategoryCode.Trim(),
        ExemptionReasonCode = string.IsNullOrWhiteSpace(dto.ExemptionReasonCode) ? null : dto.ExemptionReasonCode.Trim(),
        IsActive = dto.IsActive,
        CreatedAt = DateTime.UtcNow
    };

    public AccountingTemplateTaxCodeDto ToDto() => new()
    {
        Id = Id,
        Code = Code,
        Name = Name,
        Rate = Rate,
        IsExempt = IsExempt,
        ZatcaCategoryCode = ZatcaCategoryCode,
        ExemptionReasonCode = ExemptionReasonCode,
        IsActive = IsActive
    };
}

public class AccountingTemplatePostingProfileLine : Entity<Guid>
{
    private AccountingTemplatePostingProfileLine() { }

    public PostingProfileType Type { get; private set; }
    public string ReceivableAccountKey { get; private set; } = string.Empty;
    public string PayableAccountKey { get; private set; } = string.Empty;
    public string RevenueAccountKey { get; private set; } = string.Empty;
    public string ExpenseAccountKey { get; private set; } = string.Empty;
    public string OutputVatAccountKey { get; private set; } = string.Empty;
    public string InputVatAccountKey { get; private set; } = string.Empty;
    public string CashAccountKey { get; private set; } = string.Empty;
    public string BankAccountKey { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }

    public static AccountingTemplatePostingProfileLine Create(AccountingTemplatePostingProfileDto dto) => new()
    {
        Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
        Type = dto.Type,
        ReceivableAccountKey = dto.ReceivableAccountKey.Trim(),
        PayableAccountKey = dto.PayableAccountKey.Trim(),
        RevenueAccountKey = dto.RevenueAccountKey.Trim(),
        ExpenseAccountKey = dto.ExpenseAccountKey.Trim(),
        OutputVatAccountKey = dto.OutputVatAccountKey.Trim(),
        InputVatAccountKey = dto.InputVatAccountKey.Trim(),
        CashAccountKey = dto.CashAccountKey.Trim(),
        BankAccountKey = dto.BankAccountKey.Trim(),
        IsDefault = dto.IsDefault,
        CreatedAt = DateTime.UtcNow
    };

    public AccountingTemplatePostingProfileDto ToDto() => new()
    {
        Id = Id,
        Type = Type,
        ReceivableAccountKey = ReceivableAccountKey,
        PayableAccountKey = PayableAccountKey,
        RevenueAccountKey = RevenueAccountKey,
        ExpenseAccountKey = ExpenseAccountKey,
        OutputVatAccountKey = OutputVatAccountKey,
        InputVatAccountKey = InputVatAccountKey,
        CashAccountKey = CashAccountKey,
        BankAccountKey = BankAccountKey,
        IsDefault = IsDefault
    };
}

public class AccountingTemplateJournalLine : Entity<Guid>
{
    private AccountingTemplateJournalLine() { }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameAr { get; private set; } = string.Empty;
    public AccountingJournalType Type { get; private set; }
    public string? DefaultDebitAccountKey { get; private set; }
    public string? DefaultCreditAccountKey { get; private set; }
    public bool IsSystemJournal { get; private set; }
    public bool IsActive { get; private set; }

    public static AccountingTemplateJournalLine Create(AccountingTemplateJournalDto dto) => new()
    {
        Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
        Code = dto.Code.Trim(),
        Name = dto.Name.Trim(),
        NameAr = dto.NameAr.Trim(),
        Type = dto.Type,
        DefaultDebitAccountKey = string.IsNullOrWhiteSpace(dto.DefaultDebitAccountKey) ? null : dto.DefaultDebitAccountKey.Trim(),
        DefaultCreditAccountKey = string.IsNullOrWhiteSpace(dto.DefaultCreditAccountKey) ? null : dto.DefaultCreditAccountKey.Trim(),
        IsSystemJournal = dto.IsSystemJournal,
        IsActive = dto.IsActive,
        CreatedAt = DateTime.UtcNow
    };

    public AccountingTemplateJournalDto ToDto() => new()
    {
        Id = Id,
        Code = Code,
        Name = Name,
        NameAr = NameAr,
        Type = Type,
        DefaultDebitAccountKey = DefaultDebitAccountKey,
        DefaultCreditAccountKey = DefaultCreditAccountKey,
        IsSystemJournal = IsSystemJournal,
        IsActive = IsActive
    };
}

public class JournalEntry : Aggregate<Guid>
{
    private readonly List<JournalEntryLine> _lines = [];
    private JournalEntry() { }

    public Guid CompanyId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public DateTime EntryDate { get; private set; }
    public JournalEntryStatus Status { get; private set; }
    public string? SourceModule { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public string? SourceDocumentNumber { get; private set; }
    public string? Memo { get; private set; }
    public IReadOnlyCollection<JournalEntryLine> Lines => _lines.Where(x => !x.IsDeleted).OrderBy(x => x.LineNumber).ToList();
    public decimal TotalDebit => Lines.Sum(x => x.Debit);
    public decimal TotalCredit => Lines.Sum(x => x.Credit);

    public static JournalEntry Create(Guid companyId, string number, DateTime entryDate, string? sourceModule, Guid? sourceDocumentId, string? sourceDocumentNumber, string? memo, IEnumerable<JournalEntryLineDto> lines, string userId)
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Number = number,
            EntryDate = entryDate.Date,
            Status = JournalEntryStatus.Draft,
            SourceModule = sourceModule,
            SourceDocumentId = sourceDocumentId,
            SourceDocumentNumber = sourceDocumentNumber,
            Memo = memo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var lineNumber = 1;
        foreach (var line in lines)
            entry._lines.Add(JournalEntryLine.Create(lineNumber++, line, userId));

        entry.EnsureBalanced();
        return entry;
    }

    public void Post(string userId)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new BadRequestException("Only draft journal entries can be posted.");

        EnsureBalanced();
        Status = JournalEntryStatus.Posted;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Reverse(string userId)
    {
        if (Status != JournalEntryStatus.Posted)
            throw new BadRequestException("Only posted journal entries can be reversed.");

        Status = JournalEntryStatus.Reversed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private void EnsureBalanced()
    {
        if (!Lines.Any())
            throw new BadRequestException("Journal entry must have lines.");

        if (TotalDebit <= 0 || TotalCredit <= 0 || TotalDebit != TotalCredit)
            throw new BadRequestException("Journal entry must balance before posting.");
    }

    public JournalEntryDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        Number = Number,
        EntryDate = EntryDate,
        Status = Status,
        SourceModule = SourceModule,
        SourceDocumentId = SourceDocumentId,
        SourceDocumentNumber = SourceDocumentNumber,
        Memo = Memo,
        TotalDebit = TotalDebit,
        TotalCredit = TotalCredit,
        Lines = Lines.Select(x => x.ToDto()).ToList()
    };
}

public class JournalEntryLine : Entity<Guid>
{
    private JournalEntryLine() { }

    public int LineNumber { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public string? Description { get; private set; }

    public static JournalEntryLine Create(int lineNumber, JournalEntryLineDto dto, string userId)
    {
        if (dto.Debit < 0 || dto.Credit < 0 || (dto.Debit == 0 && dto.Credit == 0) || (dto.Debit > 0 && dto.Credit > 0))
            throw new BadRequestException("Journal line must contain either debit or credit.");

        return new JournalEntryLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            AccountId = dto.AccountId,
            Debit = dto.Debit,
            Credit = dto.Credit,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public JournalEntryLineDto ToDto() => new()
    {
        Id = Id,
        AccountId = AccountId,
        Debit = Debit,
        Credit = Credit,
        Description = Description
    };
}

public class AccountingDocument : Aggregate<Guid>
{
    private readonly List<AccountingDocumentLine> _lines = [];
    private AccountingDocument() { }

    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public AccountingDocumentType Type { get; private set; }
    public AccountingDocumentStatus Status { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public DateTime DocumentDate { get; private set; }
    public Guid? PartyId { get; private set; }
    public string? PartyName { get; private set; }
    public string? PartyVatNumber { get; private set; }
    public Guid? CurrencyId { get; private set; }
    public string? SourceModule { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public string? SourceDocumentNumber { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Guid? JournalEntryId { get; private set; }
    public IReadOnlyCollection<AccountingDocumentLine> Lines => _lines.Where(x => !x.IsDeleted).OrderBy(x => x.LineNumber).ToList();

    public static AccountingDocument Create(AccountingDocumentDto dto, string number, string userId)
    {
        var document = new AccountingDocument
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            BranchId = dto.BranchId,
            Type = dto.Type,
            Status = AccountingDocumentStatus.Draft,
            Number = number,
            DocumentDate = dto.DocumentDate == default ? DateTime.UtcNow.Date : dto.DocumentDate.Date,
            PartyId = dto.PartyId,
            PartyName = dto.PartyName,
            PartyVatNumber = dto.PartyVatNumber,
            CurrencyId = dto.CurrencyId,
            SourceModule = Clean(dto.SourceModule),
            SourceDocumentId = dto.SourceDocumentId,
            SourceDocumentNumber = dto.SourceDocumentNumber,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var lineNumber = 1;
        foreach (var line in dto.Lines)
            document._lines.Add(AccountingDocumentLine.Create(lineNumber++, line, userId));

        document.Recalculate();
        return document;
    }

    public void Post(Guid journalEntryId, string userId)
    {
        if (Status != AccountingDocumentStatus.Draft)
            throw new BadRequestException("Only draft accounting documents can be posted.");

        JournalEntryId = journalEntryId;
        Status = AccountingDocumentStatus.Posted;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Reverse(string userId)
    {
        if (Status != AccountingDocumentStatus.Posted)
            throw new BadRequestException("Only posted accounting documents can be reversed.");

        Status = AccountingDocumentStatus.Reversed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private void Recalculate()
    {
        Subtotal = Lines.Sum(x => x.NetAmount);
        TaxAmount = Lines.Sum(x => x.TaxAmount);
        TotalAmount = Lines.Sum(x => x.TotalAmount);
    }

    public AccountingDocumentDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        BranchId = BranchId,
        Type = Type,
        Status = Status,
        Number = Number,
        DocumentDate = DocumentDate,
        PartyId = PartyId,
            PartyName = PartyName,
            PartyVatNumber = PartyVatNumber,
            CurrencyId = CurrencyId,
            SourceModule = SourceModule,
            SourceDocumentId = SourceDocumentId,
            SourceDocumentNumber = SourceDocumentNumber,
        Subtotal = Subtotal,
        TaxAmount = TaxAmount,
        TotalAmount = TotalAmount,
        JournalEntryId = JournalEntryId,
        Lines = Lines.Select(x => x.ToDto()).ToList()
    };

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class AccountingDocumentLine : Entity<Guid>
{
    private AccountingDocumentLine() { }

    public int LineNumber { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid? ProductId { get; private set; }
    public Guid? ProductSkuId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxRate { get; private set; }
    public string? TaxCode { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }

    public static AccountingDocumentLine Create(int lineNumber, AccountingDocumentLineDto dto, string userId)
    {
        var netAmount = dto.NetAmount != 0 ? dto.NetAmount : (dto.Quantity * dto.UnitPrice) - dto.DiscountAmount;
        var taxAmount = dto.TaxAmount != 0 ? dto.TaxAmount : netAmount * dto.TaxRate / 100m;

        return new AccountingDocumentLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            Description = dto.Description,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            DiscountAmount = dto.DiscountAmount,
            TaxRate = dto.TaxRate,
            TaxCode = dto.TaxCode,
            NetAmount = netAmount,
            TaxAmount = taxAmount,
            TotalAmount = netAmount + taxAmount,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public AccountingDocumentLineDto ToDto() => new()
    {
        Id = Id,
        LineNumber = LineNumber,
        Description = Description,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        Quantity = Quantity,
        UnitPrice = UnitPrice,
        DiscountAmount = DiscountAmount,
        TaxRate = TaxRate,
        TaxCode = TaxCode,
        NetAmount = NetAmount,
        TaxAmount = TaxAmount,
        TotalAmount = TotalAmount
    };
}

public class ZatcaSettings : Aggregate<Guid>
{
    private ZatcaSettings() { }

    public Guid CompanyId { get; private set; }
    public string SellerName { get; private set; } = string.Empty;
    public string SellerNameAr { get; private set; } = string.Empty;
    public string VatNumber { get; private set; } = string.Empty;
    public string CommercialRegistrationNumber { get; private set; } = string.Empty;
    public string BuildingNumber { get; private set; } = string.Empty;
    public string StreetName { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = "SA";
    public string Environment { get; private set; } = "Sandbox";

    public static ZatcaSettings Upsert(ZatcaSettingsDto dto, string userId) =>
        new()
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            SellerName = dto.SellerName,
            SellerNameAr = dto.SellerNameAr,
            VatNumber = dto.VatNumber,
            CommercialRegistrationNumber = dto.CommercialRegistrationNumber,
            BuildingNumber = dto.BuildingNumber,
            StreetName = dto.StreetName,
            District = dto.District,
            City = dto.City,
            PostalCode = dto.PostalCode,
            CountryCode = string.IsNullOrWhiteSpace(dto.CountryCode) ? "SA" : dto.CountryCode,
            Environment = string.IsNullOrWhiteSpace(dto.Environment) ? "Sandbox" : dto.Environment,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void Update(ZatcaSettingsDto dto, string userId)
    {
        SellerName = dto.SellerName;
        SellerNameAr = dto.SellerNameAr;
        VatNumber = dto.VatNumber;
        CommercialRegistrationNumber = dto.CommercialRegistrationNumber;
        BuildingNumber = dto.BuildingNumber;
        StreetName = dto.StreetName;
        District = dto.District;
        City = dto.City;
        PostalCode = dto.PostalCode;
        CountryCode = string.IsNullOrWhiteSpace(dto.CountryCode) ? "SA" : dto.CountryCode;
        Environment = string.IsNullOrWhiteSpace(dto.Environment) ? "Sandbox" : dto.Environment;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public ZatcaSettingsDto ToDto(string? activeDeviceName = null) => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        SellerName = SellerName,
        SellerNameAr = SellerNameAr,
        VatNumber = VatNumber,
        CommercialRegistrationNumber = CommercialRegistrationNumber,
        BuildingNumber = BuildingNumber,
        StreetName = StreetName,
        District = District,
        City = City,
        PostalCode = PostalCode,
        CountryCode = CountryCode,
        Environment = Environment,
        ActiveDeviceName = activeDeviceName
    };
}

public class ZatcaDevice : Aggregate<Guid>
{
    private ZatcaDevice() { }

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Csid { get; private set; }
    public string? CertificatePem { get; private set; }
    public string? PrivateKeyReference { get; private set; }
    public bool IsActive { get; private set; }

    public static ZatcaDevice Create(Guid companyId, string name, string? csid, string? certificatePem, string? privateKeyReference, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = name,
            Csid = csid,
            CertificatePem = certificatePem,
            PrivateKeyReference = privateKeyReference,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
}

public class EInvoice : Aggregate<Guid>
{
    private EInvoice() { }

    public Guid CompanyId { get; private set; }
    public Guid AccountingDocumentId { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public ZatcaInvoiceType InvoiceType { get; private set; }
    public Guid Uuid { get; private set; }
    public long Icv { get; private set; }
    public string? PreviousInvoiceHash { get; private set; }
    public string InvoiceHash { get; private set; } = string.Empty;
    public string QrPayload { get; private set; } = string.Empty;
    public string XmlPayload { get; private set; } = string.Empty;
    public ZatcaSubmissionStatus SubmissionStatus { get; private set; }

    public static EInvoice Create(AccountingDocument document, ZatcaInvoiceType invoiceType, long icv, string? previousHash, string xmlPayload, string invoiceHash, string qrPayload, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            CompanyId = document.CompanyId,
            AccountingDocumentId = document.Id,
            InvoiceNumber = document.Number,
            InvoiceType = invoiceType,
            Uuid = Guid.NewGuid(),
            Icv = icv,
            PreviousInvoiceHash = previousHash,
            XmlPayload = xmlPayload,
            InvoiceHash = invoiceHash,
            QrPayload = qrPayload,
            SubmissionStatus = ZatcaSubmissionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public void MarkSubmitted(ZatcaSubmissionStatus status, string userId)
    {
        SubmissionStatus = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public EInvoiceDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        AccountingDocumentId = AccountingDocumentId,
        InvoiceNumber = InvoiceNumber,
        InvoiceType = InvoiceType,
        Uuid = Uuid,
        Icv = Icv,
        PreviousInvoiceHash = PreviousInvoiceHash,
        InvoiceHash = InvoiceHash,
        QrPayload = QrPayload,
        XmlPayload = XmlPayload,
        SubmissionStatus = SubmissionStatus,
        CreatedAt = CreatedAt ?? DateTime.UtcNow
    };
}

public class EInvoiceSubmission : Aggregate<Guid>
{
    private EInvoiceSubmission() { }

    public Guid EInvoiceId { get; private set; }
    public ZatcaSubmissionStatus Status { get; private set; }
    public string? RequestPayload { get; private set; }
    public string? ResponsePayload { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    public static EInvoiceSubmission Create(Guid eInvoiceId, ZatcaSubmissionStatus status, string? requestPayload, string? responsePayload, string? errorMessage, int retryCount, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            EInvoiceId = eInvoiceId,
            Status = status,
            RequestPayload = requestPayload,
            ResponsePayload = responsePayload,
            ErrorMessage = errorMessage,
            RetryCount = retryCount,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public EInvoiceSubmissionDto ToDto() => new()
    {
        Id = Id,
        EInvoiceId = EInvoiceId,
        Status = Status,
        RequestPayload = RequestPayload,
        ResponsePayload = ResponsePayload,
        ErrorMessage = ErrorMessage,
        RetryCount = RetryCount,
        CreatedAt = CreatedAt ?? DateTime.UtcNow
    };
}
