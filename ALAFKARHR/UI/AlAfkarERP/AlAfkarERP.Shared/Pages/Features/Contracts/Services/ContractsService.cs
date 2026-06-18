using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using Microsoft.AspNetCore.Components.Forms;
using SharedWithUI.Contracts.Dtos;
using SharedWithUI.Contracts.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Contracts.Services;

public class ContractsService : BaseApiService, IContractsService
{
    private readonly ApiConfig _apiConfig;

    public ContractsService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
    }

    public async Task<ApiResult<PaginatedResult<ContractDto>>> GetContractsAsync(Guid? companyId, string? partyType, Guid? partyId, ContractStatus? status, string? type, ContractRenewalPaymentStatus? paymentStatus, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize, string? searchText)
    {
        var query = QueryString(
            ("companyId", companyId?.ToString()),
            ("partyType", partyType),
            ("partyId", partyId?.ToString()),
            ("status", status?.ToString()),
            ("type", type),
            ("paymentStatus", paymentStatus?.ToString()),
            ("fromDate", fromDate?.ToString("yyyy-MM-dd")),
            ("toDate", toDate?.ToString("yyyy-MM-dd")),
            ("pageIndex", pageIndex.ToString()),
            ("pageSize", pageSize.ToString()),
            ("searchText", searchText));
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/contracts/contracts{query}");
        return await SendAsync<PaginatedResult<ContractDto>>(request, "contracts");
    }

    public async Task<ApiResult<ContractDto>> GetContractByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/contracts/contracts/{id}");
        return await SendAsync<ContractDto>(request, "contract");
    }

    public async Task<ApiResult<CreateContractResponseDto>> CreateContractAsync(ContractDto contract)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/contracts/contracts")
        {
            Content = JsonContent.Create(new { Contract = contract })
        };
        return await SendAsync<CreateContractResponseDto>(request, null);
    }

    public async Task<ApiResult<string>> UpdateContractAsync(ContractDto contract)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/contracts/contracts/{contract.Id}")
        {
            Content = JsonContent.Create(new { Contract = contract })
        };
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> DeleteContractAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/contracts/contracts/{id}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> WorkflowAsync(Guid id, string action)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/contracts/contracts/{id}/{action}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> ConfigureRenewalAsync(Guid id, ContractRenewalSettingsDto settings)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/contracts/contracts/{id}/renewal-settings")
        {
            Content = JsonContent.Create(new { Settings = settings })
        };
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<ContractRenewalDto>> ProcessRenewalAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/contracts/contracts/{id}/renew");
        return await SendAsync<ContractRenewalDto>(request, "renewal");
    }

    public async Task<ApiResult<string>> RecordRenewalPaymentAsync(Guid contractId, Guid renewalId, Guid? paymentReferenceId, decimal paidAmount)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/contracts/contracts/{contractId}/renewals/{renewalId}/payment")
        {
            Content = JsonContent.Create(new { PaymentReferenceId = paymentReferenceId, PaidAmount = paidAmount })
        };
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<ContractTemplateDto>>> GetTemplatesAsync(Guid? companyId, string? contractType, int pageIndex, int pageSize, string? searchText)
    {
        var query = QueryString(
            ("companyId", companyId?.ToString()),
            ("contractType", contractType),
            ("pageIndex", pageIndex.ToString()),
            ("pageSize", pageSize.ToString()),
            ("searchText", searchText));
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/contracts/templates{query}");
        return await SendAsync<PaginatedResult<ContractTemplateDto>>(request, "templates");
    }

    public async Task<ApiResult<ContractTemplateDto>> GetTemplateByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/{_apiConfig.Version}/contracts/templates/{id}");
        return await SendAsync<ContractTemplateDto>(request, "template");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateTemplateAsync(ContractTemplateDto template)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/contracts/templates")
        {
            Content = JsonContent.Create(new { Template = template })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<string>> UpdateTemplateAsync(ContractTemplateDto template)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/{_apiConfig.Version}/contracts/templates/{template.Id}")
        {
            Content = JsonContent.Create(new { Template = template })
        };
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> DeleteTemplateAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/contracts/templates/{id}");
        return await SendAsync<string>(request, null);
    }

    public async Task<ApiResult<string>> UploadTemplateFileAsync(Guid id, IBrowserFile file)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/contracts/templates/{id}/file")
        {
            Content = FileContent(file)
        };
        return await SendAsync<string>(request, "filePath");
    }

    public async Task<ApiResult<string>> UploadAttachmentAsync(Guid contractId, ContractAttachmentKind kind, IBrowserFile file)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/{_apiConfig.Version}/contracts/contracts/{contractId}/attachments?kind={kind}")
        {
            Content = FileContent(file)
        };
        return await SendAsync<string>(request, "filePath");
    }

    public async Task<ApiResult<string>> DeleteAttachmentAsync(Guid contractId, Guid attachmentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{_apiConfig.Version}/contracts/contracts/{contractId}/attachments/{attachmentId}");
        return await SendAsync<string>(request, null);
    }

    private static MultipartFormDataContent FileContent(IBrowserFile file)
    {
        var content = new MultipartFormDataContent();
        var stream = new StreamContent(file.OpenReadStream(10 * 1024 * 1024));
        stream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(stream, "file", file.Name);
        return content;
    }

    private static string QueryString(params (string Key, string? Value)[] values)
    {
        var parts = values
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value!)}")
            .ToList();

        return parts.Count == 0 ? string.Empty : $"?{string.Join("&", parts)}";
    }
}
