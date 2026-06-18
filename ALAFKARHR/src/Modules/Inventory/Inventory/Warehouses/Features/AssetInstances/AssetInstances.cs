using Catalog.Contracts.Products.Features.GetProductSkuById;
using Shared.Exceptions;
using SharedWithUI.Catalog.Dtos;

namespace Inventory.Warehouses.Features.AssetInstances;

public record CreateAssetInstanceRequest(CreateAssetInstanceDto AssetInstance);
public record UpdateAssetInstanceRequest(UpdateAssetInstanceDto AssetInstance);
public record CreateAssetInstanceCommand(CreateAssetInstanceDto AssetInstance) : ICommand<CreateAssetInstanceResult>;
public record UpdateAssetInstanceCommand(UpdateAssetInstanceDto AssetInstance) : ICommand<AssetInstanceActionResult>;
public record RetireAssetInstanceCommand(Guid Id) : ICommand<AssetInstanceActionResult>;
public record GetAssetInstancesQuery(PaginationRequest PaginationRequest, AssetInstanceFilterDto Filter) : IQuery<GetAssetInstancesResult>;
public record CreateAssetInstanceResult(Guid Id, Guid MaintenanceAssetId);
public record AssetInstanceActionResult(bool IsSuccess);
public record GetAssetInstancesResult(PaginatedResult<AssetInstanceDto> AssetInstances);

public class AssetInstanceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/inventory/asset-instances", async (
            int PageIndex,
            int PageSize,
            string? searchText,
            Guid? companyId,
            Guid? branchId,
            Guid? departmentId,
            Guid? employeeId,
            Guid? warehouseId,
            Guid? productSkuId,
            Guid? maintenanceAssetId,
            AssetInstanceStatus? status,
            ISender sender) =>
        {
            var filter = new AssetInstanceFilterDto
            {
                CompanyId = companyId,
                BranchId = branchId,
                DepartmentId = departmentId,
                EmployeeId = employeeId,
                WarehouseId = warehouseId,
                ProductSkuId = productSkuId,
                MaintenanceAssetId = maintenanceAssetId,
                Status = status
            };
            var result = await sender.Send(new GetAssetInstancesQuery(new PaginationRequest(PageIndex, PageSize, searchText), filter));
            return Results.Ok(result);
        })
        .WithName("GetAssetInstances")
        .Produces<GetAssetInstancesResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.InventoryItemPermissions.View);

        app.MapPost("/api/v1/inventory/asset-instances", async (CreateAssetInstanceRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateAssetInstanceCommand(request.AssetInstance));
            return Results.Created($"/api/v1/inventory/asset-instances/{result.Id}", result);
        })
        .WithName("CreateAssetInstance")
        .Produces<CreateAssetInstanceResult>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization(PermissionList.InventoryItemPermissions.Create);

        app.MapPut("/api/v1/inventory/asset-instances", async (UpdateAssetInstanceRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateAssetInstanceCommand(request.AssetInstance));
            return Results.Ok(result);
        })
        .WithName("UpdateAssetInstance")
        .Produces<AssetInstanceActionResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(PermissionList.InventoryItemPermissions.Edit);

        app.MapDelete("/api/v1/inventory/asset-instances/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new RetireAssetInstanceCommand(id));
            return Results.Ok(result);
        })
        .WithName("RetireAssetInstance")
        .Produces<AssetInstanceActionResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(PermissionList.InventoryItemPermissions.Delete);
    }
}

public class CreateAssetInstanceHandler(InventoryDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateAssetInstanceCommand, CreateAssetInstanceResult>
{
    public async Task<CreateAssetInstanceResult> Handle(CreateAssetInstanceCommand request, CancellationToken cancellationToken)
    {
        var userId = AssetInstanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var sku = await AssetInstanceFeatureHelpers.EnsureAssetTrackableSkuAsync(sender, request.AssetInstance.ProductSkuId, request.AssetInstance.ProductId, cancellationToken);
        await AssetInstanceFeatureHelpers.EnsureWarehouseAsync(dbContext, request.AssetInstance.WarehouseId, cancellationToken);

        var assetInstanceId = Guid.NewGuid();
        var assetTag = string.IsNullOrWhiteSpace(request.AssetInstance.AssetTag)
            ? await AssetInstanceFeatureHelpers.GenerateAssetTagAsync(dbContext, cancellationToken)
            : request.AssetInstance.AssetTag.Trim();

        var maintenanceAsset = await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
            "Inventory",
            nameof(AssetInstance),
            assetInstanceId,
            null,
            sku.Name,
            sku.NameEng,
            MaintenanceAssetType.Equipment,
            AssetInstanceFeatureHelpers.ToMaintenanceStatus(request.AssetInstance.Status),
            request.AssetInstance.CompanyId,
            request.AssetInstance.BranchId,
            null,
            request.AssetInstance.Notes,
            assetTag,
            request.AssetInstance.SerialNumber,
            request.AssetInstance.PurchaseDate,
            request.AssetInstance.WarrantyEndDate), cancellationToken);

        var assetInstance = AssetInstance.Create(
            assetInstanceId,
            assetTag,
            request.AssetInstance.SerialNumber,
            request.AssetInstance.ProductId,
            request.AssetInstance.ProductSkuId,
            request.AssetInstance.CompanyId,
            request.AssetInstance.BranchId,
            request.AssetInstance.DepartmentId,
            request.AssetInstance.EmployeeId,
            request.AssetInstance.WarehouseId,
            maintenanceAsset.MaintenanceAssetId,
            request.AssetInstance.Status,
            request.AssetInstance.PurchaseDate,
            request.AssetInstance.WarrantyEndDate,
            request.AssetInstance.Notes,
            userId);

        dbContext.AssetInstances.Add(assetInstance);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateAssetInstanceResult(assetInstance.Id, assetInstance.MaintenanceAssetId);
    }
}

