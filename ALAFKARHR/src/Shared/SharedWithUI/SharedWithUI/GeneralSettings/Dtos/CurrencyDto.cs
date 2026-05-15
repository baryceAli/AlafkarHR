namespace SharedWithUI.GeneralSettings.Dtos;

public class CurrencyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string NameEng { get;  set; }
    public decimal Value { get; set; }
    public string Symbol { get; set; }
    public bool IsDefault { get; set; }
    public Guid CompanyId { get; set; }

}
