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
    DateTime ReceiptDate) : ICommand<CreateAccountingDocumentResult>;

public record GenerateZatcaInvoiceCommand(Guid AccountingDocumentId, ZatcaInvoiceType InvoiceType) : ICommand<GenerateZatcaInvoiceResult>;

public record GenerateZatcaInvoiceResult(Guid EInvoiceId, string InvoiceHash, string QrPayload);