public class UpdateAssetInstanceHandler(InventoryDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateAssetInstanceCommand, AssetInstanceActionResult>
{
    public async Task<AssetInstanceActionResult> Handle(UpdateAssetInstanceCommand request, CancellationToken cancellationToken)
    {
        var userId = AssetInstanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var assetInstance = await dbContext.AssetInstances.FirstOrDefaultAsync(x => x.Id == request.AssetInstance.Id, cancellationToken)
            ?? throw new NotFoundException("Asset instance", request.AssetInstance.Id);

        var sku = await AssetInstanceFeatureHelpers.EnsureAssetTrackableSkuAsync(sender, request.AssetInstance.ProductSkuId, request.AssetInstance.ProductId, cancellationToken);
        await AssetInstanceFeatureHelpers.EnsureWarehouseAsync(dbContext, request.AssetInstance.WarehouseId, cancellationToken);

        var assetTag = string.IsNullOrWhiteSpace(request.AssetInstance.AssetTag)
            ? assetInstance.AssetTag
            : request.AssetInstance.AssetTag.Trim();

        var maintenanceAsset = await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
            "Inventory",
            nameof(AssetInstance),
            assetInstance.Id,
            null,
            sku.Name,
            sku.NameEng,
            MaintenanceAssetType.Equipment,
            AssetInstanceFeatureHelpers.ToMaintenanceStatus(request.AssetInstance.Status),
            request.AssetInstance.CompanyId,
            request.AssetInstance.BranchId,
            null,
            request.AssetInstance.Notes,
            assetTag,
            request.AssetInstance.SerialNumber,
            request.AssetInstance.PurchaseDate,
            request.AssetInstance.WarrantyEndDate), cancellationToken);

        assetInstance.Update(
            assetTag,
            request.AssetInstance.SerialNumber,
            request.AssetInstance.ProductId,
            request.AssetInstance.ProductSkuId,
            request.AssetInstance.CompanyId,
            request.AssetInstance.BranchId,
            request.AssetInstance.DepartmentId,
            request.AssetInstance.EmployeeId,
            request.AssetInstance.WarehouseId,
            maintenanceAsset.MaintenanceAssetId,
            request.AssetInstance.Status,
            request.AssetInstance.PurchaseDate,
            request.AssetInstance.WarrantyEndDate,
            request.AssetInstance.Notes,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AssetInstanceActionResult(true);
    }
}

