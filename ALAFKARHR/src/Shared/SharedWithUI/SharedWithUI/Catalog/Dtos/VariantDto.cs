namespace SharedWithUI.Catalog.Dtos;

public record VariantDto(
    Guid Id,
    string Name,
    string NameEng,
    string? Description
    );
