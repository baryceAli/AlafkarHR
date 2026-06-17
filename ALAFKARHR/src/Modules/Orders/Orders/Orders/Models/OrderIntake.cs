using Shared.DDD;
using SharedWithUI.Payments.Enums;

namespace Orders.Orders.Models;

public class OrderIntake : Aggregate<Guid>
{
    private readonly List<OrderIntakeLine> _lines = [];

    private OrderIntake() { }

    public string Number { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public OrderIntakeSource Source { get; private set; }
    public string? Channel { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? SubmittedByUserId { get; private set; }
    public string? SubmittedByCustomerId { get; private set; }
    public OrderIntakeStatus Status { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public Guid? SalesOrderId { get; private set; }
    public Guid? PaymentId { get; private set; }
    public PaymentMethodType? PaymentMethod { get; private set; }
    public PaymentStatus? PaymentStatus { get; private set; }
    public decimal CheckoutTotal { get; private set; }
    public DateTime? PaymentApprovedAt { get; private set; }
    public string? PaymentDecisionReason { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<OrderIntakeLine> Lines => _lines;

    public static OrderIntake Submit(OrderIntakeDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new Exception("Company is required.");
        if (dto.Lines.Count == 0)
            throw new Exception("Order must include at least one line.");

        var order = new OrderIntake
        {
            Id = Guid.NewGuid(),
            Number = string.IsNullOrWhiteSpace(dto.Number) ? $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}" : dto.Number,
            CompanyId = dto.CompanyId,
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            Source = dto.Source,
            Channel = dto.Channel,
            ExternalReference = dto.ExternalReference,
            SubmittedByUserId = dto.SubmittedByUserId ?? userId,
            SubmittedByCustomerId = dto.SubmittedByCustomerId,
            Status = OrderIntakeStatus.Submitted,
            SubmittedAt = DateTime.UtcNow,
            PaymentId = dto.PaymentId,
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = dto.PaymentStatus,
            CheckoutTotal = dto.CheckoutTotal,
            PaymentApprovedAt = dto.PaymentApprovedAt,
            PaymentDecisionReason = dto.PaymentDecisionReason,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var lineNumber = 1;
        foreach (var line in dto.Lines)
        {
            order._lines.Add(OrderIntakeLine.Create(lineNumber++, line, userId));
        }

        return order;
    }

    public void MarkAccepted(Guid salesOrderId, string userId)
    {
        if (Status is OrderIntakeStatus.Rejected or OrderIntakeStatus.Cancelled)
            throw new Exception("Rejected or cancelled orders cannot be accepted.");
        if (Status == OrderIntakeStatus.ConvertedToSales)
            throw new Exception("Order already converted to sales.");

        Status = OrderIntakeStatus.ConvertedToSales;
        AcceptedAt = DateTime.UtcNow;
        SalesOrderId = salesOrderId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Reject(string reason, string userId)
    {
        if (Status == OrderIntakeStatus.ConvertedToSales)
            throw new Exception("Converted orders cannot be rejected.");

        Status = OrderIntakeStatus.Rejected;
        RejectionReason = reason;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public OrderIntakeDto ToDto() => new()
    {
        Id = Id,
        Number = Number,
        CompanyId = CompanyId,
        CustomerId = CustomerId,
        CustomerName = CustomerName,
        Source = Source,
        Channel = Channel,
        ExternalReference = ExternalReference,
        SubmittedByUserId = SubmittedByUserId,
        SubmittedByCustomerId = SubmittedByCustomerId,
        Status = Status,
        SubmittedAt = SubmittedAt,
        AcceptedAt = AcceptedAt,
        SalesOrderId = SalesOrderId,
        PaymentId = PaymentId,
        PaymentMethod = PaymentMethod,
        PaymentStatus = PaymentStatus,
        CheckoutTotal = CheckoutTotal,
        PaymentApprovedAt = PaymentApprovedAt,
        PaymentDecisionReason = PaymentDecisionReason,
        RejectionReason = RejectionReason,
        Notes = Notes,
        Lines = Lines.Where(x => !x.IsDeleted).OrderBy(x => x.LineNumber).Select(x => x.ToDto()).ToList()
    };
}
