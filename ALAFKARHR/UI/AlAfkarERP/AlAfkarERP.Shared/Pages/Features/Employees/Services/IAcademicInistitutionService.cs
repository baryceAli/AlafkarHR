using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Employees.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public interface IAcademicInistitutionService
{
    public Task<ApiResult<AcademicInstitutionDto>> CreateAsync(AcademicInstitutionDto academicInstitution);
    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(AcademicInstitutionDto academicInstitution);
    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id);
    public Task<ApiResult<AcademicInstitutionDto>> GetByIdAsync(Guid id);
    public Task<ApiResult<PaginatedResult<AcademicInstitutionDto>>> GetByCompanyAsync(Guid companiId, int pageIndex, int pageSize);

}
