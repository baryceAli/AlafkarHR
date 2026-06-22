namespace Accounting.Accounting.Features;

public class AccountingEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var baseRoute = "/api/v1/accounting";

        app.MapGet($"{baseRoute}/dashboard", async (Guid? companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountingDashboardQuery(companyId))))
            .RequireAuthorization(PermissionList.AccountingDashboardPermissions.View);

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

        app.MapGet($"{baseRoute}/accounts", async (Guid companyId, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountsQuery(companyId, pageIndex ?? 1, pageSize ?? 20, searchText))))
            .RequireAuthorization(PermissionList.AccountPermissions.View);

        app.MapPost($"{baseRoute}/accounts", async (AccountDto account, ISender sender) =>
            Results.Created($"{baseRoute}/accounts", await sender.Send(new CreateAccountCommand(account))))
            .RequireAuthorization(PermissionList.AccountPermissions.Create);

        app.MapGet($"{baseRoute}/fiscal-periods", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetFiscalPeriodsQuery(companyId))))
            .RequireAuthorization(PermissionList.FiscalPeriodPermissions.View);

        app.MapPost($"{baseRoute}/fiscal-periods", async (FiscalPeriodDto period, ISender sender) =>
            Results.Created($"{baseRoute}/fiscal-periods", await sender.Send(new CreateFiscalPeriodCommand(period))))
            .RequireAuthorization(PermissionList.FiscalPeriodPermissions.Create);

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

        app.MapGet($"{baseRoute}/bank-accounts", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetBankAccountsQuery(companyId))))
            .RequireAuthorization(PermissionList.BankAccountPermissions.View);

        app.MapPost($"{baseRoute}/bank-accounts", async (BankAccountDto bankAccount, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertBankAccountCommand(bankAccount))))
            .RequireAuthorization(PermissionList.BankAccountPermissions.Create);

        app.MapPut($"{baseRoute}/bank-accounts", async (BankAccountDto bankAccount, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertBankAccountCommand(bankAccount))))
            .RequireAuthorization(PermissionList.BankAccountPermissions.Edit);

        app.MapGet($"{baseRoute}/cash-accounts", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetCashAccountsQuery(companyId))))
            .RequireAuthorization(PermissionList.CashAccountPermissions.View);

        app.MapPost($"{baseRoute}/cash-accounts", async (CashAccountDto cashAccount, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertCashAccountCommand(cashAccount))))
            .RequireAuthorization(PermissionList.CashAccountPermissions.Create);

        app.MapPut($"{baseRoute}/cash-accounts", async (CashAccountDto cashAccount, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertCashAccountCommand(cashAccount))))
            .RequireAuthorization(PermissionList.CashAccountPermissions.Edit);

        app.MapGet($"{baseRoute}/settings", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new GetCompanyAccountingSettingsQuery(companyId))))
            .RequireAuthorization(PermissionList.AccountingSettingsPermissions.View);

        app.MapPost($"{baseRoute}/settings", async (CompanyAccountingSettingsDto settings, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertCompanyAccountingSettingsCommand(settings))))
            .RequireAuthorization(PermissionList.AccountingSettingsPermissions.Edit);

        app.MapGet($"{baseRoute}/documents", async (AccountingDocumentType? type, Guid? companyId, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
            Results.Ok(await sender.Send(new GetAccountingDocumentsQuery(type, companyId, pageIndex ?? 1, pageSize ?? 20, searchText))))
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

        app.MapGet($"{baseRoute}/journals", async (Guid? companyId, int? pageIndex, int? pageSize, string? searchText, ISender sender) =>
            Results.Ok(await sender.Send(new GetJournalEntriesQuery(companyId, pageIndex ?? 1, pageSize ?? 20, searchText))))
            .RequireAuthorization(PermissionList.JournalEntryPermissions.View);

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
