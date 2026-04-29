using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Catalog.Dtos;
using System.ComponentModel.Design;
using System.Drawing.Printing;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Catalog.Services;

public class VariantService : BaseApiService, IVariantService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path ;

    public VariantService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        this._apiConfig = apiConfig;
        _path= $"api/{_apiConfig.Version}/catalog/variants";
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(VariantDto variant)
    {
        var request=new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                Variant=variant
            })
        };
        return await SendAsync<CreateResponseDto>(request,null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<VariantDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<VariantDto>>(request, "variantList");
    }

    public async Task<ApiResult<VariantDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<VariantDto>(request, "variant");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(VariantDto variant)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new
            {
                Variant = variant
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
