namespace SharedWithUI.Catalog.Dtos;

public record ProductSkuDto(
    Guid Id,
    Guid VariantId,
    Guid ProductId,
    Guid PackageId,
    string Sku,
    string SkuEng,
    string VariantValue,
    decimal? Price,
    //int? AvailableQuantity,
    //public string Barcode { get; set; }
    bool ShowOnStore
);

