using Shared.DDD;

namespace Orders.Orders.Models;

public class OrderIntakeLine : Entity<Guid>
{
    private OrderIntakeLine() { }

    public int LineNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductNameEng { get; private set; } = string.Empty;
    public string SkuCode { get; private set; } = string.Empty;
    public Guid UnitOfMeasureId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal RequestedUnitPrice { get; private set; }
    public decimal RequestedDiscountRate { get; private set; }
    public decimal RequestedTaxRate { get; private set; }
    public string? Notes { get; private set; }

    public static OrderIntakeLine Create(int lineNumber, OrderIntakeLineDto dto, string userId)
    {
        if (dto.ProductId == Guid.Empty)
            throw new Exception("Product is required.");
        if (dto.ProductSkuId == Guid.Empty)
            throw new Exception("Product SKU is required.");
        if (dto.UnitOfMeasureId == Guid.Empty)
            throw new Exception("Unit of measure is required.");
        if (dto.Quantity <= 0)
            throw new Exception("Quantity must be greater than zero.");

        return new OrderIntakeLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductName = dto.ProductName,
            ProductNameEng = dto.ProductNameEng,
            SkuCode = dto.SkuCode,
            UnitOfMeasureId = dto.UnitOfMeasureId,
            Quantity = dto.Quantity,
            RequestedUnitPrice = dto.RequestedUnitPrice,
            RequestedDiscountRate = dto.RequestedDiscountRate,
            RequestedTaxRate = dto.RequestedTaxRate,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public OrderIntakeLineDto ToDto() => new()
    {
        Id = Id,
        LineNumber = LineNumber,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductName = ProductName,
        ProductNameEng = ProductNameEng,
        SkuCode = SkuCode,
        UnitOfMeasureId = UnitOfMeasureId,
        Quantity = Quantity,
        RequestedUnitPrice = RequestedUnitPrice,
        RequestedDiscountRate = RequestedDiscountRate,
        RequestedTaxRate = RequestedTaxRate,
        Notes = Notes
    };
}
