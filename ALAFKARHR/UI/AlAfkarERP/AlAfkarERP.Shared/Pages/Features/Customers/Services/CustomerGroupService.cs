using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Customers.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Customers.Services;

public class CustomerGroupService : BaseApiService, ICustomerGroupService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public CustomerGroupService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService)
    {
        this._apiConfig = apiConfig;
        this._path = $"api/{_apiConfig.Version}/customers/customergroup";
        //"/api/v1/customers/customergroup/"
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CustomerGroupDto customerGroup)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                CustomerGroup = customerGroup
            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public Task<ApiResult<PaginatedResult<CustomerGroupDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string searchText = "")
    {
        var requestUri = $"{_path}/company/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}";
        
        if (!string.IsNullOrWhiteSpace(searchText))
            requestUri += $"&SearchText={Uri.EscapeDataString(searchText)}";
        
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        return SendAsync<PaginatedResult<CustomerGroupDto>>(request, "customerGroupList");
    }

    public Task<ApiResult<CustomerGroupDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return SendAsync<CustomerGroupDto>(request, "customerGroup");
    }

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CustomerGroupDto customerGroup)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new
            {
                CustomerGroup = customerGroup
            })
        };
        return SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
