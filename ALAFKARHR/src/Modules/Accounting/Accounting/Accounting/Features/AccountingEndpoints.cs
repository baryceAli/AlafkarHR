namespace Accounting.Accounting.Features;

public class AccountingEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var baseRoute = "/api/v1/accounting";

        app.MapGet($"{baseRoute}/dashboard", async (Guid? companyId, Guid? branchId, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountingDashboardQuery(companyId, branchId))))
            .RequireAuthorization(PermissionList.AccountingDashboardPermissions.View);

        app.MapGet($"{baseRoute}/reports", async (AccountingReportType type, Guid companyId, Guid? branchId, DateTime? fromDate, DateTime? toDate, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountingReportQuery(type, companyId, branchId, fromDate, toDate))))
            .RequireAuthorization(PermissionList.AccountingReportPermissions.View);

        app.MapGet($"{baseRoute}/setup/templates", async (Guid? companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountingTemplatesQuery(companyId))))
            .RequireAuthorization(PermissionList.AccountingTemplatePermissions.View);

        app.MapGet($"{baseRoute}/setup/templates/{{id:guid}}", async (Guid id, Guid? companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountingTemplateByIdQuery(id, companyId))))
            .RequireAuthorization(PermissionList.AccountingTemplatePermissions.View);

        app.MapPost($"{baseRoute}/setup/templates", async (AccountingTemplateDto template, ISender sender) =>
            Results.Created($"{baseRoute}/setup/templates", await sender.Send(new UpsertAccountingTemplateCommand(template))))
            .RequireAuthorization(PermissionList.AccountingTemplatePermissions.Create);

        app.MapPut($"{baseRoute}/setup/templates/{{id:guid}}", async (Guid id, AccountingTemplateDto template, ISender sender) =>
        {
            template.Id = id;
            return Results.Ok(await sender.Send(new UpsertAccountingTemplateCommand(template)));
        })
            .RequireAuthorization(PermissionList.AccountingTemplatePermissions.Edit);

        app.MapDelete($"{baseRoute}/setup/templates/{{id:guid}}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new DeleteAccountingTemplateCommand(id))))
            .RequireAuthorization(PermissionList.AccountingTemplatePermissions.Delete);

        app.MapPost($"{baseRoute}/setup/templates/capture-current", async (CaptureAccountingTemplateDto template, ISender sender) =>
            Results.Created($"{baseRoute}/setup/templates", await sender.Send(new CaptureAccountingTemplateCommand(template))))
            .RequireAuthorization(PermissionList.AccountingTemplatePermissions.Create);

        app.MapGet($"{baseRoute}/setup/status", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountingSetupStatusQuery(companyId))))
            .RequireAuthorization(PermissionList.AccountingDashboardPermissions.View);

        app.MapPost($"{baseRoute}/setup/apply-template", async (ApplyAccountingTemplateDto setup, ISender sender) =>
            Results.Ok(await sender.Send(new ApplyAccountingTemplateCommand(setup))))
            .RequireAuthorization(PermissionList.AccountingTemplatePermissions.Apply);

        app.MapGet($"{baseRoute}/account-coding-settings", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountCodingSettingsQuery(companyId))))
            .RequireAuthorization(PermissionList.AccountingSettingsPermissions.View);

        app.MapPost($"{baseRoute}/account-coding-settings", async (AccountCodingSettingsDto settings, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertAccountCodingSettingsCommand(settings))))
            .RequireAuthorization(PermissionList.AccountingSettingsPermissions.Edit);

        app.MapPost($"{baseRoute}/account-coding-settings/preview-renumber", async (AccountCodingSettingsDto settings, ISender sender) =>
            Results.Ok(await sender.Send(new PreviewAccountRenumberCommand(settings))))
            .RequireAuthorization(PermissionList.AccountingSettingsPermissions.Edit);

        app.MapPost($"{baseRoute}/account-coding-settings/apply-renumber", async (ApplyAccountRenumberDto renumber, ISender sender) =>
            Results.Ok(await sender.Send(new ApplyAccountRenumberCommand(renumber))))
            .RequireAuthorization(PermissionList.AccountingSettingsPermissions.Edit);

        app.MapGet($"{baseRoute}/accounts", async (Guid companyId, Guid? branchId, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountsQuery(companyId, branchId, pageIndex ?? 1, pageSize ?? 20, searchText))))
            .RequireAuthorization(PermissionList.AccountPermissions.View);

        app.MapPost($"{baseRoute}/accounts", async (AccountDto account, ISender sender) =>
            Results.Created($"{baseRoute}/accounts", await sender.Send(new CreateAccountCommand(account))))
            .RequireAuthorization(PermissionList.AccountPermissions.Create);

        app.MapPut($"{baseRoute}/accounts/{{id:guid}}", async (Guid id, AccountDto account, ISender sender) =>
        {
            account.Id = id;
            return Results.Ok(await sender.Send(new UpdateAccountCommand(id, account)));
        })
            .RequireAuthorization(PermissionList.AccountPermissions.Edit);

        app.MapGet($"{baseRoute}/fiscal-periods", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetFiscalPeriodsQuery(companyId))))
            .RequireAuthorization(PermissionList.FiscalPeriodPermissions.View);

        app.MapPost($"{baseRoute}/fiscal-periods", async (FiscalPeriodDto period, ISender sender) =>
            Results.Created($"{baseRoute}/fiscal-periods", await sender.Send(new CreateFiscalPeriodCommand(period))))
            .RequireAuthorization(PermissionList.FiscalPeriodPermissions.Create);

        app.MapPost($"{baseRoute}/fiscal-periods/{{id:guid}}/close", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new CloseFiscalPeriodCommand(id))))
            .RequireAuthorization(PermissionList.FiscalPeriodPermissions.Close);

        app.MapPost($"{baseRoute}/fiscal-periods/{{id:guid}}/lock", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new LockFiscalPeriodCommand(id))))
            .RequireAuthorization(PermissionList.FiscalPeriodPermissions.Lock);

        app.MapPost($"{baseRoute}/fiscal-periods/{{id:guid}}/reopen", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new ReopenFiscalPeriodCommand(id))))
            .RequireAuthorization(PermissionList.FiscalPeriodPermissions.Reopen);

        app.MapPost($"{baseRoute}/fiscal-periods/{{id:guid}}/year-end-close", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new YearEndCloseFiscalPeriodCommand(id))))
            .RequireAuthorization(PermissionList.FiscalPeriodPermissions.YearEndClose);

        app.MapGet($"{baseRoute}/tax-codes", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetTaxCodesQuery(companyId))))
            .RequireAuthorization(PermissionList.TaxCodePermissions.View);

        app.MapPost($"{baseRoute}/tax-codes", async (TaxCodeDto taxCode, ISender sender) =>
            Results.Created($"{baseRoute}/tax-codes", await sender.Send(new CreateTaxCodeCommand(taxCode))))
            .RequireAuthorization(PermissionList.TaxCodePermissions.Create);

        app.MapGet($"{baseRoute}/posting-profiles", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetPostingProfilesQuery(companyId))))
            .RequireAuthorization(PermissionList.PostingProfilePermissions.View);

        app.MapPost($"{baseRoute}/posting-profiles", async (PostingProfileDto profile, ISender sender) =>
            Results.Created($"{baseRoute}/posting-profiles", await sender.Send(new CreatePostingProfileCommand(profile))))
            .RequireAuthorization(PermissionList.PostingProfilePermissions.Create);

        app.MapGet($"{baseRoute}/bank-accounts", async (Guid companyId, Guid? branchId, ISender sender) =>
            Results.Ok(await sender.Send(new GetBankAccountsQuery(companyId, branchId))))
            .RequireAuthorization(PermissionList.BankAccountPermissions.View);

        app.MapPost($"{baseRoute}/bank-accounts", async (BankAccountDto bankAccount, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertBankAccountCommand(bankAccount))))
            .RequireAuthorization(PermissionList.BankAccountPermissions.Create);

        app.MapPut($"{baseRoute}/bank-accounts", async (BankAccountDto bankAccount, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertBankAccountCommand(bankAccount))))
            .RequireAuthorization(PermissionList.BankAccountPermissions.Edit);

        app.MapGet($"{baseRoute}/cash-accounts", async (Guid companyId, Guid? branchId, ISender sender) =>
            Results.Ok(await sender.Send(new GetCashAccountsQuery(companyId, branchId))))
            .RequireAuthorization(PermissionList.CashAccountPermissions.View);

        app.MapPost($"{baseRoute}/cash-accounts", async (CashAccountDto cashAccount, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertCashAccountCommand(cashAccount))))
            .RequireAuthorization(PermissionList.CashAccountPermissions.Create);

        app.MapPut($"{baseRoute}/cash-accounts", async (CashAccountDto cashAccount, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertCashAccountCommand(cashAccount))))
            .RequireAuthorization(PermissionList.CashAccountPermissions.Edit);

        app.MapGet($"{baseRoute}/bank-reconciliation/transactions", async (Guid companyId, Guid? branchId, BankTransactionStatus? status, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
            Results.Ok(await sender.Send(new GetBankTransactionsQuery(companyId, branchId, status, pageIndex ?? 1, pageSize ?? 20, searchText))))
            .RequireAuthorization(PermissionList.BankReconciliationPermissions.View);

        app.MapGet($"{baseRoute}/bank-reconciliation/summary", async (Guid companyId, Guid? branchId, ISender sender) =>
            Results.Ok(await sender.Send(new GetBankReconciliationSummaryQuery(companyId, branchId))))
            .RequireAuthorization(PermissionList.BankReconciliationPermissions.View);

        app.MapGet($"{baseRoute}/bank-reconciliation/transactions/{{id:guid}}/matches", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new GetBankReconciliationMatchesQuery(id))))
            .RequireAuthorization(PermissionList.BankReconciliationPermissions.View);

        app.MapPost($"{baseRoute}/bank-reconciliation/transactions", async (BankTransactionDto transaction, ISender sender) =>
            Results.Created($"{baseRoute}/bank-reconciliation/transactions", await sender.Send(new CreateBankTransactionCommand(transaction))))
            .RequireAuthorization(PermissionList.BankReconciliationPermissions.Create);

        app.MapPost($"{baseRoute}/bank-reconciliation/reconcile", async (ReconcileBankTransactionDto reconciliation, ISender sender) =>
            Results.Ok(await sender.Send(new ReconcileBankTransactionCommand(reconciliation))))
            .RequireAuthorization(PermissionList.BankReconciliationPermissions.Reconcile);

        app.MapGet($"{baseRoute}/settings", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetCompanyAccountingSettingsQuery(companyId))))
            .RequireAuthorization(PermissionList.AccountingSettingsPermissions.View);

        app.MapPost($"{baseRoute}/settings", async (CompanyAccountingSettingsDto settings, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertCompanyAccountingSettingsCommand(settings))))
            .RequireAuthorization(PermissionList.AccountingSettingsPermissions.Edit);

        app.MapGet($"{baseRoute}/documents", async (AccountingDocumentType? type, Guid? companyId, Guid? branchId, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountingDocumentsQuery(type, companyId, branchId, pageIndex ?? 1, pageSize ?? 20, searchText))))
            .RequireAuthorization(PermissionList.AccountingDocumentPermissions.View);

        app.MapPost($"{baseRoute}/documents", async (AccountingDocumentDto document, ISender sender) =>
            Results.Created($"{baseRoute}/documents", await sender.Send(new CreateAccountingDocumentCommand(document))))
            .RequireAuthorization(PermissionList.AccountingDocumentPermissions.Create);

        app.MapPost($"{baseRoute}/documents/{{id:guid}}/post", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PostAccountingDocumentCommand(id))))
            .RequireAuthorization(PermissionList.AccountingDocumentPermissions.Post);

        app.MapPost($"{baseRoute}/documents/{{id:guid}}/zatca", async (Guid id, ZatcaInvoiceType invoiceType, ISender sender) =>
            Results.Ok(await sender.Send(new GenerateZatcaInvoiceCommand(id, invoiceType))))
            .RequireAuthorization(PermissionList.ZatcaEInvoicePermissions.Generate);

        app.MapGet($"{baseRoute}/journals", async (Guid? companyId, Guid? branchId, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
            Results.Ok(await sender.Send(new GetJournalEntriesQuery(companyId, branchId, pageIndex ?? 1, pageSize ?? 20, searchText))))
            .RequireAuthorization(PermissionList.JournalEntryPermissions.View);

        app.MapPost($"{baseRoute}/journals/quick-entry", async (QuickJournalEntryDto journalEntry, ISender sender) =>
            Results.Created($"{baseRoute}/journals", await sender.Send(new CreateQuickJournalEntryCommand(journalEntry))))
            .RequireAuthorization(PermissionList.JournalEntryPermissions.Create);

        app.MapGet($"{baseRoute}/zatca/settings", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetZatcaSettingsQuery(companyId))))
            .RequireAuthorization(PermissionList.ZatcaSettingsPermissions.View);

        app.MapPost($"{baseRoute}/zatca/settings", async (ZatcaSettingsDto settings, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertZatcaSettingsCommand(settings))))
            .RequireAuthorization(PermissionList.ZatcaSettingsPermissions.Edit);

        app.MapGet($"{baseRoute}/zatca/invoices", async (Guid? companyId, ZatcaSubmissionStatus? status, int? pageIndex, int? pageSize, ISender sender) =>
            Results.Ok(await sender.Send(new GetEInvoicesQuery(companyId, status, pageIndex ?? 1, pageSize ?? 20))))
            .RequireAuthorization(PermissionList.ZatcaEInvoicePermissions.View);

        app.MapPost($"{baseRoute}/zatca/invoices/{{id:guid}}/submit", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new SubmitEInvoiceCommand(id))))
            .RequireAuthorization(PermissionList.ZatcaEInvoicePermissions.Submit);
    }
}
