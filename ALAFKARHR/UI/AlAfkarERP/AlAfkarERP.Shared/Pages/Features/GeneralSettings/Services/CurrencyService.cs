using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.GeneralSettings.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.GeneralSettings.Services;

public class CurrencyService : BaseApiService, ICurrencyService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public CurrencyService(HttpClient http,ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/Settings";
        //"/api/v1/Settings/company/{companyId}"
    }

    public async Task<ApiResult<PaginatedResult<CurrencyDto>>> GetAsync(Guid companyId, int pageIndex, int pageSize, string searchText)
    {
        //"/api/v1/Settings/company/{companyId}"
        var request=new HttpRequestMessage(HttpMethod.Get,$"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");

        return await SendAsync<PaginatedResult<CurrencyDto>>(request, "currencyList");
    }

    
}
