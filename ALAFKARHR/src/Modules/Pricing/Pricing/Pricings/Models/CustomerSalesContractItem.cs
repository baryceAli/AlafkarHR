namespace Pricing.Pricings.Models;

public class CustomerSalesContractItem : Entity<Guid>
{
    private CustomerSalesContractItem()
    {
    }

    public Guid CustomerSalesContractId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? UnitId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal? MinQuantity { get; private set; }

    public static CustomerSalesContractItem Create(
        Guid id,
        Guid customerSalesContractId,
        Guid productSkuId,
        Guid? unitId,
        decimal unitPrice,
        decimal? minQuantity,
        string createdBy)
    {
        if (customerSalesContractId == Guid.Empty) throw new ArgumentException("Contract is required.", nameof(customerSalesContractId));
        if (productSkuId == Guid.Empty) throw new ArgumentException("Product SKU is required.", nameof(productSkuId));
        if (unitPrice < 0) throw new Exception("Unit price cannot be negative.");
        if (minQuantity.HasValue && minQuantity.Value < 0) throw new Exception("Minimum quantity cannot be negative.");

        return new CustomerSalesContractItem
        {
            Id = id,
            CustomerSalesContractId = customerSalesContractId,
            ProductSkuId = productSkuId,
            UnitId = unitId,
            UnitPrice = unitPrice,
            MinQuantity = minQuantity,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
