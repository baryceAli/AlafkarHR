using Shared.DDD;

namespace Cart.Carts.Models;

public class ShoppingCartLine : Entity<Guid>
{
    private ShoppingCartLine() { }

    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductNameEng { get; private set; } = string.Empty;
    public string SkuCode { get; private set; } = string.Empty;
    public Guid UnitOfMeasureId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountRate { get; private set; }
    public decimal TaxRate { get; private set; }
    public string? Notes { get; private set; }

    public static ShoppingCartLine Create(CartLineDto dto, string userId)
    {
        if (dto.ProductSkuId == Guid.Empty)
            throw new Exception("Product SKU is required.");
        if (dto.Quantity <= 0)
            throw new Exception("Quantity must be greater than zero.");

        return new ShoppingCartLine
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductName = dto.ProductName,
            ProductNameEng = dto.ProductNameEng,
            SkuCode = dto.SkuCode,
            UnitOfMeasureId = dto.UnitOfMeasureId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            DiscountRate = dto.DiscountRate,
            TaxRate = dto.TaxRate,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void UpdateQuantity(decimal quantity, string userId)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be greater than zero.");
        Quantity = quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void UpdateCheckoutPrice(decimal unitPrice, decimal discountRate, decimal taxRate, string userId)
    {
        if (unitPrice < 0)
            throw new Exception("Unit price cannot be negative.");

        UnitPrice = unitPrice;
        DiscountRate = discountRate;
        TaxRate = taxRate;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public CartLineDto ToDto() => new()
    {
        Id = Id,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductName = ProductName,
        ProductNameEng = ProductNameEng,
        SkuCode = SkuCode,
        UnitOfMeasureId = UnitOfMeasureId,
        Quantity = Quantity,
        UnitPrice = UnitPrice,
        DiscountRate = DiscountRate,
        TaxRate = TaxRate,
        Notes = Notes
    };
}
