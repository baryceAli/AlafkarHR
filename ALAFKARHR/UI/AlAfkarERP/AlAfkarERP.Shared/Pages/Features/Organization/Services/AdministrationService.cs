using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using AlAfkarERP.Shared.Pages.Reuable;
using AlAfkarERP.Shared.Services;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class AdministrationService : BaseApiService,IAdministrationService
{
    public AdministrationService(HttpClient http) : base(http)
    {
    }

    public Task<ApiResult<CreateResponseDto>> CreateAsync(AdministrationDto company)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<PagedResult<AdministrationDto>>> GetAsync(int pageIndex, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<List<AdministrationDto>>> GetByBranchAsync(Guid branchId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<AdministrationDto>> GetByIdAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(AdministrationDto company)
    {
        throw new NotImplementedException();
    }
}
