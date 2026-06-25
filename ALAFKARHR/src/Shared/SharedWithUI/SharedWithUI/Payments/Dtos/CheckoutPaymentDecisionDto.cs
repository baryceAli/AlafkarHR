using SharedWithUI.Payments.Enums;

namespace SharedWithUI.Payments.Dtos;

public class CheckoutPaymentDecisionDto
{
    public Guid PaymentId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? StoreFrontId { get; set; }
    public Guid? PosCashierSessionId { get; set; }
    public Guid? CashAccountId { get; set; }
    public Guid? BankAccountId { get; set; }
    public PaymentMethodType Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public bool IsApproved => Status is PaymentStatus.Approved or PaymentStatus.Received;
    public string? DecisionReason { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
