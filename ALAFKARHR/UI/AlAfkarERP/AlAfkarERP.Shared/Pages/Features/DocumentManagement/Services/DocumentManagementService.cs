using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using AlAfkarERP.Shared.Utilities;
using Microsoft.AspNetCore.Components.Forms;
using SharedWithUI.DocumentManagement.Dtos;
using SharedWithUI.DocumentManagement.Enums;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.DocumentManagement.Services;

public class DocumentManagementService : BaseApiService, IDocumentManagementService
{
    private readonly ITokenService _tokenService;
    private readonly string _path;

    public DocumentManagementService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _tokenService = tokenService;
        _path = $"api/{apiConfig.Version}/documentmanagement/documents";
    }

    public async Task<ApiResult<PaginatedResult<DocumentItemDto>>> GetAsync(int pageIndex, int pageSize, string? searchText, string? sourceModule, string? sourceEntity, Guid? sourceRecordId, DocumentListScope scope = DocumentListScope.All)
    {
        var query = QueryString(
            ("pageIndex", pageIndex.ToString()),
            ("pageSize", pageSize.ToString()),
            ("searchText", searchText),
            ("sourceModule", sourceModule),
            ("sourceEntity", sourceEntity),
            ("sourceRecordId", sourceRecordId?.ToString()),
            ("scope", scope == DocumentListScope.All ? null : scope.ToString()));

        return await SendAsync<PaginatedResult<DocumentItemDto>>(new HttpRequestMessage(HttpMethod.Get, $"{_path}/{query}"), "documents");
    }

    public async Task<ApiResult<DocumentDetailDto>> GetByIdAsync(Guid id)
    {
        return await SendAsync<DocumentDetailDto>(new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}"), "document");
    }

    public async Task<ApiResult<DocumentUploadOptionsDto>> GetUploadOptionsAsync()
    {
        return await SendAsync<DocumentUploadOptionsDto>(new HttpRequestMessage(HttpMethod.Get, $"{_path}/upload-options"), "options");
    }

    public async Task<ApiResult<DocumentUploadPolicyDto>> GetUploadPolicyAsync()
    {
        return await SendAsync<DocumentUploadPolicyDto>(new HttpRequestMessage(HttpMethod.Get, $"{_path}/upload-policy"), "policy");
    }

    public async Task<ApiResult<DocumentUploadPolicyDto>> GetDefaultUploadPolicyAsync()
    {
        return await SendAsync<DocumentUploadPolicyDto>(new HttpRequestMessage(HttpMethod.Get, $"{_path}/upload-policy/defaults"), "policy");
    }

    public async Task<ApiResult<DocumentUploadPolicyDto>> UpdateUploadPolicyAsync(UpdateDocumentUploadPolicyDto policy)
    {
        return await SendAsync<DocumentUploadPolicyDto>(new HttpRequestMessage(HttpMethod.Put, $"{_path}/upload-policy")
        {
            Content = JsonContent.Create(new { Policy = policy })
        }, "policy");
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CreateDocumentDto document, IBrowserFile file)
    {
        var uploadOptions = await GetUploadOptionsAsync();
        var content = FileContent(file, uploadOptions.Data?.MaxFileSizeBytes);
        content.Add(new StringContent(document.CompanyId.ToString()), nameof(CreateDocumentDto.CompanyId));
        content.Add(new StringContent(document.Title ?? string.Empty), nameof(CreateDocumentDto.Title));
        if (!string.IsNullOrWhiteSpace(document.Description)) content.Add(new StringContent(document.Description), nameof(CreateDocumentDto.Description));
        if (!string.IsNullOrWhiteSpace(document.SourceModule)) content.Add(new StringContent(document.SourceModule), nameof(CreateDocumentDto.SourceModule));
        if (!string.IsNullOrWhiteSpace(document.SourceEntity)) content.Add(new StringContent(document.SourceEntity), nameof(CreateDocumentDto.SourceEntity));
        if (document.SourceRecordId.HasValue) content.Add(new StringContent(document.SourceRecordId.Value.ToString()), nameof(CreateDocumentDto.SourceRecordId));

        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, _path) { Content = content }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UpdateDocumentDto document)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{_path}/{document.Id}")
        {
            Content = JsonContent.Create(new { Document = document })
        }, null);
    }

    public async Task<ApiResult<CreateResponseDto>> UploadVersionAsync(Guid id, IBrowserFile file)
    {
        var uploadOptions = await GetUploadOptionsAsync();
        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{_path}/{id}/versions")
        {
            Content = FileContent(file, uploadOptions.Data?.MaxFileSizeBytes)
        }, null);
    }

    public async Task<ApiResult<CreateResponseDto>> InviteCollaboratorAsync(Guid id, InviteDocumentCollaboratorDto collaborator)
    {
        return await SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{_path}/{id}/collaborators")
        {
            Content = JsonContent.Create(new { Collaborator = collaborator })
        }, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> RemoveCollaboratorAsync(Guid id, Guid collaboratorId)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}/collaborators/{collaboratorId}"), null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}"), null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteStorageAsync(Guid id)
    {
        return await SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}/storage"), null);
    }

    public async Task<ApiResult<DocumentDownloadDto>> DownloadAsync(Guid id, Guid? versionId = null)
    {
        try
        {
            var url = $"{_path}/{id}/download";
            if (versionId.HasValue)
                url += $"?versionId={versionId}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var tokens = await _tokenService.GetTokensAsync();
            if (!string.IsNullOrWhiteSpace(tokens?.AccessToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<DocumentDownloadDto>.Failure(new ErrorResponseDto { Title = response.ReasonPhrase ?? "Download failed", Status = (int)response.StatusCode });

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                ?? "document";
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

            return ApiResult<DocumentDownloadDto>.Success(new DocumentDownloadDto(fileName, contentType, Convert.ToBase64String(bytes)));
        }
        catch (Exception ex)
        {
            return ApiResult<DocumentDownloadDto>.Failure(new ErrorResponseDto { Title = "Download failed", Detail = ex.Message });
        }
    }

    private static MultipartFormDataContent FileContent(IBrowserFile file, long? maxAllowedSize = null)
    {
        var content = new MultipartFormDataContent();
        var stream = new StreamContent(file.OpenReadStream(maxAllowedSize ?? 100 * 1024 * 1024));
        stream.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
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

public record DocumentDownloadDto(string FileName, string ContentType, string Base64Content);
