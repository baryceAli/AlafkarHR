using Shared.DDD;

namespace GeneralSettings.GeneralSettings.Models;

public class Currency:Aggregate<Guid>
{
    public string Code { get; set; }
    public string Name { get; private set; }
    public string NameEng { get; private set; }
    public decimal Value { get; private set; }
    public string Symbol { get; set; }

    public static Currency Create(Guid id,string code, string name,string nameEng, decimal value,string symbol, string user)
    {
        return new Currency
        {
            Id = id,
            Code= code,
            Name = name,
            NameEng = nameEng,
            Value = value,
            Symbol = symbol,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user
        };
    }
}
