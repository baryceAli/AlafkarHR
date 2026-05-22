using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesOrderLine : Entity<Guid>
{
    private SalesOrderLine()
    {
    }

    public int LineNumber { get; private set; }
    public Guid ProductId { get; private set; }

    public Guid ProductSkuId { get; private set; }

    public string ProductName { get; private set; }
    public string ProductNameEng { get; private set; }

    public string SkuCode { get; private set; }

    public decimal Quantity { get; private set; }

    //public decimal ReservedQuantity { get; private set; }

    public decimal DeliveredQuantity { get; private set; }

    public decimal InvoicedQuantity { get; private set; }

    public decimal UnitPrice { get; private set; }
    public Guid UnitOfMeasureId { get; private set; }
    public decimal DiscountRate { get; private set; }
    public decimal DiscountAmount =>
    (Quantity * UnitPrice) * DiscountRate / 100m;

    public decimal TaxRate { get; private set; }

    public decimal TaxAmount =>
        NetAmount * TaxRate / 100m;
    public decimal NetAmount =>
        (Quantity * UnitPrice) - DiscountAmount;

    public string? Notes { get; private set; }
    public decimal TotalAmount =>
        NetAmount + TaxAmount;

    //public bool IsFullyReserved =>
    //    ReservedQuantity >= Quantity;

    public bool IsFullyDelivered =>
        DeliveredQuantity >= Quantity;

    public bool IsFullyInvoiced =>
        InvoicedQuantity >= Quantity;

    internal static SalesOrderLine Create(
        int lineNumber,
        Guid productId,
        Guid productSkuId,
        string productName,
        string productNameEng,
        string skuCode,
        decimal quantity,
        decimal unitPrice,
        Guid unitOfMeasureId,
        decimal discountRate,
        decimal taxRate,
        string? notes,
        string createdBy)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be greater than zero.");
            //throw new DomainException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new Exception("Price cannot be negative.");
        //throw new DomainException("Price cannot be negative.");
        if (discountRate < 0 || discountRate > 100)
            throw new Exception("Invalid discount rate.");

        if (taxRate < 0 || taxRate > 100)
            throw new Exception("Invalid tax rate.");
        return new SalesOrderLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            ProductId = productId,
            ProductSkuId = productSkuId,
            ProductName = productName,
            ProductNameEng = productNameEng,
            SkuCode = skuCode,
            Quantity = quantity,
            UnitPrice = unitPrice,
            UnitOfMeasureId = unitOfMeasureId,
            DiscountRate = discountRate,
            TaxRate = taxRate,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
        };
    }
    internal void Update(
        decimal quantity,
        decimal unitPrice,
        Guid unitOfMeasureId,
        decimal discountRate,
        decimal taxRate,
        string? notes,
        string modifiedBy)
    {
        ChangeQuantity(quantity);
        ChangePrice(unitPrice);
        ChangeDiscount(discountRate);
        ChangeTaxRate(taxRate);
        UnitOfMeasureId= unitOfMeasureId;
        Notes = notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy= modifiedBy;

    }
    internal void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
    //internal void Reserve(decimal quantity)
    //{
    //    if (ReservedQuantity + quantity > Quantity)
    //        throw new Exception("Cannot over reserve.");
    //        //throw new DomainException("Cannot over reserve.");

    //    ReservedQuantity += quantity;
    //}

    internal void Deliver(decimal quantity)
    {
        if (DeliveredQuantity + quantity > Quantity)
            throw new Exception("Cannot over deliver.");
        //throw new DomainException("Cannot over deliver.");
        if (quantity <= 0)
            throw new Exception("Invalid quantity.");
        DeliveredQuantity += quantity;
    }

    internal void Invoice(decimal quantity)
    {
        if (InvoicedQuantity + quantity > Quantity)
            throw new Exception("Cannot over invoice.");
        //throw new DomainException("Cannot over invoice.");
        
        if (quantity <= 0)
            throw new Exception("Invalid quantity.");

        //if (InvoicedQuantity + quantity > DeliveredQuantity)
        //    throw new Exception("Cannot invoice undelivered quantity.");


        InvoicedQuantity += quantity;
    }
    internal void ChangeQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new Exception("Invalid quantity.");
        if (quantity < DeliveredQuantity)
            throw new Exception("Invalid quantity");
        Quantity = quantity;
    }
    internal void ChangePrice(decimal price)
    {
        if (price < 0)
            throw new Exception("Invalid price.");

        UnitPrice = price;
    }
    internal void ChangeDiscount(decimal discountRate)
    {
        

        if (discountRate < 0 || discountRate > 100)
            throw new Exception("Invalid discount rate.");
        DiscountRate = discountRate;
    }

    internal void ChangeTaxRate(decimal taxRate)
    {
        if (taxRate < 0 || taxRate > 100)
            throw new Exception("Invalid tax rate.");
        TaxRate = taxRate;
    }
}