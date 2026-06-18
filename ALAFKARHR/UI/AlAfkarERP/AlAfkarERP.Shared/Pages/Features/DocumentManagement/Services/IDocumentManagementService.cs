using AlAfkarERP.Shared.Dtos;
using Microsoft.AspNetCore.Components.Forms;
using SharedWithUI.DocumentManagement.Dtos;
using SharedWithUI.DocumentManagement.Enums;

namespace AlAfkarERP.Shared.Pages.Features.DocumentManagement.Services;

public interface IDocumentManagementService
{
    Task<ApiResult<PaginatedResult<DocumentItemDto>>> GetAsync(int pageIndex, int pageSize, string? searchText, string? sourceModule, string? sourceEntity, Guid? sourceRecordId, DocumentListScope scope = DocumentListScope.All);
    Task<ApiResult<DocumentDetailDto>> GetByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateAsync(CreateDocumentDto document, IBrowserFile file);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(UpdateDocumentDto document);
    Task<ApiResult<CreateResponseDto>> UploadVersionAsync(Guid id, IBrowserFile file);
    Task<ApiResult<CreateResponseDto>> InviteCollaboratorAsync(Guid id, InviteDocumentCollaboratorDto collaborator);
    Task<ApiResult<UpdateDeleteResponseDto>> RemoveCollaboratorAsync(Guid id, Guid collaboratorId);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    Task<ApiResult<DocumentDownloadDto>> DownloadAsync(Guid id, Guid? versionId = null);
}
