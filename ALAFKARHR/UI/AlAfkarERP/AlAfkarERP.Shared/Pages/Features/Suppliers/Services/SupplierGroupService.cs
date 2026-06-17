using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Suppliers.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Suppliers.Services;

public class SupplierGroupService : BaseApiService, ISupplierGroupService
{
    private readonly string _path;

    public SupplierGroupService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/suppliers/supplier-group";
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(SupplierGroupDto supplierGroup)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new { SupplierGroup = supplierGroup })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(SupplierGroupDto supplierGroup)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new { SupplierGroup = supplierGroup })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<List<SupplierGroupDto>>> GetByCompanyAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}");
        return await SendAsync<List<SupplierGroupDto>>(request, "supplierGroups");
    }

    public async Task<ApiResult<SupplierGroupDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<SupplierGroupDto>(request, "supplierGroup");
    }
}
