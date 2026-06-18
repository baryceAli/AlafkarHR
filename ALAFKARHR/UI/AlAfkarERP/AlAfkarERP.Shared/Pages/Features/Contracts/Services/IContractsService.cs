using AlAfkarERP.Shared.Dtos;
using Microsoft.AspNetCore.Components.Forms;
using SharedWithUI.Contracts.Dtos;
using SharedWithUI.Contracts.Enums;

namespace AlAfkarERP.Shared.Pages.Features.Contracts.Services;

public interface IContractsService
{
    Task<ApiResult<PaginatedResult<ContractDto>>> GetContractsAsync(Guid? companyId, string? partyType, Guid? partyId, ContractStatus? status, string? type, ContractRenewalPaymentStatus? paymentStatus, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize, string? searchText);
    Task<ApiResult<ContractDto>> GetContractByIdAsync(Guid id);
    Task<ApiResult<CreateContractResponseDto>> CreateContractAsync(ContractDto contract);
    Task<ApiResult<string>> UpdateContractAsync(ContractDto contract);
    Task<ApiResult<string>> DeleteContractAsync(Guid id);
    Task<ApiResult<string>> WorkflowAsync(Guid id, string action);
    Task<ApiResult<string>> ConfigureRenewalAsync(Guid id, ContractRenewalSettingsDto settings);
    Task<ApiResult<ContractRenewalDto>> ProcessRenewalAsync(Guid id);
    Task<ApiResult<string>> RecordRenewalPaymentAsync(Guid contractId, Guid renewalId, Guid? paymentReferenceId, decimal paidAmount);
    Task<ApiResult<PaginatedResult<ContractTemplateDto>>> GetTemplatesAsync(Guid? companyId, string? contractType, int pageIndex, int pageSize, string? searchText);
    Task<ApiResult<ContractTemplateDto>> GetTemplateByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateTemplateAsync(ContractTemplateDto template);
    Task<ApiResult<string>> UpdateTemplateAsync(ContractTemplateDto template);
    Task<ApiResult<string>> DeleteTemplateAsync(Guid id);
    Task<ApiResult<string>> UploadTemplateFileAsync(Guid id, IBrowserFile file);
    Task<ApiResult<string>> UploadAttachmentAsync(Guid contractId, ContractAttachmentKind kind, IBrowserFile file);
    Task<ApiResult<string>> DeleteAttachmentAsync(Guid contractId, Guid attachmentId);
}
