using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.Accounting.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Accounting.Services;

public interface IAccountingService
{
    Task<ApiResult<AccountingDashboardDto>> GetDashboardAsync(Guid? companyId);
    Task<ApiResult<List<AccountingTemplateDto>>> GetTemplatesAsync(Guid? companyId = null);
    Task<ApiResult<AccountingTemplateDto>> GetTemplateAsync(Guid id, Guid? companyId = null);
    Task<ApiResult<Guid>> SaveTemplateAsync(AccountingTemplateDto template);
    Task<ApiResult<Guid>> DeleteTemplateAsync(Guid id);
    Task<ApiResult<Guid>> CaptureTemplateAsync(CaptureAccountingTemplateDto template);
    Task<ApiResult<AccountingSetupStatusDto>> GetSetupStatusAsync(Guid companyId);
    Task<ApiResult<ApplyAccountingTemplateResultDto>> ApplyTemplateAsync(ApplyAccountingTemplateDto setup);
    Task<ApiResult<PaginatedResult<AccountDto>>> GetAccountsAsync(Guid companyId, int pageIndex, int pageSize, string? searchText);
    Task<ApiResult<Guid>> CreateAccountAsync(AccountDto account);
    Task<ApiResult<Guid>> SaveAccountAsync(AccountDto account);
    Task<ApiResult<List<FiscalPeriodDto>>> GetFiscalPeriodsAsync(Guid companyId);
    Task<ApiResult<Guid>> CreateFiscalPeriodAsync(FiscalPeriodDto period);
    Task<ApiResult<List<TaxCodeDto>>> GetTaxCodesAsync(Guid companyId);
    Task<ApiResult<Guid>> CreateTaxCodeAsync(TaxCodeDto taxCode);
    Task<ApiResult<List<PostingProfileDto>>> GetPostingProfilesAsync(Guid companyId);
    Task<ApiResult<Guid>> CreatePostingProfileAsync(PostingProfileDto profile);
    Task<ApiResult<List<BankAccountDto>>> GetBankAccountsAsync(Guid companyId);
    Task<ApiResult<Guid>> SaveBankAccountAsync(BankAccountDto bankAccount);
    Task<ApiResult<List<CashAccountDto>>> GetCashAccountsAsync(Guid companyId);
    Task<ApiResult<Guid>> SaveCashAccountAsync(CashAccountDto cashAccount);
    Task<ApiResult<CompanyAccountingSettingsDto?>> GetCompanyAccountingSettingsAsync(Guid companyId);
    Task<ApiResult<Guid>> SaveCompanyAccountingSettingsAsync(CompanyAccountingSettingsDto settings);
    Task<ApiResult<PaginatedResult<AccountingDocumentDto>>> GetDocumentsAsync(AccountingDocumentType? type, Guid? companyId, int pageIndex, int pageSize, string? searchText);
    Task<ApiResult<Guid>> CreateDocumentAsync(AccountingDocumentDto document);
    Task<ApiResult<Guid>> PostDocumentAsync(Guid id);
    Task<ApiResult<Guid>> GenerateZatcaInvoiceAsync(Guid documentId, ZatcaInvoiceType invoiceType);
    Task<ApiResult<PaginatedResult<JournalEntryDto>>> GetJournalsAsync(Guid? companyId, int pageIndex, int pageSize, string? searchText);
    Task<ApiResult<PaginatedResult<EInvoiceDto>>> GetEInvoicesAsync(Guid? companyId, ZatcaSubmissionStatus? status, int pageIndex, int pageSize);
    Task<ApiResult<ZatcaSettingsDto?>> GetZatcaSettingsAsync(Guid companyId);
    Task<ApiResult<Guid>> SaveZatcaSettingsAsync(ZatcaSettingsDto settings);
    Task<ApiResult<Guid>> SubmitEInvoiceAsync(Guid id);
}
