using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Pricing.Dtos;
using System.ComponentModel.Design;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public class PriceListService : BaseApiService, IPriceListService
{
    private readonly string _path;
    private readonly ApiConfig _apiConfig;

    public PriceListService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService)
    {
        _apiConfig = apiConfig;
        _path = $"/api/{_apiConfig.Version}/pricing/priceLists";
        ///api/v1/pricing/priceLists/company/{companyId}
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(PriceListDto priceList)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                PriceList = priceList

            })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<PriceListDto>>> GetByCompanyId(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<PriceListDto>>(request, "priceList");
    }

    public async Task<ApiResult<PriceListDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<PriceListDto>(request, "priceList");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(PriceListDto priceList)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new
            {
                PriceList = priceList
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
