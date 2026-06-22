using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Accounting.Services;

public class AccountingService : BaseApiService, IAccountingService
{
    private readonly ApiConfig _apiConfig;

    public AccountingService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
    }

    public async Task<ApiResult<AccountingDashboardDto>> GetDashboardAsync(Guid? companyId)
    {
        var query = companyId.HasValue ? $"?companyId={companyId}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/dashboard{query}");
        return await SendAsync<AccountingDashboardDto>(request, "dashboard");
    }

    public async Task<ApiResult<List<AccountingTemplateDto>>> GetTemplatesAsync(Guid? companyId = null)
    {
        var query = companyId.HasValue ? $"?companyId={companyId}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/setup/templates{query}");
        return await SendAsync<List<AccountingTemplateDto>>(request, "templates");
    }

    public async Task<ApiResult<AccountingTemplateDto>> GetTemplateAsync(Guid id, Guid? companyId = null)
    {
        var query = companyId.HasValue ? $"?companyId={companyId}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/setup/templates/{id}{query}");
        return await SendAsync<AccountingTemplateDto>(request, "template");
    }

    public async Task<ApiResult<Guid>> SaveTemplateAsync(AccountingTemplateDto template)
    {
        var method = template.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var path = template.Id == Guid.Empty
            ? $"api/{_apiConfig.Version}/accounting/setup/templates"
            : $"api/{_apiConfig.Version}/accounting/setup/templates/{template.Id}";
        var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(template)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> DeleteTemplateAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/accounting/setup/templates/{id}");
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> CaptureTemplateAsync(CaptureAccountingTemplateDto template)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/setup/templates/capture-current")
        {
            Content = JsonContent.Create(template)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<AccountingSetupStatusDto>> GetSetupStatusAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/setup/status?companyId={companyId}");
        return await SendAsync<AccountingSetupStatusDto>(request, "status");
    }

    public async Task<ApiResult<ApplyAccountingTemplateResultDto>> ApplyTemplateAsync(ApplyAccountingTemplateDto setup)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/setup/apply-template")
        {
            Content = JsonContent.Create(setup)
        };
        return await SendAsync<ApplyAccountingTemplateResultDto>(request, "result");
    }

    public async Task<ApiResult<PaginatedResult<AccountDto>>> GetAccountsAsync(Guid companyId, int pageIndex, int pageSize, string? searchText)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/accounts?companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<AccountDto>>(request, "accounts");
    }

    public async Task<ApiResult<Guid>> CreateAccountAsync(AccountDto account)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/accounts")
        {
            Content = JsonContent.Create(account)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> SaveAccountAsync(AccountDto account)
    {
        var method = account.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var path = account.Id == Guid.Empty
            ? $"api/{_apiConfig.Version}/accounting/accounts"
            : $"api/{_apiConfig.Version}/accounting/accounts/{account.Id}";
        var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(account)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<List<FiscalPeriodDto>>> GetFiscalPeriodsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/fiscal-periods?companyId={companyId}");
        return await SendAsync<List<FiscalPeriodDto>>(request, "periods");
    }

    public async Task<ApiResult<Guid>> CreateFiscalPeriodAsync(FiscalPeriodDto period)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/fiscal-periods")
        {
            Content = JsonContent.Create(period)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> CloseFiscalPeriodAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/fiscal-periods/{id}/close");
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> LockFiscalPeriodAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/fiscal-periods/{id}/lock");
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> ReopenFiscalPeriodAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/fiscal-periods/{id}/reopen");
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> YearEndCloseFiscalPeriodAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/fiscal-periods/{id}/year-end-close");
        return await SendAsync<Guid>(request, "journalEntryId");
    }

    public async Task<ApiResult<List<TaxCodeDto>>> GetTaxCodesAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/tax-codes?companyId={companyId}");
        return await SendAsync<List<TaxCodeDto>>(request, "taxCodes");
    }

    public async Task<ApiResult<Guid>> CreateTaxCodeAsync(TaxCodeDto taxCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/tax-codes")
        {
            Content = JsonContent.Create(taxCode)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<List<PostingProfileDto>>> GetPostingProfilesAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/posting-profiles?companyId={companyId}");
        return await SendAsync<List<PostingProfileDto>>(request, "profiles");
    }

    public async Task<ApiResult<Guid>> CreatePostingProfileAsync(PostingProfileDto profile)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/posting-profiles")
        {
            Content = JsonContent.Create(profile)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<List<BankAccountDto>>> GetBankAccountsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/bank-accounts?companyId={companyId}");
        return await SendAsync<List<BankAccountDto>>(request, "bankAccounts");
    }

    public async Task<ApiResult<Guid>> SaveBankAccountAsync(BankAccountDto bankAccount)
    {
        var method = bankAccount.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var request = new HttpRequestMessage(method, $"api/{_apiConfig.Version}/accounting/bank-accounts")
        {
            Content = JsonContent.Create(bankAccount)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<List<CashAccountDto>>> GetCashAccountsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/cash-accounts?companyId={companyId}");
        return await SendAsync<List<CashAccountDto>>(request, "cashAccounts");
    }

    public async Task<ApiResult<Guid>> SaveCashAccountAsync(CashAccountDto cashAccount)
    {
        var method = cashAccount.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var request = new HttpRequestMessage(method, $"api/{_apiConfig.Version}/accounting/cash-accounts")
        {
            Content = JsonContent.Create(cashAccount)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<CompanyAccountingSettingsDto?>> GetCompanyAccountingSettingsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/settings?companyId={companyId}");
        return await SendAsync<CompanyAccountingSettingsDto?>(request, "settings");
    }

    public async Task<ApiResult<Guid>> SaveCompanyAccountingSettingsAsync(CompanyAccountingSettingsDto settings)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/settings")
        {
            Content = JsonContent.Create(settings)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<PaginatedResult<AccountingDocumentDto>>> GetDocumentsAsync(AccountingDocumentType? type, Guid? companyId, int pageIndex, int pageSize, string? searchText)
    {
        var query = $"companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}";
        if (type.HasValue)
            query = $"type={type.Value}&{query}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/documents?{query}");
        return await SendAsync<PaginatedResult<AccountingDocumentDto>>(request, "documents");
    }

    public async Task<ApiResult<Guid>> CreateDocumentAsync(AccountingDocumentDto document)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/documents")
        {
            Content = JsonContent.Create(document)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> PostDocumentAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/documents/{id}/post");
        return await SendAsync<Guid>(request, "entryId");
    }

    public async Task<ApiResult<Guid>> GenerateZatcaInvoiceAsync(Guid documentId, ZatcaInvoiceType invoiceType)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/documents/{documentId}/zatca?invoiceType={invoiceType}");
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<PaginatedResult<JournalEntryDto>>> GetJournalsAsync(Guid? companyId, int pageIndex, int pageSize, string? searchText)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/journals?companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<JournalEntryDto>>(request, "journalEntries");
    }

    public async Task<ApiResult<Guid>> CreateQuickJournalEntryAsync(QuickJournalEntryDto journalEntry)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/journals/quick-entry")
        {
            Content = JsonContent.Create(journalEntry)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<PaginatedResult<BankTransactionDto>>> GetBankTransactionsAsync(Guid companyId, BankTransactionStatus? status, int pageIndex, int pageSize, string? searchText)
    {
        var query = $"companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}";
        if (status.HasValue)
            query = $"status={status.Value}&{query}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/bank-reconciliation/transactions?{query}");
        return await SendAsync<PaginatedResult<BankTransactionDto>>(request, "transactions");
    }

    public async Task<ApiResult<BankReconciliationSummaryDto>> GetBankReconciliationSummaryAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/bank-reconciliation/summary?companyId={companyId}");
        return await SendAsync<BankReconciliationSummaryDto>(request, "summary");
    }

    public async Task<ApiResult<List<BankReconciliationMatchDto>>> GetBankReconciliationMatchesAsync(Guid bankTransactionId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/bank-reconciliation/transactions/{bankTransactionId}/matches");
        return await SendAsync<List<BankReconciliationMatchDto>>(request, "matches");
    }

    public async Task<ApiResult<Guid>> CreateBankTransactionAsync(BankTransactionDto transaction)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/bank-reconciliation/transactions")
        {
            Content = JsonContent.Create(transaction)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> ReconcileBankTransactionAsync(ReconcileBankTransactionDto reconciliation)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/bank-reconciliation/reconcile")
        {
            Content = JsonContent.Create(reconciliation)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<AccountingReportDto>> GetAccountingReportAsync(AccountingReportType type, Guid companyId, DateTime? fromDate, DateTime? toDate)
    {
        var query = $"type={type}&companyId={companyId}";
        if (fromDate.HasValue)
            query += $"&fromDate={fromDate.Value:O}";
        if (toDate.HasValue)
            query += $"&toDate={toDate.Value:O}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/reports?{query}");
        return await SendAsync<AccountingReportDto>(request, "report");
    }

    public async Task<ApiResult<PaginatedResult<EInvoiceDto>>> GetEInvoicesAsync(Guid? companyId, ZatcaSubmissionStatus? status, int pageIndex, int pageSize)
    {
        var query = $"companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}";
        if (status.HasValue)
            query = $"status={status.Value}&{query}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/zatca/invoices?{query}");
        return await SendAsync<PaginatedResult<EInvoiceDto>>(request, "invoices");
    }

    public async Task<ApiResult<ZatcaSettingsDto?>> GetZatcaSettingsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/accounting/zatca/settings?companyId={companyId}");
        return await SendAsync<ZatcaSettingsDto?>(request, "settings");
    }

    public async Task<ApiResult<Guid>> SaveZatcaSettingsAsync(ZatcaSettingsDto settings)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/zatca/settings")
        {
            Content = JsonContent.Create(settings)
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<Guid>> SubmitEInvoiceAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/accounting/zatca/invoices/{id}/submit");
        return await SendAsync<Guid>(request, "submissionId");
    }
}
