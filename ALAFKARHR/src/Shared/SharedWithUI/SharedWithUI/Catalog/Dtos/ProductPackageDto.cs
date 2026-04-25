namespace SharedWithUI.Catalog.Dtos;

public record ProductPackageDto(
    Guid Id,
    string Name,
    string NameEng,
    double UnitsCount
    //decimal PackagePrice
);
//public double QuantityPerPackage { get; private set; }
//public decimal PackagePrice { get; set; }
//public bool ShowOnStore { get; set; }