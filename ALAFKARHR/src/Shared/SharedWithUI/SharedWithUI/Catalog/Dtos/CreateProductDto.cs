namespace SharedWithUI.Catalog.Dtos;

public record CreateProductDto(
    Guid Id,
    Guid CategoryId,
    Guid BrandId,
    Guid UnitId,
    string Name,
    string NameEng,
    decimal Price,
    string ImageUrl
    //string CreatedBy
    );

