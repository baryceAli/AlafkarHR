using SharedWithUI.Accounting.Enums;

namespace SharedWithUI.Accounting.Dtos;

public class AccountDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public NormalBalance NormalBalance { get; set; }
    public AccountRole Role { get; set; } = AccountRole.None;
    public string? TemplateKey { get; set; }
    public Guid? ParentAccountId { get; set; }
    public bool IsPostingAccount { get; set; } = true;
    public bool IsSystemAccount { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FiscalPeriodDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public FiscalPeriodStatus Status { get; set; } = FiscalPeriodStatus.Open;
}

public class TaxCodeDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsExempt { get; set; }
    public string? ZatcaCategoryCode { get; set; }
    public string? ExemptionReasonCode { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PostingProfileDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public PostingProfileType Type { get; set; }
    public Guid ReceivableAccountId { get; set; }
    public Guid PayableAccountId { get; set; }
    public Guid RevenueAccountId { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public Guid OutputVatAccountId { get; set; }
    public Guid InputVatAccountId { get; set; }
    public Guid CashAccountId { get; set; }
    public Guid BankAccountId { get; set; }
    public bool IsDefault { get; set; }
}

public class BankAccountDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string? Iban { get; set; }
    public string? BranchCode { get; set; }
    public string? Swift { get; set; }
    public string CurrencyCode { get; set; } = "SAR";
    public Guid? LedgerAccountId { get; set; }
    public Guid? JournalId { get; set; }
    public bool CreateLinkedLedger { get; set; } = true;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CashAccountDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "SAR";
    public Guid? LedgerAccountId { get; set; }
    public Guid? JournalId { get; set; }
    public bool CreateLinkedLedger { get; set; } = true;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CompanyAccountingSettingsDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? ReceivableAccountId { get; set; }
    public Guid? PayableAccountId { get; set; }
    public Guid? RevenueAccountId { get; set; }
    public Guid? ExpenseAccountId { get; set; }
    public Guid? CogsAccountId { get; set; }
    public Guid? InventoryAccountId { get; set; }
    public Guid? InputVatAccountId { get; set; }
    public Guid? OutputVatAccountId { get; set; }
    public Guid? VatSettlementAccountId { get; set; }
    public Guid? CashAccountId { get; set; }
    public Guid? BankAccountId { get; set; }
    public Guid? RoundingAccountId { get; set; }
    public Guid? SuspenseAccountId { get; set; }
    public Guid? RetainedEarningsAccountId { get; set; }
    public int FiscalYearStartMonth { get; set; } = 1;
    public int FiscalYearStartDay { get; set; } = 1;
}

public class AccountingJournalDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public AccountingJournalType Type { get; set; }
    public Guid? DefaultDebitAccountId { get; set; }
    public Guid? DefaultCreditAccountId { get; set; }
    public string? ZatcaDeviceSerial { get; set; }
    public bool IsSystemJournal { get; set; }
    public bool IsActive { get; set; } = true;
}

public class JournalEntryDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;
    public string? SourceModule { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public string? Memo { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public List<JournalEntryLineDto> Lines { get; set; } = [];
}

public class CreateJournalEntryDto
{
    public Guid CompanyId { get; set; }
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public string? SourceModule { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public string? Memo { get; set; }
    public List<JournalEntryLineDto> Lines { get; set; } = [];
}

public class JournalEntryLineDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public AccountRole AccountRole { get; set; } = AccountRole.None;
    public string? AccountCode { get; set; }
    public string? AccountName { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string? Description { get; set; }
}

public class AccountingDocumentDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public AccountingDocumentType Type { get; set; }
    public AccountingDocumentStatus Status { get; set; } = AccountingDocumentStatus.Draft;
    public string Number { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; } = DateTime.UtcNow;
    public Guid? PartyId { get; set; }
    public string? PartyName { get; set; }
    public string? PartyVatNumber { get; set; }
    public Guid? CurrencyId { get; set; }
    public string? SourceModule { get; set; }
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? JournalEntryId { get; set; }
    public List<AccountingDocumentLineDto> Lines { get; set; } = [];
}

public class AccountingDocumentLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? ProductId { get; set; }
    public Guid? ProductSkuId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxRate { get; set; }
    public string? TaxCode { get; set; }
    public decimal NetAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class ZatcaSettingsDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string SellerNameAr { get; set; } = string.Empty;
    public string VatNumber { get; set; } = string.Empty;
    public string CommercialRegistrationNumber { get; set; } = string.Empty;
    public string BuildingNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "SA";
    public string Environment { get; set; } = "Sandbox";
    public string? ActiveDeviceName { get; set; }
}

