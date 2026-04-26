namespace SharedWithUI.Inventory.Dtos;

public record WarehouseDto(
    Guid Id,
    string Name,
    string NameEng,
    string Location,
    string? Address,
    double Longitude,
    double Latitude,
    Guid CompanyId
    );
