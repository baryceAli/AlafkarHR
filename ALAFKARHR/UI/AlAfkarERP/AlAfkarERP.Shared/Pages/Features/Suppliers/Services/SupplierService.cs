using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Suppliers.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Suppliers.Services;

public class SupplierService : BaseApiService, ISupplierService
{
    private readonly string _path;

    public SupplierService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService)
    {
        _path = $"api/{apiConfig.Version}/suppliers/supplier";
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(SupplierDto supplier)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new { Supplier = supplier })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(SupplierDto supplier)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new { Supplier = supplier })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<SupplierDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<SupplierDto>(request, "supplier");
    }

    public async Task<ApiResult<PaginatedResult<SupplierDto>>> GetByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string searchText = "")
    {
        var requestUri = $"{_path}/company/{companyId}?PageIndex={pageIndex}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchText))
            requestUri += $"&SearchText={Uri.EscapeDataString(searchText)}";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        return await SendAsync<PaginatedResult<SupplierDto>>(request, "supplierList");
    }
}
