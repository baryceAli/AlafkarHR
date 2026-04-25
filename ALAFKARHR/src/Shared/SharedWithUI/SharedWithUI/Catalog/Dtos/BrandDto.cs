namespace SharedWithUI.Catalog.Dtos;

public record BrandDto(
    Guid Id, 
    string Name,
    string NameEng,
    string? Description);