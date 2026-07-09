namespace Inventory.Warehouses.Features.Warehouses;

internal static class WarehouseConfigurationHelper
{
    public static async Task PrepareWarehouseConfigurationAsync(
        InventoryDbContext dbContext,
        WarehouseDto warehouse,
        Guid warehouseId,
        GetCurrentUserBranchAccessResult branchAccess,
        string userId,
        CancellationToken cancellationToken)
    {
        warehouse.ShortCode = NormalizeShortCode(warehouse.ShortCode, warehouse.NameEng, warehouse.Name);

        await EnsureResupplySourcesAsync(dbContext, warehouse, warehouseId, branchAccess, cancellationToken);
        await EnsureRequiredStandardLocationsAsync(dbContext, warehouse, warehouseId, userId, cancellationToken);
        await ValidateSelectedLocationsAsync(dbContext, warehouse, warehouseId, cancellationToken);
    }

    public static WarehouseDto ToDto(Warehouse warehouse)
    {
        var dto = warehouse.Adapt<WarehouseDto>();
        dto.ResupplyFromWarehouseIds = warehouse.ResupplyFromLinks
            .Select(x => x.SourceWarehouseId)
            .Distinct()
            .ToList();
        return dto;
    }

    private static async Task EnsureRequiredStandardLocationsAsync(
        InventoryDbContext dbContext,
        WarehouseDto warehouse,
        Guid warehouseId,
        string userId,
        CancellationToken cancellationToken)
    {
        var required = new Dictionary<string, WarehouseLocationType>();

        if (warehouse.InboundFlow >= WarehouseOperationFlow.TwoStep)
        {
            required["Input"] = WarehouseLocationType.Receiving;
            required["Stock"] = WarehouseLocationType.Storage;
        }

        if (warehouse.InboundFlow == WarehouseOperationFlow.ThreeStep)
            required["Quality"] = WarehouseLocationType.Quality;

        if (warehouse.OutboundFlow >= WarehouseOperationFlow.TwoStep)
        {
            required["Stock"] = WarehouseLocationType.Storage;
            required["Output"] = WarehouseLocationType.Output;
        }

        if (warehouse.OutboundFlow == WarehouseOperationFlow.ThreeStep)
            required["Packing"] = WarehouseLocationType.Packing;

        if (warehouse.ResupplyFromWarehouseIds.Any())
            required["Transit"] = WarehouseLocationType.Transit;

        foreach (var location in required)
        {
            var locationId = await EnsureStandardLocationAsync(dbContext, warehouse, warehouseId, location.Key, location.Value, userId, cancellationToken);
            ApplyDefaultLocation(warehouse, location.Key, locationId);
        }
    }

