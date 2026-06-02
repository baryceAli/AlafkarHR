namespace SharedWithUI.Customers.Dtos;
public class CustomerPricingProfileDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    //public string? CustomerNameEng { get; set; }
    public Guid PriceListId { get;  set; }
    public string? PriceListName { get; set; }
    //public string? PriceListNameEng { get; set; }
    public decimal? DiscountPercentage { get; set; }

    public bool AllowAdditionalDiscounts { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
    public Guid? CompanyId { get; set; }

}
