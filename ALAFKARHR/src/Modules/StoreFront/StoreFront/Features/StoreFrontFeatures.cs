using Catalog.Contracts.Products.Features.GetProductByCompany;

namespace StoreFront.Features;

public record GetStoreFrontTypesQuery(Guid CompanyId) : IQuery<GetStoreFrontTypesResult>;
public record GetStoreFrontTypesResult(List<StoreFrontTypeDto> Types);
public record SaveStoreFrontTypeCommand(StoreFrontTypeDto Type) : ICommand<SaveStoreFrontTypeResult>;
public record SaveStoreFrontTypeResult(StoreFrontTypeDto Type);
public record DeleteStoreFrontTypeCommand(Guid Id) : ICommand<DeleteStoreFrontTypeResult>;
public record DeleteStoreFrontTypeResult(bool IsSuccess);
public record GetStoreFrontsByCompanyQuery(Guid CompanyId) : IQuery<GetStoreFrontsByCompanyResult>;
public record GetStoreFrontsByCompanyResult(List<StoreFrontDto> Stores);
public record GetStoreFrontByIdQuery(Guid Id) : IQuery<GetStoreFrontByIdResult>;
public record GetStoreFrontByIdResult(StoreFrontDto Store);
public record SaveStoreFrontCommand(StoreFrontDto Store) : ICommand<SaveStoreFrontResult>;
public record SaveStoreFrontResult(StoreFrontDto Store);
public record SetStoreFrontStatusCommand(Guid Id, bool IsActive) : ICommand<SetStoreFrontStatusResult>;
public record SetStoreFrontStatusResult(bool IsSuccess);
public record DeleteStoreFrontCommand(Guid Id) : ICommand<DeleteStoreFrontResult>;
public record DeleteStoreFrontResult(bool IsSuccess);
public record GetStoreFrontItemsQuery(Guid StoreFrontId) : IQuery<GetStoreFrontItemsResult>;
public record GetStoreFrontItemsResult(List<StoreFrontSellableItemDto> Items);
public record SaveStoreFrontItemsCommand(Guid StoreFrontId, List<StoreFrontSellableItemDto> Items) : ICommand<SaveStoreFrontItemsResult>;
public record SaveStoreFrontItemsResult(bool IsSuccess);
public record GetStoreFrontCatalogQuery(Guid StoreFrontId, Guid? CustomerId, string? SearchText) : IQuery<GetStoreFrontCatalogResult>;
public record GetStoreFrontCatalogResult(List<StoreFrontCatalogItemDto> Items);

public record SaveStoreFrontTypeRequest(StoreFrontTypeDto Type);
public record SaveStoreFrontTypeResponse(StoreFrontTypeDto Type);
public record SaveStoreFrontRequest(StoreFrontDto Store);
public record SaveStoreFrontResponse(StoreFrontDto Store);
public record SetStoreFrontStatusRequest(bool IsActive);
public record SaveStoreFrontItemsRequest(List<StoreFrontSellableItemDto> Items);

public class StoreFrontTypeValidator : AbstractValidator<SaveStoreFrontTypeCommand>
{
    public StoreFrontTypeValidator()
    {
        RuleFor(x => x.Type.CompanyId).NotEmpty();
        RuleFor(x => x.Type.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type.NameEng).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type.Code).NotEmpty().MaximumLength(100);
    }
}

public class StoreFrontValidator : AbstractValidator<SaveStoreFrontCommand>
{
    public StoreFrontValidator()
    {
        RuleFor(x => x.Store.CompanyId).NotEmpty();
        RuleFor(x => x.Store.StoreFrontTypeId).NotEmpty();
        RuleFor(x => x.Store.DefaultWarehouseId).NotEmpty();
        RuleFor(x => x.Store.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Store.NameEng).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Store.Code).NotEmpty().MaximumLength(100);
    }
}

