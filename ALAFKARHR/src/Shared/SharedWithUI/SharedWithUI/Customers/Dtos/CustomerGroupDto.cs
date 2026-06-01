namespace SharedWithUI.Customers.Dtos;

public class CustomerGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameEng { get; set; }
    public string? Description { get; set; }
    public decimal? DefaultDiscountPercentage { get; set; }
    public Guid? DefaultPriceListId { get; set; }
    public Guid? CompanyId { get; set; }

}
