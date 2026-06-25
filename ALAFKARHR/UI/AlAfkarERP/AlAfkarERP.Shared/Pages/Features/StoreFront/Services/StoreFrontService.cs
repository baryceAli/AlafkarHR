using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Accounting.Dtos;
using SharedWithUI.StoreFront.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.StoreFront.Services;

public class StoreFrontService : BaseApiService, IStoreFrontService
{
    private readonly string _path;

    public StoreFrontService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/store-front";
    }

    public async Task<ApiResult<List<StoreFrontTypeDto>>> GetTypesAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/types/company/{companyId}");
        return await SendAsync<List<StoreFrontTypeDto>>(request, "types");
    }

    public async Task<ApiResult<StoreFrontTypeDto>> SaveTypeAsync(StoreFrontTypeDto type)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/types")
        {
            Content = JsonContent.Create(new { Type = type })
        };
        return await SendAsync<StoreFrontTypeDto>(request, "type");
    }

    public async Task<ApiResult<bool>> DeleteTypeAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/types/{id}");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<List<StoreFrontDto>>> GetStoresAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/company/{companyId}");
        return await SendAsync<List<StoreFrontDto>>(request, "stores");
    }

    public async Task<ApiResult<StoreFrontDto>> GetStoreAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{id}");
        return await SendAsync<StoreFrontDto>(request, "store");
    }

    public async Task<ApiResult<StoreFrontDto>> SaveStoreAsync(StoreFrontDto store)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/stores")
        {
            Content = JsonContent.Create(new { Store = store })
        };
        return await SendAsync<StoreFrontDto>(request, "store");
    }

    public async Task<ApiResult<bool>> SetStoreStatusAsync(Guid id, bool isActive)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_path}/stores/{id}/status")
        {
            Content = JsonContent.Create(new { IsActive = isActive })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> DeleteStoreAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/stores/{id}");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<List<StoreFrontSellableItemDto>>> GetItemsAsync(Guid storeFrontId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/items");
        return await SendAsync<List<StoreFrontSellableItemDto>>(request, "items");
    }

    public async Task<ApiResult<bool>> SaveItemsAsync(Guid storeFrontId, List<StoreFrontSellableItemDto> items)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/stores/{storeFrontId}/items")
        {
            Content = JsonContent.Create(new { Items = items })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<List<StoreFrontDepartmentDto>>> GetDepartmentsAsync(Guid storeFrontId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/departments");
        return await SendAsync<List<StoreFrontDepartmentDto>>(request, "departments");
    }

    public async Task<ApiResult<StoreFrontDepartmentDto>> SaveDepartmentAsync(Guid storeFrontId, StoreFrontDepartmentDto department)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/stores/{storeFrontId}/departments")
        {
            Content = JsonContent.Create(new { Department = department })
        };
        return await SendAsync<StoreFrontDepartmentDto>(request, "department");
    }

    public async Task<ApiResult<bool>> DeleteDepartmentAsync(Guid storeFrontId, Guid departmentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/stores/{storeFrontId}/departments/{departmentId}");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<List<StoreFrontCatalogItemDto>>> GetCatalogAsync(Guid storeFrontId, Guid? customerId = null, string? searchText = null)
    {
        var query = new List<string>();
        if (customerId.HasValue)
            query.Add($"customerId={customerId.Value}");
        if (!string.IsNullOrWhiteSpace(searchText))
            query.Add($"searchText={Uri.EscapeDataString(searchText)}");

        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/catalog{suffix}");
        return await SendAsync<List<StoreFrontCatalogItemDto>>(request, "items");
    }

    public async Task<ApiResult<List<CashAccountDto>>> GetCashAccountsAsync(Guid storeFrontId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/cash-accounts");
        return await SendAsync<List<CashAccountDto>>(request, "cashAccounts");
    }

    public async Task<ApiResult<Guid>> SaveCashAccountAsync(Guid storeFrontId, CashAccountDto cashAccount)
    {
        var method = cashAccount.Id == Guid.Empty ? HttpMethod.Post : HttpMethod.Put;
        var url = cashAccount.Id == Guid.Empty
            ? $"{_path}/stores/{storeFrontId}/cash-accounts"
            : $"{_path}/stores/{storeFrontId}/cash-accounts/{cashAccount.Id}";
        var request = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(new { CashAccount = cashAccount })
        };
        return await SendAsync<Guid>(request, "id");
    }

    public async Task<ApiResult<PosCashierSessionDto?>> GetOpenSessionAsync(Guid storeFrontId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/sessions/open");
        return await SendAsync<PosCashierSessionDto?>(request, "session");
    }

    public async Task<ApiResult<List<PosCashierSessionDto>>> GetOpenSessionsAsync(Guid storeFrontId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/sessions/open-targets");
        return await SendAsync<List<PosCashierSessionDto>>(request, "sessions");
    }

    public async Task<ApiResult<PosCashierSessionDto>> OpenSessionAsync(OpenPosCashierSessionDto session)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/sessions/open")
        {
            Content = JsonContent.Create(new { Session = session })
        };
        return await SendAsync<PosCashierSessionDto>(request, "session");
    }

    public async Task<ApiResult<PosCashierSessionDto>> CloseSessionAsync(Guid sessionId, ClosePosCashierSessionDto close)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/sessions/{sessionId}/close")
        {
            Content = JsonContent.Create(new { Close = close })
        };
        return await SendAsync<PosCashierSessionDto>(request, "session");
    }

    public async Task<ApiResult<PosCashierSessionSummaryDto>> GetSessionSummaryAsync(Guid storeFrontId, DateTime? fromDate = null, DateTime? toDate = null, bool ownOnly = false)
    {
        var query = new List<string>();
        if (fromDate.HasValue)
            query.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        if (toDate.HasValue)
            query.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        if (ownOnly)
            query.Add("ownOnly=true");

        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/sessions/summary{suffix}");
        return await SendAsync<PosCashierSessionSummaryDto>(request, "summary");
    }

    public async Task<ApiResult<List<PosCashierSessionDto>>> GetSessionsAsync(Guid storeFrontId, DateTime? fromDate = null, DateTime? toDate = null, bool ownOnly = false)
    {
        var query = new List<string>();
        if (fromDate.HasValue)
            query.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        if (toDate.HasValue)
            query.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        if (ownOnly)
            query.Add("ownOnly=true");

        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/stores/{storeFrontId}/sessions{suffix}");
        return await SendAsync<List<PosCashierSessionDto>>(request, "sessions");
    }
}
