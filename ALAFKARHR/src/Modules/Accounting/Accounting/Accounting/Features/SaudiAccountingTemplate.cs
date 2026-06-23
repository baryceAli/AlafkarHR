namespace Accounting.Accounting.Features;

public static class SaudiAccountingTemplate
{
    public const string Code = "SA_SME";

    public static AccountingTemplateDto Template => new()
    {
        Code = Code,
        Name = "Saudi SME Chart of Accounts",
        NameAr = "دليل حسابات سعودي للمنشآت الصغيرة والمتوسطة",
        CountryCode = "SA",
        CurrencyCode = "SAR",
        Visibility = AccountingTemplateVisibility.Shared,
        IsSystem = true,
        IsActive = true,
        FiscalYearStartMonth = 1,
        FiscalYearStartDay = 1,
        AccountsCount = Accounts.Count,
        TaxCodesCount = 3,
        PostingProfilesCount = RequiredPostingProfiles.Count,
        JournalsCount = RequiredJournalCodes.Count,
        Accounts = Accounts.Select(x => new AccountingTemplateAccountDto
        {
            TemplateKey = x.TemplateKey,
            Code = x.Code,
            Name = x.Name,
            NameEng = x.NameEng,
            Type = x.Type,
            NormalBalance = x.NormalBalance,
            Role = x.Role,
            ParentTemplateKey = x.ParentTemplateKey,
            IsPostingAccount = x.IsPostingAccount
        }).ToList(),
        TaxCodes =
        [
            new() { Code = "VAT15", Name = "VAT 15%", Rate = 15, ZatcaCategoryCode = "S", IsActive = true },
            new() { Code = "VAT0", Name = "VAT 0%", Rate = 0, ZatcaCategoryCode = "Z", IsActive = true },
            new() { Code = "VATEX", Name = "VAT Exempt", Rate = 0, IsExempt = true, ZatcaCategoryCode = "E", ExemptionReasonCode = "VATEX-SA", IsActive = true }
        ],
        PostingProfiles =
        [
            Profile(PostingProfileType.Sales),
            Profile(PostingProfileType.Purchases),
            Profile(PostingProfileType.CustomerReceipt),
            Profile(PostingProfileType.SupplierPayment)
        ],
        Journals =
        [
            JournalTemplate("SAL", "Sales Journal", "Sales Journal", AccountingJournalType.Sales, "SA_RECEIVABLE", "SA_SALES_REVENUE"),
            JournalTemplate("PUR", "Purchase Journal", "Purchase Journal", AccountingJournalType.Purchase, "SA_PURCHASES_EXPENSE", "SA_PAYABLE"),
            JournalTemplate("CSH", "Cash Journal", "Cash Journal", AccountingJournalType.Cash, "SA_CASH", "SA_CASH"),
            JournalTemplate("BNK", "Bank Journal", "Bank Journal", AccountingJournalType.Bank, "SA_BANK", "SA_BANK"),
            JournalTemplate("CC", "Credit Card Journal", "Credit Card Journal", AccountingJournalType.CreditCard, "SA_BANK", "SA_BANK"),
            JournalTemplate("MISC", "Miscellaneous Journal", "Miscellaneous Journal", AccountingJournalType.Miscellaneous, "SA_SUSPENSE", "SA_SUSPENSE")
        ]
    };

