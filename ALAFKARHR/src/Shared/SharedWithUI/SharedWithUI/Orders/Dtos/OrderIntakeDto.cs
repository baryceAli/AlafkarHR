using SharedWithUI.Orders.Enums;
using SharedWithUI.Payments.Enums;

namespace SharedWithUI.Orders.Dtos;

public class OrderIntakeDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public OrderIntakeSource Source { get; set; }
    public string? Channel { get; set; }
    public string? ExternalReference { get; set; }
    public string? SubmittedByUserId { get; set; }
    public string? SubmittedByCustomerId { get; set; }
    public OrderIntakeStatus Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public Guid? SalesOrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public PaymentMethodType? PaymentMethod { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public decimal CheckoutTotal { get; set; }
    public DateTime? PaymentApprovedAt { get; set; }
    public string? PaymentDecisionReason { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }
    public List<OrderIntakeLineDto> Lines { get; set; } = [];
}