public class StoreFrontQueryHandler(StoreFrontDbContext dbContext, ISender sender)
    : IQueryHandler<GetStoreFrontTypesQuery, GetStoreFrontTypesResult>,
      IQueryHandler<GetStoreFrontsByCompanyQuery, GetStoreFrontsByCompanyResult>,
      IQueryHandler<GetStoreFrontByIdQuery, GetStoreFrontByIdResult>,
      IQueryHandler<GetStoreFrontItemsQuery, GetStoreFrontItemsResult>,
      IQueryHandler<GetStoreFrontCatalogQuery, GetStoreFrontCatalogResult>
{
    public async Task<GetStoreFrontTypesResult> Handle(GetStoreFrontTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await dbContext.StoreFrontTypes
            .AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderBy(x => x.Name)
            .Select(x => new StoreFrontTypeDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                Name = x.Name,
                NameEng = x.NameEng,
                Code = x.Code,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new GetStoreFrontTypesResult(types);
    }

    public async Task<GetStoreFrontsByCompanyResult> Handle(GetStoreFrontsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        var stores = await dbContext.StoreFronts
            .AsNoTracking()
            .Include(x => x.StoreFrontType)
            .Where(x => x.CompanyId == request.CompanyId)
            .Where(x => branchAccess.CanViewAllBranches || x.BranchId == null || (x.BranchId.HasValue && branchAccess.BranchIds.Contains(x.BranchId.Value)))
            .OrderBy(x => x.Name)
            .Select(x => ToStoreDto(x, x.SellableItems.Count(item => item.IsActive && !item.IsDeleted)))
            .ToListAsync(cancellationToken);

        return new GetStoreFrontsByCompanyResult(stores);
    }

    public async Task<GetStoreFrontByIdResult> Handle(GetStoreFrontByIdQuery request, CancellationToken cancellationToken)
    {
        var store = await dbContext.StoreFronts
            .AsNoTracking()
            .Include(x => x.StoreFrontType)
            .Include(x => x.SellableItems)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Store front not found: {request.Id}");

        await EnsureCanReadStoreAsync(store.CompanyId, store.BranchId, cancellationToken);
        return new GetStoreFrontByIdResult(ToStoreDto(store, store.SellableItems.Count(x => x.IsActive && !x.IsDeleted)));
    }

    public async Task<GetStoreFrontItemsResult> Handle(GetStoreFrontItemsQuery request, CancellationToken cancellationToken)
    {
        var store = await dbContext.StoreFronts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StoreFrontId, cancellationToken)
            ?? throw new NotFoundException($"Store front not found: {request.StoreFrontId}");
        await EnsureCanReadStoreAsync(store.CompanyId, store.BranchId, cancellationToken);

        var items = await dbContext.StoreFrontSellableItems
            .AsNoTracking()
            .Where(x => x.StoreFrontId == request.StoreFrontId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.ProductNameEng)
            .Select(x => ToItemDto(x))
            .ToListAsync(cancellationToken);

        return new GetStoreFrontItemsResult(items);
    }

    public async Task<GetStoreFrontCatalogResult> Handle(GetStoreFrontCatalogQuery request, CancellationToken cancellationToken)
    {
        var store = await dbContext.StoreFronts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StoreFrontId && x.IsActive, cancellationToken)
            ?? throw new NotFoundException($"Active store front not found: {request.StoreFrontId}");

        await EnsureCanReadStoreAsync(store.CompanyId, store.BranchId, cancellationToken);

        var assignedItems = await dbContext.StoreFrontSellableItems
            .AsNoTracking()
            .Where(x => x.StoreFrontId == request.StoreFrontId && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        if (assignedItems.Count == 0)
            return new GetStoreFrontCatalogResult([]);

        var products = await sender.Send(
            new GetPricedProductByCompanyQuery(store.CompanyId, request.CustomerId ?? store.DefaultCustomerId, store.PriceListId, new PaginationRequest(0, 1000, request.SearchText)),
            cancellationToken);

        var settingsBySku = assignedItems.ToDictionary(x => x.ProductSkuId);
        var catalogItems = products.ProductList.Data
            .SelectMany(product => product.Skus)
            .Where(sku => settingsBySku.ContainsKey(sku.Id) && sku.IsSellable)
            .Select(sku => new StoreFrontCatalogItemDto
            {
                Sku = sku,
                Settings = ToItemDto(settingsBySku[sku.Id])
            })
            .ToList();

        return new GetStoreFrontCatalogResult(catalogItems);
    }

    private static StoreFrontDto ToStoreDto(StoreFrontStore store, int activeItemsCount) => new()
    {
        Id = store.Id,
        CompanyId = store.CompanyId,
        BranchId = store.BranchId,
        StoreFrontTypeId = store.StoreFrontTypeId,
        StoreFrontTypeName = store.StoreFrontType?.Name,
        StoreFrontTypeNameEng = store.StoreFrontType?.NameEng,
        DefaultWarehouseId = store.DefaultWarehouseId,
        DefaultCustomerId = store.DefaultCustomerId,
        PriceListId = store.PriceListId,
        Name = store.Name,
        NameEng = store.NameEng,
        Code = store.Code,
        ReceiptHeader = store.ReceiptHeader,
        ReceiptFooter = store.ReceiptFooter,
        IsActive = store.IsActive,
        ActiveItemsCount = activeItemsCount
    };

    private static StoreFrontSellableItemDto ToItemDto(StoreFrontSellableItem item) => new()
    {
        Id = item.Id,
        StoreFrontId = item.StoreFrontId,
        ProductSkuId = item.ProductSkuId,
        ProductName = item.ProductName,
        ProductNameEng = item.ProductNameEng,
        SkuCode = item.SkuCode,
        IsActive = item.IsActive,
        DisplayOrder = item.DisplayOrder,
        AllowManualPrice = item.AllowManualPrice,
        RequireManualPriceNote = item.RequireManualPriceNote,
        MinimumManualPrice = item.MinimumManualPrice,
        MaximumManualPrice = item.MaximumManualPrice
    };

    private async Task EnsureCanReadStoreAsync(Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanRead(branchAccess, branchId))
            throw new ForbiddenException("You do not have permission to view this storefront branch scope.");
    }
}

