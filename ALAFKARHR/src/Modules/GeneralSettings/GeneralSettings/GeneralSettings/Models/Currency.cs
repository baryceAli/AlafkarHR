using Shared.DDD;

namespace GeneralSettings.GeneralSettings.Models;

public class Currency:Aggregate<Guid>
{
    public string Code { get; set; }
    public string Name { get; private set; }
    public string NameEng { get; private set; }
    public decimal Value { get; private set; }
    public string Symbol { get; set; }
    public bool IsDefault { get; set; }
    public Guid CompanyId { get; set; }
    public static Currency Create(Guid id,string code, string name,string nameEng, decimal value,string symbol, bool isDefault,Guid companyId, string user)
    {
        return new Currency
        {
            Id = id,
            Code= code,
            Name = name,
            NameEng = nameEng,
            Value = value,
            Symbol = symbol,
            IsDefault = isDefault,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user
        };
    }

    public void Update(string code, string name, string nameEng, decimal value, string symbol, bool isDefault, string user)
    {
        Code = code;
        Name = name;
        NameEng = nameEng;
        Value = value;
        Symbol = symbol;
        IsDefault = isDefault;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    public void SetDefault(bool isDefault, string user)
    {
        IsDefault = isDefault;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = user;
    }

    public void Remove(string user)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = user;
    }
}