public class EInvoiceDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid AccountingDocumentId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public ZatcaInvoiceType InvoiceType { get; set; }
    public Guid Uuid { get; set; }
    public long Icv { get; set; }
    public string? PreviousInvoiceHash { get; set; }
    public string? InvoiceHash { get; set; }
    public string? QrPayload { get; set; }
    public string? XmlPayload { get; set; }
    public ZatcaSubmissionStatus SubmissionStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EInvoiceSubmissionDto
{
    public Guid Id { get; set; }
    public Guid EInvoiceId { get; set; }
    public ZatcaSubmissionStatus Status { get; set; }
    public string? RequestPayload { get; set; }
    public string? ResponsePayload { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AccountingDashboardDto
{
    public int Accounts { get; set; }
    public int OpenPeriods { get; set; }
    public int DraftDocuments { get; set; }
    public int PostedDocuments { get; set; }
    public int PendingZatcaSubmissions { get; set; }
    public int FailedZatcaSubmissions { get; set; }
    public decimal OutputVat { get; set; }
    public decimal InputVat { get; set; }
}

public class AccountingTemplateDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "SA";
    public string CurrencyCode { get; set; } = "SAR";
    public AccountingTemplateVisibility Visibility { get; set; } = AccountingTemplateVisibility.Private;
    public Guid? CompanyId { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public int FiscalYearStartMonth { get; set; } = 1;
    public int FiscalYearStartDay { get; set; } = 1;
    public int AccountsCount { get; set; }
    public int TaxCodesCount { get; set; }
    public int PostingProfilesCount { get; set; }
    public int JournalsCount { get; set; }
    public List<AccountingTemplateAccountDto> Accounts { get; set; } = [];
    public List<AccountingTemplateTaxCodeDto> TaxCodes { get; set; } = [];
    public List<AccountingTemplatePostingProfileDto> PostingProfiles { get; set; } = [];
    public List<AccountingTemplateJournalDto> Journals { get; set; } = [];
}

public class AccountingTemplateAccountDto
{
    public Guid Id { get; set; }
    public string TemplateKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEng { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public NormalBalance NormalBalance { get; set; }
    public AccountRole Role { get; set; } = AccountRole.None;
    public string? ParentTemplateKey { get; set; }
    public bool IsPostingAccount { get; set; } = true;
}

public class AccountingTemplateTaxCodeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsExempt { get; set; }
    public string? ZatcaCategoryCode { get; set; }
    public string? ExemptionReasonCode { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AccountingTemplatePostingProfileDto
{
    public Guid Id { get; set; }
    public PostingProfileType Type { get; set; }
    public string ReceivableAccountKey { get; set; } = string.Empty;
    public string PayableAccountKey { get; set; } = string.Empty;
    public string RevenueAccountKey { get; set; } = string.Empty;
    public string ExpenseAccountKey { get; set; } = string.Empty;
    public string OutputVatAccountKey { get; set; } = string.Empty;
    public string InputVatAccountKey { get; set; } = string.Empty;
    public string CashAccountKey { get; set; } = string.Empty;
    public string BankAccountKey { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = true;
}

public class AccountingTemplateJournalDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public AccountingJournalType Type { get; set; }
    public string? DefaultDebitAccountKey { get; set; }
    public string? DefaultCreditAccountKey { get; set; }
    public bool IsSystemJournal { get; set; } = true;
    public bool IsActive { get; set; } = true;
}

public class CaptureAccountingTemplateDto
{
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "SA";
    public string CurrencyCode { get; set; } = "SAR";
    public AccountingTemplateVisibility Visibility { get; set; } = AccountingTemplateVisibility.Private;
}

public class ApplyAccountingTemplateDto
{
    public Guid CompanyId { get; set; }
    public Guid? TemplateId { get; set; }
    public string TemplateCode { get; set; } = "SA_SME";
    public DateTime FiscalYearStart { get; set; } = new(DateTime.UtcNow.Year, 1, 1);
    public bool CreateDefaultJournals { get; set; } = true;
}

public class ApplyAccountingTemplateResultDto
{
    public int AccountsCreated { get; set; }
    public int TaxCodesCreated { get; set; }
    public int PostingProfilesCreated { get; set; }
    public int JournalsCreated { get; set; }
    public int FiscalPeriodsCreated { get; set; }
}

public class AccountingSetupStatusDto
{
    public Guid CompanyId { get; set; }
    public bool ChartExists { get; set; }
    public int MinimumAccountsMissing { get; set; }
    public bool DefaultTaxCodesExist { get; set; }
    public bool CompanyDefaultsComplete { get; set; }
    public bool DefaultBankAccountExists { get; set; }
    public bool DefaultCashAccountExists { get; set; }
    public bool PostingProfilesExist { get; set; }
    public bool JournalsExist { get; set; }
    public bool FiscalPeriodExists { get; set; }
    public bool ZatcaSettingsComplete { get; set; }
    public bool ReadyToPost { get; set; }
    public List<string> MissingItems { get; set; } = [];
}
