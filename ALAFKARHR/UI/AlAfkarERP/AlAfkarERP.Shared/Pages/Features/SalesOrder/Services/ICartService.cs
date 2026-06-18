using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Cart.Dtos;
using SharedWithUI.Payments.Enums;

namespace AlAfkarERP.Shared.Pages.Features.SalesOrder.Services;

public interface ICartService
{
    Task<ApiResult<CreateResponseDto>> CreateAsync(CartDto cart);
    Task<ApiResult<CartDto>> GetByIdAsync(Guid id);
    Task<ApiResult<bool>> AddLineAsync(Guid cartId, CartLineDto line);
    Task<ApiResult<bool>> UpdateLineAsync(Guid cartId, Guid lineId, decimal quantity);
    Task<ApiResult<bool>> RemoveLineAsync(Guid cartId, Guid lineId);
    Task<ApiResult<bool>> ClearAsync(Guid cartId);
    Task<ApiResult<CheckoutCartResultDto>> CheckoutAsync(Guid cartId, PaymentMethodType paymentMethod, string? paymentReference, string? paymentNotes);
}

public class CheckoutCartResultDto
{
    public Guid OrderIntakeId { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal CheckoutTotal { get; set; }
}
