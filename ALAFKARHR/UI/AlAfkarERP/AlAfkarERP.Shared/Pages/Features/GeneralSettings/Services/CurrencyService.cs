using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.GeneralSettings.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public class CurrencyService : BaseApiService, ICurrencyService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public CurrencyService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/Settings";
    }

    public async Task<ApiResult<PaginatedResult<CurrencyDto>>> GetAvailableAsync(int pageIndex, int pageSize, string searchText = "")
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_path}/currencies/available?pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}");

        return await SendAsync<PaginatedResult<CurrencyDto>>(request, "currencyList");
    }

    public async Task<ApiResult<PaginatedResult<CurrencyDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string searchText = "")
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}");

        return await SendAsync<PaginatedResult<CurrencyDto>>(request, "currencyList");
    }

    public async Task<ApiResult<CurrencyDto>> CreateAsync(Guid companyId, CurrencyDto currency)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/company/{companyId}/currencies")
        {
            Content = JsonContent.Create(new
            {
                Currency = currency
            })
        };

        return await SendAsync<CurrencyDto>(request, "currency");
    }

    public async Task<ApiResult<CurrencyDto>> UpdateAsync(Guid companyId, Guid currencyId, CurrencyDto currency)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/company/{companyId}/currencies/{currencyId}")
        {
            Content = JsonContent.Create(new
            {
                Currency = currency
            })
        };

        return await SendAsync<CurrencyDto>(request, "currency");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid companyId, Guid currencyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/company/{companyId}/currencies/{currencyId}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
