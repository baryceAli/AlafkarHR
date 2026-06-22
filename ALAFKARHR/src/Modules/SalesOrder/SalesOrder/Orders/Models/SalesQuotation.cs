using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesQuotation : Aggregate<Guid>
{
    private readonly List<SalesQuotationLine> _lines = [];

    private SalesQuotation() { }

    public string Number { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public Guid? PriceListId { get; private set; }
    public string? CouponCode { get; private set; }
    public string? SalespersonId { get; private set; }
    public SalesQuotationStatus Status { get; private set; }
    public DateTime QuotationDate { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public string? Notes { get; private set; }
    public string? Terms { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Guid? SalesOrderId { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public DateTime? ConvertedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public IReadOnlyCollection<SalesQuotationLine> Lines => _lines;

    public static SalesQuotation Create(SalesQuotationDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty) throw new Exception("Company is required.");
        if (dto.CustomerId == Guid.Empty) throw new Exception("Customer is required.");
        if (!dto.Lines.Any()) throw new Exception("Quotation must include at least one line.");

        var quotation = new SalesQuotation
        {
            Id = Guid.NewGuid(),
            Number = string.IsNullOrWhiteSpace(dto.Number) ? $"SQ-{DateTime.UtcNow:yyyyMMddHHmmss}" : dto.Number,
            CompanyId = dto.CompanyId,
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            PriceListId = dto.PriceListId,
            CouponCode = dto.CouponCode,
            SalespersonId = dto.SalespersonId ?? userId,
            Status = SalesQuotationStatus.Draft,
            QuotationDate = dto.QuotationDate == default ? DateTime.UtcNow : dto.QuotationDate,
            ValidUntil = dto.ValidUntil,
            Notes = dto.Notes,
            Terms = dto.Terms,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        quotation.ReplaceLines(dto.Lines, userId);
        return quotation;
    }

    public void Update(SalesQuotationDto dto, string userId)
    {
        EnsureEditable();
        CustomerId = dto.CustomerId;
        CustomerName = dto.CustomerName;
        PriceListId = dto.PriceListId;
        CouponCode = dto.CouponCode;
        SalespersonId = dto.SalespersonId ?? SalespersonId;
        QuotationDate = dto.QuotationDate == default ? QuotationDate : dto.QuotationDate;
        ValidUntil = dto.ValidUntil;
        Notes = dto.Notes;
        Terms = dto.Terms;
        ReplaceLines(dto.Lines, userId);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void ApplyResolvedPriceList(Guid? priceListId)
    {
        EnsureEditable();
        PriceListId = priceListId;
    }

    public void Send(string userId)
    {
        if (Status != SalesQuotationStatus.Draft)
            throw new Exception("Only draft quotations can be sent.");

        Status = SalesQuotationStatus.Sent;
        SentAt = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Accept(string userId)
    {
        if (Status is not (SalesQuotationStatus.Sent or SalesQuotationStatus.Draft))
            throw new Exception("Quotation cannot be accepted.");

        Status = SalesQuotationStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Reject(string reason, string userId)
    {
        if (Status == SalesQuotationStatus.ConvertedToSalesOrder)
            throw new Exception("Converted quotation cannot be rejected.");

        Status = SalesQuotationStatus.Rejected;
        RejectionReason = reason;
        RejectedAt = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Cancel(string userId)
    {
        if (Status == SalesQuotationStatus.ConvertedToSalesOrder)
            throw new Exception("Converted quotation cannot be cancelled.");

        Status = SalesQuotationStatus.Cancelled;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void MarkConverted(Guid salesOrderId, string userId)
    {
        if (Status is SalesQuotationStatus.Rejected or SalesQuotationStatus.Cancelled or SalesQuotationStatus.Expired)
            throw new Exception("Quotation cannot be converted.");

        Status = SalesQuotationStatus.ConvertedToSalesOrder;
        SalesOrderId = salesOrderId;
        ConvertedAt = DateTime.UtcNow;
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ExpireIfNeeded(string userId)
    {
        if (ValidUntil.HasValue && ValidUntil.Value.Date < DateTime.UtcNow.Date &&
            Status is SalesQuotationStatus.Draft or SalesQuotationStatus.Sent)
        {
            Status = SalesQuotationStatus.Expired;
            ModifiedBy = userId;
            ModifiedAt = DateTime.UtcNow;
        }
    }

    private void ReplaceLines(List<SalesQuotationLineDto> lines, string userId)
    {
        if (!lines.Any())
            throw new Exception("Quotation must include at least one line.");

        _lines.Clear();
        var lineNumber = 1;
        foreach (var line in lines)
        {
            _lines.Add(SalesQuotationLine.Create(lineNumber++, line, userId));
        }

        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        Subtotal = _lines.Sum(x => x.NetAmount);
        TaxAmount = _lines.Sum(x => x.TaxAmount);
        TotalAmount = Subtotal + TaxAmount;
    }

    private void EnsureEditable()
    {
        if (Status is not (SalesQuotationStatus.Draft or SalesQuotationStatus.Sent))
            throw new Exception("Quotation is not editable.");
    }
}
