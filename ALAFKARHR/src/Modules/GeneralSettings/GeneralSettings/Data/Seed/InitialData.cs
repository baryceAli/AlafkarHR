using GeneralSettings.GeneralSettings.Models;

namespace GeneralSettings.Data.Seed;

public static class InitialData
{
    public static List<Currency> currencies => new List<Currency>
    {
        Currency.Create(Guid.Parse("48913546-292a-43da-8135-89df5cac92e9"),"USD","دولار","USD",1M,"24","2243B966-E7C2-43F5-9E00-21F6315BCB22"),
        Currency.Create(Guid.Parse("71077eb6-cb32-49b1-bb41-72c1c7eeac5c"),"SAR","ريال","SAR",3.75M,"fdfc","2243B966-E7C2-43F5-9E00-21F6315BCB22"),

    };
}
