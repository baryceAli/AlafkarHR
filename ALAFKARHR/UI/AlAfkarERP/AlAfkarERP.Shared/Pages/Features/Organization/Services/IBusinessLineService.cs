using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Organization.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public interface IBusinessLineService
{
    Task<ApiResult<List<BusinessLineDto>>> GetAsync(bool includeInactive = false);
    Task<ApiResult<BusinessLineDto>> CreateAsync(BusinessLineDto dto);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(BusinessLineDto dto);
    Task<ApiResult<UpdateDeleteResponseDto>> SetStatusAsync(Guid id, bool isActive);
}