    private static async Task<Guid> EnsureStandardLocationAsync(
        InventoryDbContext dbContext,
        WarehouseDto warehouse,
        Guid warehouseId,
        string suffix,
        WarehouseLocationType locationType,
        string userId,
        CancellationToken cancellationToken)
    {
        var code = $"{warehouse.ShortCode}/{suffix}";
        var existing = await dbContext.WarehouseLocations
            .FirstOrDefaultAsync(x => x.CompanyId == warehouse.CompanyId
                && x.WarehouseId == warehouseId
                && x.Code == code,
                cancellationToken);

        if (existing is not null)
        {
            if (!existing.IsActive)
            {
                existing.Update(new WarehouseLocationDto
                {
                    Id = existing.Id,
                    CompanyId = warehouse.CompanyId,
                    WarehouseId = warehouseId,
                    Code = code,
                    Name = existing.Name,
                    NameEng = existing.NameEng,
                    ParentCode = existing.ParentCode,
                    LocationType = existing.LocationType,
                    IsActive = true
                }, userId);
            }

            return existing.Id;
        }

        var dto = new WarehouseLocationDto
        {
            CompanyId = warehouse.CompanyId,
            WarehouseId = warehouseId,
            Code = code,
            Name = GetStandardLocationName(suffix, false),
            NameEng = GetStandardLocationName(suffix, true),
            ParentCode = warehouse.ShortCode,
            LocationType = locationType,
            IsActive = true
        };

        var entity = WarehouseLocation.Create(dto, userId);
        await dbContext.WarehouseLocations.AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    private static void ApplyDefaultLocation(WarehouseDto warehouse, string suffix, Guid locationId)
    {
        switch (suffix)
        {
            case "Stock":
                warehouse.DefaultSourceLocationId ??= locationId;
                warehouse.DefaultDestinationLocationId ??= locationId;
                break;
            case "Input":
                warehouse.DefaultDestinationLocationId ??= locationId;
                break;
            case "Quality":
                warehouse.DefaultQualityLocationId ??= locationId;
                break;
            case "Packing":
                warehouse.DefaultPackingLocationId ??= locationId;
                break;
            case "Output":
                warehouse.DefaultOutputLocationId ??= locationId;
                break;
            case "Transit":
                warehouse.DefaultTransitLocationId ??= locationId;
                break;
        }
    }

    private static async Task ValidateSelectedLocationsAsync(
        InventoryDbContext dbContext,
        WarehouseDto warehouse,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var locationIds = new[]
            {
                warehouse.DefaultSourceLocationId,
                warehouse.DefaultDestinationLocationId,
                warehouse.DefaultQualityLocationId,
                warehouse.DefaultPackingLocationId,
                warehouse.DefaultOutputLocationId,
                warehouse.DefaultTransitLocationId
            }
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        if (locationIds.Count == 0)
            return;

        var validCount = await dbContext.WarehouseLocations.AsNoTracking()
            .CountAsync(x => locationIds.Contains(x.Id)
                && x.CompanyId == warehouse.CompanyId
                && x.WarehouseId == warehouseId
                && x.IsActive
                && !x.IsDeleted,
                cancellationToken);

        if (validCount != locationIds.Count)
            throw new BadRequestException("One or more default warehouse locations are inactive or do not belong to the selected warehouse.");
    }

    private static async Task EnsureResupplySourcesAsync(
        InventoryDbContext dbContext,
        WarehouseDto warehouse,
        Guid warehouseId,
        GetCurrentUserBranchAccessResult branchAccess,
        CancellationToken cancellationToken)
    {
        warehouse.ResupplyFromWarehouseIds = warehouse.ResupplyFromWarehouseIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (warehouse.ResupplyFromWarehouseIds.Contains(warehouseId))
            throw new BadRequestException("A warehouse cannot resupply itself.");

        if (warehouse.ResupplyFromWarehouseIds.Count == 0)
            return;

        var sourceWarehouses = await dbContext.Warehouses.AsNoTracking()
            .Where(x => warehouse.ResupplyFromWarehouseIds.Contains(x.Id)
                && x.CompanyId == warehouse.CompanyId
                && !x.IsDeleted)
            .Select(x => new { x.Id, x.BranchId })
            .ToListAsync(cancellationToken);

        if (sourceWarehouses.Count != warehouse.ResupplyFromWarehouseIds.Count)
            throw new BadRequestException("One or more resupply warehouses do not belong to the selected company.");

        foreach (var sourceWarehouse in sourceWarehouses)
        {
            if (!BranchScopePolicy.CanRead(branchAccess, sourceWarehouse.BranchId) ||
                !BranchScopePolicy.CanMutate(branchAccess, sourceWarehouse.BranchId))
            {
                throw new ForbiddenException("You do not have permission to use one or more resupply warehouses.");
            }
        }
    }

    private static string NormalizeShortCode(string? shortCode, string? nameEng, string? name)
    {
        var source = string.IsNullOrWhiteSpace(shortCode)
            ? FirstLetters(nameEng) ?? FirstLetters(name) ?? "WH"
            : shortCode;

        var normalized = new string(source
            .Trim()
            .ToUpperInvariant()
            .Where(char.IsLetterOrDigit)
            .Take(12)
            .ToArray());

        return string.IsNullOrWhiteSpace(normalized) ? "WH" : normalized;
    }

    private static string? FirstLetters(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var letters = value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x[0])
            .Take(4)
            .ToArray();

        return letters.Length == 0 ? null : new string(letters);
    }

    private static string GetStandardLocationName(string suffix, bool english) =>
        suffix switch
        {
            "Stock" => english ? "Stock" : "المخزون",
            "Input" => english ? "Input" : "الاستلام",
            "Quality" => english ? "Quality" : "الجودة",
            "Packing" => english ? "Packing" : "التعبئة",
            "Output" => english ? "Output" : "الإخراج",
            "Transit" => english ? "Transit" : "النقل",
            _ => suffix
        };
}
