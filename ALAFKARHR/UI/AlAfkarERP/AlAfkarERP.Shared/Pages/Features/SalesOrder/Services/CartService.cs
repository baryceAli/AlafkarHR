using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Cart.Dtos;
using SharedWithUI.Payments.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public class CartService : BaseApiService, ICartService
{
    private readonly string _path;

    public CartService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/cart/carts";
    }

    public async Task<ApiResult<CreateResponseDto>> CreateAsync(CartDto cart)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _path)
        {
            Content = JsonContent.Create(new { Cart = cart })
        };
        return await SendAsync<CreateResponseDto>(request, null);
    }

    public async Task<ApiResult<CartDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<CartDto>(request, "cart");
    }

    public async Task<ApiResult<bool>> AddLineAsync(Guid cartId, CartLineDto line)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{cartId}/lines")
        {
            Content = JsonContent.Create(new { Line = line })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> UpdateLineAsync(Guid cartId, Guid lineId, decimal quantity)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/{cartId}/lines/{lineId}")
        {
            Content = JsonContent.Create(new { Quantity = quantity })
        };
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> RemoveLineAsync(Guid cartId, Guid lineId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{cartId}/lines/{lineId}");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<bool>> ClearAsync(Guid cartId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{cartId}/lines");
        return await SendAsync<bool>(request, "isSuccess");
    }

    public async Task<ApiResult<CheckoutCartResultDto>> CheckoutAsync(Guid cartId, PaymentMethodType paymentMethod, string? paymentReference, string? paymentNotes, Guid? bankAccountId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{cartId}/checkout")
        {
            Content = JsonContent.Create(new
            {
                PaymentMethod = paymentMethod,
                PaymentReference = paymentReference,
                PaymentNotes = paymentNotes,
                BankAccountId = bankAccountId
            })
        };
        return await SendAsync<CheckoutCartResultDto>(request, null);
    }
}
