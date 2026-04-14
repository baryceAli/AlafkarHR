using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Pages.Features.Company.Dtos;
using AlAfkarERP.Shared.Pages.Reuable;
using AlAfkarERP.Shared.Services;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Company.Services;

public class BranchService : BaseApiService, IBranchService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public BranchService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        _path = $"{_apiConfig.BaseURL}/api/{_apiConfig.Version}/organization/branches";
    }

    public async Task<ApiResult<BranchDto>> CreateAsync(BranchDto branch)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                Branch = branch
            })
        };
        return await SendAsync<BranchDto>(request, "createdBranch");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{Id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PagedResult<BranchDto>>> GetAsync(int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}?pageIndex{pageIndex}&pageSize={pageSize}");
        return await SendAsync<PagedResult<BranchDto>>(request, "branchList");
    }

    public async Task<ApiResult<List<BranchDto>>> GetByCompanyIdAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/GetByCompanyId/{companyId}");
        return await SendAsync<List<BranchDto>>(request, "BranchList");
    }

    public async Task<ApiResult<BranchDto>> GetByIdAsync(Guid Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{Id}");
        return await SendAsync<BranchDto>(request, "branch");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(BranchDto branch)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content= JsonContent.Create(new
            {
                Branch=branch
            })
        };

        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
