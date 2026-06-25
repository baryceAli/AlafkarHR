namespace Inventory.Warehouses.Features.Warehouses;

public class EnsureStoreFrontWarehouseHandler(InventoryDbContext dbContext)
    : ICommandHandler<EnsureStoreFrontWarehouseCommand, EnsureStoreFrontWarehouseResult>
{
    public async Task<EnsureStoreFrontWarehouseResult> Handle(EnsureStoreFrontWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (request.BranchId == Guid.Empty)
            throw new BadRequestException("StoreFront branch is required.");

        if (request.CurrentWarehouseId.HasValue && request.CurrentWarehouseId.Value != Guid.Empty)
        {
            var current = await dbContext.Warehouses.FirstOrDefaultAsync(x => x.Id == request.CurrentWarehouseId.Value, cancellationToken)
                ?? throw new NotFoundException($"Warehouse not found: {request.CurrentWarehouseId.Value}");
            if (current.CompanyId != request.CompanyId || current.BranchId != request.BranchId || current.IsDeleted)
                throw new BadRequestException("Default warehouse must belong to the StoreFront branch.");
            return new EnsureStoreFrontWarehouseResult(current.Id);
        }

        var normalizedCode = NormalizeCode(request.Code);
        var existing = await dbContext.Warehouses
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId
                && x.BranchId == request.BranchId
                && x.NameEng == request.NameEng
                && !x.IsDeleted, cancellationToken);
        if (existing is not null)
            return new EnsureStoreFrontWarehouseResult(existing.Id);

        var warehouse = Warehouse.Create(
            Guid.NewGuid(),
            string.IsNullOrWhiteSpace(request.Name) ? request.NameEng : request.Name,
            string.IsNullOrWhiteSpace(request.NameEng) ? request.Name : request.NameEng,
            normalizedCode,
            null,
            0,
            0,
            request.CompanyId,
            request.BranchId,
            WarehouseType.Commercial,
            request.UserId);

        await dbContext.Warehouses.AddAsync(warehouse, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EnsureStoreFrontWarehouseResult(warehouse.Id);
    }

    private static string NormalizeCode(string value)
        => string.IsNullOrWhiteSpace(value) ? "STOREFRONT" : value.Trim().ToUpperInvariant();
}