    public static IReadOnlyList<AccountTemplateLine> Accounts { get; } =
    [
        Group("SA_ASSETS", "1000", "Assets", "الأصول", AccountType.Asset, NormalBalance.Debit),
        Group("SA_CURRENT_ASSETS", "100001", "Current Assets", "الأصول المتداولة", AccountType.Asset, NormalBalance.Debit, "SA_ASSETS"),
        Group("SA_FIXED_ASSETS", "100002", "Fixed Assets", "الأصول الثابتة", AccountType.Asset, NormalBalance.Debit, "SA_ASSETS"),
        Group("SA_LIABILITIES", "1001", "Liabilities", "الالتزامات", AccountType.Liability, NormalBalance.Credit),
        Group("SA_CURRENT_LIABILITIES", "100101", "Current Liabilities", "الالتزامات المتداولة", AccountType.Liability, NormalBalance.Credit, "SA_LIABILITIES"),
        Group("SA_EQUITY_GROUP", "1002", "Equity", "حقوق الملكية", AccountType.Equity, NormalBalance.Credit),
        Group("SA_INCOME", "1003", "Income", "الإيرادات", AccountType.Revenue, NormalBalance.Credit),
        Group("SA_EXPENSES", "1004", "Expenses", "المصروفات", AccountType.Expense, NormalBalance.Debit),

        Ledger("SA_CASH", "100001001", "Cash on Hand", "الصندوق", AccountType.Asset, NormalBalance.Debit, AccountRole.Cash, "SA_CURRENT_ASSETS"),
        Ledger("SA_BANK", "100001002", "Bank Account", "الحساب البنكي", AccountType.Asset, NormalBalance.Debit, AccountRole.Bank, "SA_CURRENT_ASSETS"),
        Ledger("SA_RECEIVABLE", "100001003", "Accounts Receivable", "الذمم المدينة", AccountType.Asset, NormalBalance.Debit, AccountRole.Receivable, "SA_CURRENT_ASSETS"),
        Ledger("SA_INVENTORY", "100001004", "Inventory", "المخزون", AccountType.Asset, NormalBalance.Debit, AccountRole.Inventory, "SA_CURRENT_ASSETS"),
        Ledger("SA_INPUT_VAT", "100001005", "Input VAT", "ضريبة المدخلات", AccountType.Asset, NormalBalance.Debit, AccountRole.InputVat, "SA_CURRENT_ASSETS"),
        Ledger("SA_PAYABLE", "100101001", "Accounts Payable", "الذمم الدائنة", AccountType.Liability, NormalBalance.Credit, AccountRole.Payable, "SA_CURRENT_LIABILITIES"),
        Ledger("SA_OUTPUT_VAT", "100101002", "Output VAT", "ضريبة المخرجات", AccountType.Liability, NormalBalance.Credit, AccountRole.OutputVat, "SA_CURRENT_LIABILITIES"),
        Ledger("SA_VAT_SETTLEMENT", "100101003", "VAT Settlement", "تسوية ضريبة القيمة المضافة", AccountType.Liability, NormalBalance.Credit, AccountRole.VatSettlement, "SA_CURRENT_LIABILITIES"),
        Ledger("SA_OWNER_CAPITAL", "1002001", "Owner Capital", "رأس المال", AccountType.Equity, NormalBalance.Credit, AccountRole.Equity, "SA_EQUITY_GROUP"),
        Ledger("SA_RETAINED_EARNINGS", "1002002", "Retained Earnings", "الأرباح المبقاة", AccountType.Equity, NormalBalance.Credit, AccountRole.RetainedEarnings, "SA_EQUITY_GROUP"),
        Ledger("SA_SALES_REVENUE", "1003001", "Sales Revenue", "إيرادات المبيعات", AccountType.Revenue, NormalBalance.Credit, AccountRole.Revenue, "SA_INCOME"),
        Ledger("SA_SALES_RETURNS", "1003002", "Sales Returns", "مردودات المبيعات", AccountType.Revenue, NormalBalance.Debit, AccountRole.SalesReturn, "SA_INCOME"),
        Ledger("SA_COGS", "1004001", "Cost of Goods Sold", "تكلفة البضاعة المباعة", AccountType.Expense, NormalBalance.Debit, AccountRole.Cogs, "SA_EXPENSES"),
        Ledger("SA_PURCHASES_EXPENSE", "1004002", "Purchases / Direct Expense", "المشتريات / المصروف المباشر", AccountType.Expense, NormalBalance.Debit, AccountRole.Expense, "SA_EXPENSES"),
        Ledger("SA_OPERATING_EXPENSES", "1004003", "Operating Expenses", "المصروفات التشغيلية", AccountType.Expense, NormalBalance.Debit, AccountRole.Expense, "SA_EXPENSES"),
        Ledger("SA_BANK_CHARGES", "1004004", "Bank Charges", "مصاريف بنكية", AccountType.Expense, NormalBalance.Debit, AccountRole.Expense, "SA_EXPENSES"),
        Ledger("SA_ROUNDING", "1004005", "Rounding Difference", "فروقات التقريب", AccountType.Expense, NormalBalance.Debit, AccountRole.Rounding, "SA_EXPENSES"),
        Ledger("SA_SUSPENSE", "1000001", "Suspense", "حساب معلق", AccountType.Asset, NormalBalance.Debit, AccountRole.Suspense, "SA_ASSETS")
    ];

    public static IReadOnlyList<TaxCodeDto> TaxCodes(Guid companyId) =>
    [
        new() { CompanyId = companyId, Code = "VAT15", Name = "VAT 15%", Rate = 15, ZatcaCategoryCode = "S", IsActive = true },
        new() { CompanyId = companyId, Code = "VAT0", Name = "VAT 0%", Rate = 0, ZatcaCategoryCode = "Z", IsActive = true },
        new() { CompanyId = companyId, Code = "VATEX", Name = "VAT Exempt", Rate = 0, IsExempt = true, ZatcaCategoryCode = "E", ExemptionReasonCode = "VATEX-SA", IsActive = true }
    ];

