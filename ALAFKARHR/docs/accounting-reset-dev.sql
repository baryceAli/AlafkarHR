-- Development-only Accounting reset for local HRDb.
-- Run this only against a non-production database before reapplying the SA_SME template.

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DELETE FROM [Accounting].[BankTransactions];

    DELETE FROM [Accounting].[EInvoiceSubmissions];
    DELETE FROM [Accounting].[EInvoices];
    DELETE FROM [Accounting].[ZatcaDevices];
    DELETE FROM [Accounting].[ZatcaSettings];

    DELETE FROM [Accounting].[AccountingDocumentLines];
    DELETE FROM [Accounting].[AccountingDocuments];

    DELETE FROM [Accounting].[JournalEntryLines];
    DELETE FROM [Accounting].[JournalEntries];

    DELETE FROM [Accounting].[BankAccounts];
    DELETE FROM [Accounting].[CashAccounts];
    DELETE FROM [Accounting].[PostingProfiles];
    DELETE FROM [Accounting].[CompanyAccountingSettings];
    DELETE FROM [Accounting].[AccountCodingSettings];
    DELETE FROM [Accounting].[FiscalPeriods];
    DELETE FROM [Accounting].[TaxCodes];
    DELETE FROM [Accounting].[AccountingJournals];
    DELETE FROM [Accounting].[Accounts];

    DELETE FROM [Accounting].[AccountingTemplateAccounts];
    DELETE FROM [Accounting].[AccountingTemplateJournals];
    DELETE FROM [Accounting].[AccountingTemplatePostingProfiles];
    DELETE FROM [Accounting].[AccountingTemplateTaxCodes];
    DELETE FROM [Accounting].[AccountingTemplates];

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
