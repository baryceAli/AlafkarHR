using Shared.Contracts.CQRS;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;

namespace Accounting.Contracts.Accounting.Features;

public record CreateAccountingDocumentCommand(AccountingDocumentDto Document) : ICommand<CreateAccountingDocumentResult>;

public record CreateAccountingDocumentResult(Guid Id, string Number);

public record PostAccountingDocumentCommand(Guid Id) : ICommand<PostAccountingDocumentResult>;

public record PostAccountingDocumentResult(Guid JournalEntryId);

public record CreateAndPostJournalEntryCommand(CreateJournalEntryDto JournalEntry) : ICommand<CreateAndPostJournalEntryResult>;

public record CreateAndPostJournalEntryResult(Guid JournalEntryId, string Number);

public record GetAccountingCashAccountScopeQuery(Guid CompanyId, Guid BranchId, Guid CashAccountId)
    : IQuery<GetAccountingCashAccountScopeResult>;

public record GetAccountingCashAccountScopeResult(Guid CashAccountId, Guid LedgerAccountId);

public record GetAccountingCashAccountsQuery(Guid CompanyId, Guid? BranchId)
    : IQuery<GetAccountingCashAccountsResult>;

public record GetAccountingCashAccountsResult(List<CashAccountDto> CashAccounts);

public record UpsertAccountingCashAccountCommand(CashAccountDto CashAccount)
    : ICommand<UpsertAccountingCashAccountResult>;

public record UpsertAccountingCashAccountResult(Guid Id);

public record RecordAccountingReceiptCommand(
    Guid CompanyId,
    Guid? BranchId,
    Guid? PartyId,
    string? PartyName,
    string? SourceModule,
    Guid? SourceDocumentId,
    string? SourceDocumentNumber,
    decimal Amount,
    bool ToBank,
    DateTime ReceiptDate,
    Guid? CashAccountId = null,
    Guid? BankAccountId = null) : ICommand<CreateAccountingDocumentResult>;

public record GenerateZatcaInvoiceCommand(Guid AccountingDocumentId, ZatcaInvoiceType InvoiceType) : ICommand<GenerateZatcaInvoiceResult>;

public record GenerateZatcaInvoiceResult(Guid EInvoiceId, string InvoiceHash, string QrPayload);
