using SharedWithUI.Orders.Enums;
using SharedWithUI.Payments.Enums;

namespace SharedWithUI.Payments.Dtos;

public class CheckoutPaymentRequestDto
{
    public Guid CompanyId { get; set; }
    public Guid? CustomerId { get; set; }
    public OrderIntakeSource Source { get; set; }
    public string? Channel { get; set; }
    public PaymentMethodType Method { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}
