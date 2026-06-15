using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Customers.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Customers.Services;

public class CustomerPricingProfileService : BaseApiService, ICustomerPricingProfileService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public CustomerPricingProfileService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService)
    {
        this._apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/customers/customerPricingProfile";
        // /api/v1/customers/customerPricingProfile
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CustomerPricingProfileDto customerPricingProfile)
    {
        var request=new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                CustomerPricingProfile = customerPricingProfile
            })
        };
        return await SendAsync<CreateResponseDto>(request,null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request,null);
    }

    public async Task<ApiResult<PaginatedResult<CustomerPricingProfileDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string searchText = "")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<CustomerPricingProfileDto>>(request,"customerPricingProfileList");
    }

    public async Task<ApiResult<CustomerPricingProfileDto>> GetByIdAsync(Guid id)
    {
        var request=new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<CustomerPricingProfileDto>(request,"customerPricingProfile");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CustomerPricingProfileDto customerPricingProfile)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new
            {
                CustomerPricingProfile = customerPricingProfile
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
