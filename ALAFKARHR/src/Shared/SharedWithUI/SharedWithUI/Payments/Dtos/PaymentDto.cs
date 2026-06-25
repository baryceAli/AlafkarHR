using SharedWithUI.Orders.Enums;
using SharedWithUI.Payments.Enums;

namespace SharedWithUI.Payments.Dtos;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? StoreFrontId { get; set; }
    public Guid? PosCashierSessionId { get; set; }
    public Guid? CashAccountId { get; set; }
    public Guid? BankAccountId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OrderIntakeId { get; set; }
    public OrderIntakeSource Source { get; set; }
    public PaymentSourceType SourceType { get; set; } = PaymentSourceType.Checkout;
    public Guid? SourceDocumentId { get; set; }
    public string? SourceDocumentNumber { get; set; }
    public string? Channel { get; set; }
    public PaymentMethodType Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? DecisionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