public class RetireAssetInstanceHandler(InventoryDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RetireAssetInstanceCommand, AssetInstanceActionResult>
{
    public async Task<AssetInstanceActionResult> Handle(RetireAssetInstanceCommand request, CancellationToken cancellationToken)
    {
        var userId = AssetInstanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var assetInstance = await dbContext.AssetInstances.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Asset instance", request.Id);

        var sku = await AssetInstanceFeatureHelpers.GetProductSkuAsync(sender, assetInstance.ProductSkuId, assetInstance.ProductId, cancellationToken);
        var maintenanceAsset = await sender.Send(new UpsertLinkedMaintenanceAssetCommand(
            "Inventory",
            nameof(AssetInstance),
            assetInstance.Id,
            null,
            sku.Name,
            sku.NameEng,
            MaintenanceAssetType.Equipment,
            MaintenanceAssetStatus.Retired,
            assetInstance.CompanyId,
            assetInstance.BranchId,
            null,
            assetInstance.Notes,
            assetInstance.AssetTag,
            assetInstance.SerialNumber,
            assetInstance.PurchaseDate,
            assetInstance.WarrantyEndDate), cancellationToken);

        assetInstance.Update(
            assetInstance.AssetTag,
            assetInstance.SerialNumber,
            assetInstance.ProductId,
            assetInstance.ProductSkuId,
            assetInstance.CompanyId,
            assetInstance.BranchId,
            assetInstance.DepartmentId,
            assetInstance.EmployeeId,
            assetInstance.WarehouseId,
            maintenanceAsset.MaintenanceAssetId,
            AssetInstanceStatus.Retired,
            assetInstance.PurchaseDate,
            assetInstance.WarrantyEndDate,
            assetInstance.Notes,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AssetInstanceActionResult(true);
    }
}

public class GetAssetInstancesHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetAssetInstancesQuery, GetAssetInstancesResult>
{
    public async Task<GetAssetInstancesResult> Handle(GetAssetInstancesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.AssetInstances.AsNoTracking().AsQueryable();

        if (request.Filter.CompanyId.HasValue)
            query = query.Where(x => x.CompanyId == request.Filter.CompanyId.Value);
        if (request.Filter.BranchId.HasValue)
            query = query.Where(x => x.BranchId == request.Filter.BranchId.Value);
        if (request.Filter.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == request.Filter.DepartmentId.Value);
        if (request.Filter.EmployeeId.HasValue)
            query = query.Where(x => x.EmployeeId == request.Filter.EmployeeId.Value);
        if (request.Filter.WarehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == request.Filter.WarehouseId.Value);
        if (request.Filter.ProductSkuId.HasValue)
            query = query.Where(x => x.ProductSkuId == request.Filter.ProductSkuId.Value);
        if (request.Filter.MaintenanceAssetId.HasValue)
            query = query.Where(x => x.MaintenanceAssetId == request.Filter.MaintenanceAssetId.Value);
        if (request.Filter.Status.HasValue)
            query = query.Where(x => x.Status == request.Filter.Status.Value);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x =>
                x.AssetTag.ToLower().Contains(search) ||
                (x.SerialNumber != null && x.SerialNumber.ToLower().Contains(search)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var assetInstances = await query
            .OrderBy(x => x.AssetTag)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .Select(x => new AssetInstanceDto
            {
                Id = x.Id,
                AssetTag = x.AssetTag,
                SerialNumber = x.SerialNumber,
                ProductId = x.ProductId,
                ProductSkuId = x.ProductSkuId,
                CompanyId = x.CompanyId,
                BranchId = x.BranchId,
                DepartmentId = x.DepartmentId,
                EmployeeId = x.EmployeeId,
                WarehouseId = x.WarehouseId,
                MaintenanceAssetId = x.MaintenanceAssetId,
                Status = x.Status,
                PurchaseDate = x.PurchaseDate,
                WarrantyEndDate = x.WarrantyEndDate,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt ?? DateTime.UtcNow
            })
            .ToListAsync(cancellationToken);

        return new GetAssetInstancesResult(new PaginatedResult<AssetInstanceDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            assetInstances));
    }
}

internal static class AssetInstanceFeatureHelpers
{
    public static async Task<ProductSkuDto> EnsureAssetTrackableSkuAsync(ISender sender, Guid productSkuId, Guid productId, CancellationToken cancellationToken)
    {
        var sku = await GetProductSkuAsync(sender, productSkuId, productId, cancellationToken);
        if (!sku.IsAssetTrackable)
            throw new BadRequestException("Product SKU is not configured for asset tracking.");
        return sku;
    }

    public static async Task<ProductSkuDto> GetProductSkuAsync(ISender sender, Guid productSkuId, Guid productId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductSkuByIdQuery(productSkuId), cancellationToken);
        var sku = result.ProductSku;
        if (sku.ProductId != productId)
            throw new BadRequestException("Product SKU does not belong to the selected product.");
        return sku;
    }

    public static async Task EnsureWarehouseAsync(InventoryDbContext dbContext, Guid? warehouseId, CancellationToken cancellationToken)
    {
        if (!warehouseId.HasValue)
            return;

        var exists = await dbContext.Warehouses.AnyAsync(x => x.Id == warehouseId.Value, cancellationToken);
        if (!exists)
            throw new NotFoundException("Warehouse", warehouseId.Value);
    }

    public static string GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static async Task<string> GenerateAssetTagAsync(InventoryDbContext dbContext, CancellationToken cancellationToken)
    {
        var count = await dbContext.AssetInstances.LongCountAsync(cancellationToken) + 1;
        return $"IA-{DateTime.UtcNow:yyyyMMdd}-{count:0000}";
    }

    public static MaintenanceAssetStatus ToMaintenanceStatus(AssetInstanceStatus status) =>
        status switch
        {
            AssetInstanceStatus.InMaintenance => MaintenanceAssetStatus.UnderMaintenance,
            AssetInstanceStatus.Retired => MaintenanceAssetStatus.Retired,
            AssetInstanceStatus.Lost => MaintenanceAssetStatus.Inactive,
            _ => MaintenanceAssetStatus.Active
        };
}
