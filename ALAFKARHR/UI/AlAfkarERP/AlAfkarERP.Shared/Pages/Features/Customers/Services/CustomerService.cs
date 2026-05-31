using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Customers.Dtos;
using System.ComponentModel.Design;
using System.Drawing.Printing;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Customers.Services;

public class CustomerService : BaseApiService, ICustomerService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;

    public CustomerService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/Customers/Customer";
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CustomerDto customer)
    {
        var request=new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new
            {
                Customer=customer
            })
        };
        return await SendAsync<CreateResponseDto>(request,null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<CustomerDto>>> GetByCompany(Guid companyId, int pageIndex, int pageSize, string searchText = "")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/Company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<CustomerDto>>(request, "customerList");
    }

    public async Task<ApiResult<CustomerDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<CustomerDto>(request, "customer");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(CustomerDto customer)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _path)
        {
            Content = JsonContent.Create(new
            {
                Customer = customer
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