    public static IReadOnlyList<AccountingJournalDto> Journals(Guid companyId, IReadOnlyDictionary<AccountRole, Guid> roleAccounts) =>
    [
        Journal(companyId, "SAL", "Sales Journal", "دفتر يومية المبيعات", AccountingJournalType.Sales, roleAccounts[AccountRole.Receivable], roleAccounts[AccountRole.Revenue]),
        Journal(companyId, "PUR", "Purchase Journal", "دفتر يومية المشتريات", AccountingJournalType.Purchase, roleAccounts[AccountRole.Expense], roleAccounts[AccountRole.Payable]),
        Journal(companyId, "CSH", "Cash Journal", "دفتر يومية الصندوق", AccountingJournalType.Cash, roleAccounts[AccountRole.Cash], roleAccounts[AccountRole.Cash]),
        Journal(companyId, "BNK", "Bank Journal", "دفتر يومية البنك", AccountingJournalType.Bank, roleAccounts[AccountRole.Bank], roleAccounts[AccountRole.Bank]),
        Journal(companyId, "CC", "Credit Card Journal", "دفتر يومية البطاقات", AccountingJournalType.CreditCard, roleAccounts[AccountRole.Bank], roleAccounts[AccountRole.Bank]),
        Journal(companyId, "MISC", "Miscellaneous Journal", "دفتر يومية عام", AccountingJournalType.Miscellaneous, roleAccounts[AccountRole.Suspense], roleAccounts[AccountRole.Suspense])
    ];

    public static IReadOnlyList<string> RequiredTaxCodes { get; } = ["VAT15", "VAT0", "VATEX"];
    public static IReadOnlyList<PostingProfileType> RequiredPostingProfiles { get; } = [PostingProfileType.Sales, PostingProfileType.Purchases, PostingProfileType.CustomerReceipt, PostingProfileType.SupplierPayment];
    public static IReadOnlyList<string> RequiredJournalCodes { get; } = ["SAL", "PUR", "CSH", "BNK", "CC", "MISC"];

    private static AccountTemplateLine Group(string key, string code, string name, string nameAr, AccountType type, NormalBalance balance, string? parentKey = null) =>
        new(key, code, nameAr, name, type, balance, AccountRole.None, parentKey, false);

    private static AccountTemplateLine Ledger(string key, string code, string name, string nameAr, AccountType type, NormalBalance balance, AccountRole role, string? parentKey = null) =>
        new(key, code, nameAr, name, type, balance, role, parentKey, true);

    private static AccountingJournalDto Journal(Guid companyId, string code, string name, string nameAr, AccountingJournalType type, Guid debitAccountId, Guid creditAccountId) => new()
    {
        CompanyId = companyId,
        Code = code,
        Name = name,
        NameAr = nameAr,
        Type = type,
        DefaultDebitAccountId = debitAccountId,
        DefaultCreditAccountId = creditAccountId,
        IsSystemJournal = true,
        IsActive = true
    };

    private static AccountingTemplatePostingProfileDto Profile(PostingProfileType type) => new()
    {
        Type = type,
        ReceivableAccountKey = "SA_RECEIVABLE",
        PayableAccountKey = "SA_PAYABLE",
        RevenueAccountKey = "SA_SALES_REVENUE",
        ExpenseAccountKey = "SA_PURCHASES_EXPENSE",
        OutputVatAccountKey = "SA_OUTPUT_VAT",
        InputVatAccountKey = "SA_INPUT_VAT",
        CashAccountKey = "SA_CASH",
        BankAccountKey = "SA_BANK",
        IsDefault = true
    };

    private static AccountingTemplateJournalDto JournalTemplate(string code, string name, string nameAr, AccountingJournalType type, string debitKey, string creditKey) => new()
    {
        Code = code,
        Name = name,
        NameAr = nameAr,
        Type = type,
        DefaultDebitAccountKey = debitKey,
        DefaultCreditAccountKey = creditKey,
        IsSystemJournal = true,
        IsActive = true
    };
}

public record AccountTemplateLine(
    string TemplateKey,
    string Code,
    string Name,
    string NameEng,
    AccountType Type,
    NormalBalance NormalBalance,
    AccountRole Role,
    string? ParentTemplateKey,
    bool IsPostingAccount);
