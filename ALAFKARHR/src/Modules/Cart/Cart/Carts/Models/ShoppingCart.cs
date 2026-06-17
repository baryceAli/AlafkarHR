using Shared.DDD;

namespace Cart.Carts.Models;

public class ShoppingCart : Aggregate<Guid>
{
    private readonly List<ShoppingCartLine> _lines = [];

    private ShoppingCart() { }

    public Guid CompanyId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string? UserId { get; private set; }
    public string? SessionId { get; private set; }
    public OrderIntakeSource Source { get; private set; }
    public string? Channel { get; private set; }
    public Guid? PriceListId { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<ShoppingCartLine> Lines => _lines;

    public static ShoppingCart Create(CartDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new Exception("Company is required.");

        return new ShoppingCart
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            CustomerId = dto.CustomerId,
            UserId = dto.UserId ?? userId,
            SessionId = dto.SessionId,
            Source = dto.Source,
            Channel = dto.Channel,
            PriceListId = dto.PriceListId,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void AddLine(CartLineDto dto, string userId)
    {
        var existing = Lines.FirstOrDefault(x => !x.IsDeleted && x.ProductSkuId == dto.ProductSkuId && x.UnitOfMeasureId == dto.UnitOfMeasureId);
        if (existing is not null)
        {
            existing.UpdateQuantity(existing.Quantity + dto.Quantity, userId);
            return;
        }

        _lines.Add(ShoppingCartLine.Create(dto, userId));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void UpdateLineQuantity(Guid lineId, decimal quantity, string userId)
    {
        var line = Lines.FirstOrDefault(x => !x.IsDeleted && x.Id == lineId)
            ?? throw new Exception("Cart line not found.");
        line.UpdateQuantity(quantity, userId);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void RemoveLine(Guid lineId, string userId)
    {
        var line = Lines.FirstOrDefault(x => !x.IsDeleted && x.Id == lineId)
            ?? throw new Exception("Cart line not found.");
        line.Remove(userId);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Clear(string userId)
    {
        foreach (var line in Lines.Where(x => !x.IsDeleted))
        {
            line.Remove(userId);
        }
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void UpdateLineCheckoutPrice(Guid lineId, decimal unitPrice, decimal discountRate, decimal taxRate, string userId)
    {
        var line = Lines.FirstOrDefault(x => !x.IsDeleted && x.Id == lineId)
            ?? throw new Exception("Cart line not found.");

        line.UpdateCheckoutPrice(unitPrice, discountRate, taxRate, userId);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public CartDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        CustomerId = CustomerId,
        UserId = UserId,
        SessionId = SessionId,
        Source = Source,
        Channel = Channel,
        PriceListId = PriceListId,
        Notes = Notes,
        Lines = Lines.Where(x => !x.IsDeleted).Select(x => x.ToDto()).ToList()
    };

    public OrderIntakeDto ToOrderIntakeDto(CheckoutPaymentDecisionDto payment) => new()
    {
        CompanyId = CompanyId,
        CustomerId = CustomerId,
        Source = Source,
        Channel = Channel,
        SubmittedByUserId = UserId,
        PaymentId = payment.PaymentId,
        PaymentMethod = payment.Method,
        PaymentStatus = payment.Status,
        CheckoutTotal = payment.Amount,
        PaymentApprovedAt = payment.ApprovedAt,
        PaymentDecisionReason = payment.DecisionReason,
        Notes = Notes,
        Lines = Lines.Where(x => !x.IsDeleted).Select((line, index) => new OrderIntakeLineDto
        {
            LineNumber = index + 1,
            ProductId = line.ProductId,
            ProductSkuId = line.ProductSkuId,
            ProductName = line.ProductName,
            ProductNameEng = line.ProductNameEng,
            SkuCode = line.SkuCode,
            UnitOfMeasureId = line.UnitOfMeasureId,
            Quantity = line.Quantity,
            RequestedUnitPrice = line.UnitPrice,
            RequestedDiscountRate = line.DiscountRate,
            RequestedTaxRate = line.TaxRate,
            Notes = line.Notes
        }).ToList()
    };
}
