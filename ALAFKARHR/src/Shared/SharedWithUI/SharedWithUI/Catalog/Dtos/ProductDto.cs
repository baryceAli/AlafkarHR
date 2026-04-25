namespace SharedWithUI.Catalog.Dtos;

public record ProductDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryNameEng,
    Guid BrandId,
    string BrandName,
    string BrandNameEng,
    Guid UnitId,
    string UnitName,
    string UnitNameEng,
    string Name,
    string NameEng,
    decimal Price,
    string ImageUrl,
    List<ProductSkuDto> ProductSkus
);