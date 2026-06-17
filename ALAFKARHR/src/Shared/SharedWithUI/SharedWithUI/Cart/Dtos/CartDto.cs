using SharedWithUI.Orders.Enums;

namespace SharedWithUI.Cart.Dtos;

public class CartDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public OrderIntakeSource Source { get; set; }
    public string? Channel { get; set; }
    public Guid? PriceListId { get; set; }
    public string? Notes { get; set; }
    public List<CartLineDto> Lines { get; set; } = [];
    public decimal Subtotal => Lines.Sum(x => x.NetAmount);
    public decimal TaxAmount => Lines.Sum(x => x.TaxAmount);
    public decimal TotalAmount => Lines.Sum(x => x.TotalAmount);
}
