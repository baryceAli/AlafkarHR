using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Employees.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public interface ISpecializationService
{
    public Task<ApiResult<SpecializationDto>> CreateAsync(SpecializationDto specialization);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(SpecializationDto specialization);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    public Task<ApiResult<SpecializationDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<PaginatedResult<SpecializationDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize);
}
