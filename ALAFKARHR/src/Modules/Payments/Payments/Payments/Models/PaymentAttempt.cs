using Shared.DDD;
using SharedWithUI.Orders.Enums;

namespace Payments.Payments.Models;

public class PaymentAttempt : Aggregate<Guid>
{
    private PaymentAttempt() { }

    public Guid CompanyId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? OrderIntakeId { get; private set; }
    public OrderIntakeSource Source { get; private set; }
    public PaymentSourceType SourceType { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public string? SourceDocumentNumber { get; private set; }
    public string? Channel { get; private set; }
    public PaymentMethodType Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public string? Reference { get; private set; }
    public string? DecisionReason { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ApprovedAt { get; private set; }

    public static PaymentAttempt Create(CheckoutPaymentRequestDto dto, PaymentStatus status, string? reason, string userId)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new Exception("Company is required.");
        if (dto.Amount <= 0)
            throw new Exception("Payment amount must be greater than zero.");

        var now = DateTime.UtcNow;
        return new PaymentAttempt
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            CustomerId = dto.CustomerId,
            Source = dto.Source,
            SourceType = dto.SourceType,
            SourceDocumentId = dto.SourceDocumentId,
            SourceDocumentNumber = dto.SourceDocumentNumber,
            Channel = dto.Channel,
            Method = dto.Method,
            Status = status,
            Amount = dto.Amount,
            Reference = dto.Reference,
            DecisionReason = reason,
            Notes = dto.Notes,
            ApprovedAt = status is PaymentStatus.Approved or PaymentStatus.Received ? now : null,
            CreatedAt = now,
            CreatedBy = userId
        };
    }

    public CheckoutPaymentDecisionDto ToDecisionDto() => new()
    {
        PaymentId = Id,
        Method = Method,
        Status = Status,
        Amount = Amount,
        DecisionReason = DecisionReason,
        ApprovedAt = ApprovedAt
    };
}
