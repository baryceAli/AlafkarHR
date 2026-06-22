namespace SharedWithUI.Accounting.Enums;

public enum AccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Revenue = 4,
    Expense = 5
}

public enum NormalBalance
{
    Debit = 1,
    Credit = 2
}

public enum AccountRole
{
    None = 0,
    Receivable = 1,
    Payable = 2,
    Cash = 3,
    Bank = 4,
    Revenue = 5,
    SalesReturn = 6,
    Expense = 7,
    Cogs = 8,
    Inventory = 9,
    InputVat = 10,
    OutputVat = 11,
    VatSettlement = 12,
    Equity = 13,
    RetainedEarnings = 14,
    Suspense = 15,
    Rounding = 16
}

public enum FiscalPeriodStatus
{
    Open = 1,
    Closed = 2,
    Locked = 3
}

public enum JournalEntryStatus
{
    Draft = 1,
    Posted = 2,
    Reversed = 3
}

public enum AccountingJournalType
{
    Sales = 1,
    Purchase = 2,
    Cash = 3,
    Bank = 4,
    CreditCard = 5,
    Miscellaneous = 6
}

public enum AccountingDocumentType
{
    SalesInvoice = 1,
    SalesCreditNote = 2,
    SalesDebitNote = 3,
    SupplierInvoice = 4,
    CustomerReceipt = 5,
    SupplierPayment = 6,
    SupplierCreditNote = 7
}

public enum AccountingDocumentStatus
{
    Draft = 1,
    Posted = 2,
    Reversed = 3,
    Cancelled = 4
}

public enum PostingProfileType
{
    Sales = 1,
    Purchases = 2,
    CustomerReceipt = 3,
    SupplierPayment = 4
}

public enum AccountingTemplateVisibility
{
    Private = 1,
    Shared = 2
}

public enum ZatcaInvoiceType
{
    StandardTaxInvoice = 1,
    SimplifiedTaxInvoice = 2,
    CreditNote = 3,
    DebitNote = 4
}

public enum ZatcaSubmissionStatus
{
    Pending = 1,
    Cleared = 2,
    Reported = 3,
    Failed = 4,
    RetryScheduled = 5
}

public enum BankTransactionStatus
{
    Unreconciled = 1,
    Reconciled = 2,
    Ignored = 3
}

public enum AccountingReportType
{
    GeneralLedger = 1,
    TrialBalance = 2,
    AgedReceivables = 3,
    AgedPayables = 4,
    TaxSummary = 5
}