public class StoreFrontCommandHandler(
    StoreFrontDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IBusinessLineEntitlementService entitlementService,
    ICompanyHierarchyReader companyHierarchyReader,
    ISender sender)
    : ICommandHandler<SaveStoreFrontTypeCommand, SaveStoreFrontTypeResult>,
      ICommandHandler<DeleteStoreFrontTypeCommand, DeleteStoreFrontTypeResult>,
      ICommandHandler<SaveStoreFrontCommand, SaveStoreFrontResult>,
      ICommandHandler<SetStoreFrontStatusCommand, SetStoreFrontStatusResult>,
      ICommandHandler<DeleteStoreFrontCommand, DeleteStoreFrontResult>,
      ICommandHandler<SaveStoreFrontItemsCommand, SaveStoreFrontItemsResult>
{
    public async Task<SaveStoreFrontTypeResult> Handle(SaveStoreFrontTypeCommand request, CancellationToken cancellationToken)
    {
        var type = request.Type.Id == Guid.Empty
            ? null
            : await dbContext.StoreFrontTypes.FirstOrDefaultAsync(x => x.Id == request.Type.Id, cancellationToken);

        if (type is null)
        {
            type = StoreFrontType.Create(request.Type.CompanyId, request.Type.Name, request.Type.NameEng, request.Type.Code, GetUserId());
            await dbContext.StoreFrontTypes.AddAsync(type, cancellationToken);
        }
        else
        {
            type.Update(request.Type.Name, request.Type.NameEng, request.Type.Code, request.Type.IsActive, GetUserId());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        request.Type.Id = type.Id;
        request.Type.Code = type.Code;
        return new SaveStoreFrontTypeResult(request.Type);
    }

    public async Task<DeleteStoreFrontTypeResult> Handle(DeleteStoreFrontTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await dbContext.StoreFrontTypes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Store front type not found: {request.Id}");

        var isUsed = await dbContext.StoreFronts.AnyAsync(x => x.StoreFrontTypeId == request.Id, cancellationToken);
        if (isUsed)
            throw new BadRequestException("Store front type is used by one or more stores.");

        type.Remove(GetUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteStoreFrontTypeResult(true);
    }

    public async Task<SaveStoreFrontResult> Handle(SaveStoreFrontCommand request, CancellationToken cancellationToken)
    {
        var existing = request.Store.Id == Guid.Empty
            ? null
            : await dbContext.StoreFronts.FirstOrDefaultAsync(x => x.Id == request.Store.Id, cancellationToken);

        if (request.Store.IsActive && (existing is null || !existing.IsActive))
            await EnsureStoreActivationAvailableAsync(request.Store.CompanyId, existing?.Id, cancellationToken);

        if (existing is not null && request.Store.CompanyId != existing.CompanyId)
            throw new BadRequestException("Store front company cannot be changed.");

        if (existing is not null)
            await EnsureCanMutateStoreAsync(existing.CompanyId, existing.BranchId, cancellationToken);

        await EnsureCanMutateStoreAsync(request.Store.CompanyId, request.Store.BranchId, cancellationToken);
        await EnsureTypeAsync(request.Store.CompanyId, request.Store.StoreFrontTypeId, cancellationToken);

        StoreFrontStore store;
        if (existing is null)
        {
            store = StoreFrontStore.Create(request.Store, GetUserId());
            await dbContext.StoreFronts.AddAsync(store, cancellationToken);
        }
        else
        {
            existing.Update(request.Store, GetUserId());
            store = existing;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        request.Store.Id = store.Id;
        return new SaveStoreFrontResult(request.Store);
    }

    public async Task<SetStoreFrontStatusResult> Handle(SetStoreFrontStatusCommand request, CancellationToken cancellationToken)
    {
        var store = await dbContext.StoreFronts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Store front not found: {request.Id}");

        await EnsureCanMutateStoreAsync(store.CompanyId, store.BranchId, cancellationToken);

        if (request.IsActive && !store.IsActive)
            await EnsureStoreActivationAvailableAsync(store.CompanyId, store.Id, cancellationToken);

        store.SetActive(request.IsActive, GetUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SetStoreFrontStatusResult(true);
    }

    public async Task<DeleteStoreFrontResult> Handle(DeleteStoreFrontCommand request, CancellationToken cancellationToken)
    {
        var store = await dbContext.StoreFronts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Store front not found: {request.Id}");

        await EnsureCanMutateStoreAsync(store.CompanyId, store.BranchId, cancellationToken);

        var userId = GetUserId();
        store.Remove(userId);

        var items = await dbContext.StoreFrontSellableItems
            .Where(x => x.StoreFrontId == request.Id)
            .ToListAsync(cancellationToken);
        foreach (var item in items)
            item.Remove(userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteStoreFrontResult(true);
    }

    public async Task<SaveStoreFrontItemsResult> Handle(SaveStoreFrontItemsCommand request, CancellationToken cancellationToken)
    {
        var store = await dbContext.StoreFronts.FirstOrDefaultAsync(x => x.Id == request.StoreFrontId, cancellationToken)
            ?? throw new NotFoundException($"Store front not found: {request.StoreFrontId}");

        await EnsureCanMutateStoreAsync(store.CompanyId, store.BranchId, cancellationToken);

        var requestedBySku = request.Items
            .Where(x => x.ProductSkuId != Guid.Empty)
            .GroupBy(x => x.ProductSkuId)
            .ToDictionary(x => x.Key, x => x.First());

        var currentItems = await dbContext.StoreFrontSellableItems
            .Where(x => x.StoreFrontId == request.StoreFrontId)
            .ToListAsync(cancellationToken);

        foreach (var current in currentItems.Where(x => !requestedBySku.ContainsKey(x.ProductSkuId)))
            current.Remove(GetUserId());

        foreach (var dto in requestedBySku.Values)
        {
            var existing = currentItems.FirstOrDefault(x => x.ProductSkuId == dto.ProductSkuId);
            if (existing is null)
            {
                await dbContext.StoreFrontSellableItems.AddAsync(StoreFrontSellableItem.Create(store.Id, dto, GetUserId()), cancellationToken);
            }
            else
            {
                existing.Update(dto, GetUserId());
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveStoreFrontItemsResult(true);
    }

    private async Task EnsureStoreActivationAvailableAsync(Guid companyId, Guid? currentStoreId, CancellationToken cancellationToken)
    {
        var parentCompanyId = await companyHierarchyReader.GetParentCompanyIdForCompanyAsync(companyId, cancellationToken);
        var hierarchyIds = await companyHierarchyReader.GetCompanyHierarchyIdsAsync(parentCompanyId, cancellationToken);
        var usedActivations = await dbContext.StoreFronts
            .CountAsync(x => hierarchyIds.Contains(x.CompanyId) && x.IsActive && (!currentStoreId.HasValue || x.Id != currentStoreId.Value), cancellationToken);

        await entitlementService.EnsureActivationAvailableAsync(BusinessLineKeys.StoreFront, companyId, usedActivations, cancellationToken);
    }

    private async Task EnsureTypeAsync(Guid companyId, Guid typeId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.StoreFrontTypes.AnyAsync(x => x.Id == typeId && x.CompanyId == companyId && x.IsActive, cancellationToken);
        if (!exists)
            throw new InvalidOperationException("Store front type is not active or does not belong to this company");
    }

    private async Task EnsureCanMutateStoreAsync(Guid companyId, Guid? branchId, CancellationToken cancellationToken)
    {
        var branchAccess = await sender.Send(new GetCurrentUserBranchAccessQuery(companyId), cancellationToken);
        if (!BranchScopePolicy.CanMutate(branchAccess, branchId))
            throw new ForbiddenException("You do not have permission to change this storefront branch scope.");
    }

    private string GetUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not authenticated");
}

public class StoreFrontEndpoints : ICarterModule
{
    private const string Route = "/api/v1/store-front";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Route}/types/company/{{companyId:guid}}", async (Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetStoreFrontTypesQuery(companyId));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontStorePermissions.View);

        app.MapPost($"{Route}/types", async (SaveStoreFrontTypeRequest request, HttpContext context, ISender sender) =>
        {
            EnsurePermission(
                context.User,
                request.Type.Id == Guid.Empty ? PermissionList.StoreFrontStorePermissions.Create : PermissionList.StoreFrontStorePermissions.Edit);
            var result = await sender.Send(new SaveStoreFrontTypeCommand(request.Type));
            return Results.Ok(result);
        }).RequireAuthorization();

        app.MapDelete($"{Route}/types/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteStoreFrontTypeCommand(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontStorePermissions.Delete);

        app.MapGet($"{Route}/stores/company/{{companyId:guid}}", async (Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetStoreFrontsByCompanyQuery(companyId));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontStorePermissions.View);

        app.MapGet($"{Route}/stores/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetStoreFrontByIdQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontStorePermissions.View);

        app.MapPost($"{Route}/stores", async (SaveStoreFrontRequest request, HttpContext context, ISender sender) =>
        {
            EnsurePermission(
                context.User,
                request.Store.Id == Guid.Empty ? PermissionList.StoreFrontStorePermissions.Create : PermissionList.StoreFrontStorePermissions.Edit);
            var result = await sender.Send(new SaveStoreFrontCommand(request.Store));
            return Results.Ok(result.Adapt<SaveStoreFrontResponse>());
        }).RequireAuthorization();

        app.MapPatch($"{Route}/stores/{{id:guid}}/status", async (Guid id, SetStoreFrontStatusRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SetStoreFrontStatusCommand(id, request.IsActive));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontStorePermissions.Edit);

        app.MapDelete($"{Route}/stores/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteStoreFrontCommand(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontStorePermissions.Delete);

        app.MapGet($"{Route}/stores/{{id:guid}}/items", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetStoreFrontItemsQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontItemPermissions.View);

        app.MapPut($"{Route}/stores/{{id:guid}}/items", async (Guid id, SaveStoreFrontItemsRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SaveStoreFrontItemsCommand(id, request.Items));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontItemPermissions.Edit);

        app.MapGet($"{Route}/stores/{{id:guid}}/catalog", async (Guid id, Guid? customerId, string? searchText, ISender sender) =>
        {
            var result = await sender.Send(new GetStoreFrontCatalogQuery(id, customerId, searchText));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.StoreFrontPosPermissions.View);
    }

    private static void EnsurePermission(ClaimsPrincipal user, string permission)
    {
        if (!user.Claims.Any(c => c.Value == permission))
            throw new ForbiddenException($"Missing permission: {permission}");
    }
}
