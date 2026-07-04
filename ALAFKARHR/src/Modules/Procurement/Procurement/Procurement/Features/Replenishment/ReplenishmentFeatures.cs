using SharedWithUI.Catalog.Enums;

namespace Procurement.Procurement.Features;

public record GetReplenishmentSuggestionsQuery(Guid CompanyId, Guid? BranchId, Guid? WarehouseId, Guid? ProductSkuId)
    : IQuery<GetReplenishmentSuggestionsResult>;

public record GetReplenishmentSuggestionsResult(IReadOnlyCollection<ReplenishmentSuggestionDto> Items);

public record CreatePurchaseRequestFromReplenishmentCommand(CreatePurchaseRequestFromReplenishmentDto Request)
    : ICommand<CreateProcurementDocumentResult>;

public class CreatePurchaseRequestFromReplenishmentValidator : AbstractValidator<CreatePurchaseRequestFromReplenishmentCommand>
{
    public CreatePurchaseRequestFromReplenishmentValidator()
    {
        RuleFor(x => x.Request.CompanyId).NotEmpty();
        RuleFor(x => x.Request.Lines).NotEmpty();
        RuleForEach(x => x.Request.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ReorderingRuleId).NotEmpty();
            line.RuleFor(x => x.ProductSkuId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
        });
    }
}

public class GetReplenishmentSuggestionsHandler(ProcurementDbContext dbContext, ISender sender)
    : IQueryHandler<GetReplenishmentSuggestionsQuery, GetReplenishmentSuggestionsResult>
{
    public async Task<GetReplenishmentSuggestionsResult> Handle(GetReplenishmentSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var access = await sender.Send(new GetCurrentUserBranchAccessQuery(request.CompanyId), cancellationToken);
        if (!BranchScopePolicy.CanFilter(access, request.BranchId))
            throw new ForbiddenException("You do not have permission to filter replenishment by this branch.");

        if (request.WarehouseId.HasValue && request.BranchId.HasValue)
            await sender.Send(new EnsureWarehouseBranchScopeQuery(request.CompanyId, request.WarehouseId.Value, request.BranchId.Value), cancellationToken);

        var rules = await dbContext.ReorderingRules.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.IsActive)
            .Where(x => !request.ProductSkuId.HasValue || x.ProductSkuId == request.ProductSkuId.Value)
            .Where(x => !request.WarehouseId.HasValue || !x.WarehouseId.HasValue || x.WarehouseId == request.WarehouseId.Value)
            .OrderBy(x => x.ProductSkuId)
            .ToListAsync(cancellationToken);

        var supplierItems = await dbContext.SupplierItems.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .ToListAsync(cancellationToken);
        var today = DateTime.UtcNow.Date;
        var vendorPricelists = await dbContext.VendorPricelists.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId
                && x.ValidFrom.Date <= today
                && (!x.ValidTo.HasValue || x.ValidTo.Value.Date >= today))
            .ToListAsync(cancellationToken);

        var suggestions = new List<ReplenishmentSuggestionDto>();
        foreach (var rule in rules)
        {
            suggestions.Add(await BuildSuggestionAsync(
                rule,
                request.BranchId,
                request.WarehouseId ?? rule.WarehouseId,
                supplierItems,
                vendorPricelists,
                cancellationToken));
        }

        return new GetReplenishmentSuggestionsResult(suggestions
            .Where(x => x.SuggestedQuantity > 0 || !x.CanCreatePurchaseRequest)
            .OrderByDescending(x => !x.CanCreatePurchaseRequest)
            .ThenBy(x => x.ProductNameEng)
            .ThenBy(x => x.SkuCode)
            .ToList());
    }

    private async Task<ReplenishmentSuggestionDto> BuildSuggestionAsync(
        ReorderingRule rule,
        Guid? branchId,
        Guid? warehouseId,
        IReadOnlyCollection<SupplierItem> supplierItems,
        IReadOnlyCollection<VendorPricelist> vendorPricelists,
        CancellationToken cancellationToken)
    {
        var productName = string.Empty;
        var productNameEng = string.Empty;
        var skuCode = string.Empty;

        try
        {
            var product = (await sender.Send(new GetProductByIdQuery(rule.ProductId), cancellationToken)).Product;
            var sku = product.Skus.FirstOrDefault(x => x.Id == rule.ProductSkuId);
            productName = sku?.ProductName ?? product.Name;
            productNameEng = sku?.ProductNameEng ?? product.NameEng;
            skuCode = sku?.SkuCode ?? string.Empty;

            var context = await sender.Send(new GetProductSkuInventoryContextQuery(rule.CompanyId, rule.ProductSkuId), cancellationToken);
            if (!IsReplenishable(context, out var warning))
                return Warning(rule, productName, productNameEng, skuCode, warning);

            var availability = await sender.Send(new GetSkuAvailabilityQuery(rule.CompanyId, rule.ProductSkuId, warehouseId, branchId), cancellationToken);
            var warehouse = warehouseId.HasValue
                ? availability.Warehouses.FirstOrDefault(x => x.WarehouseId == warehouseId.Value)
                : null;

            var current = warehouse?.TotalQuantity ?? availability.TotalQuantity;
            var reserved = warehouse?.ReservedQuantity ?? availability.ReservedQuantity;
            var available = warehouse?.AvailableQuantity ?? availability.AvailableQuantity;
            if (available >= rule.MinimumQuantity)
                return new ReplenishmentSuggestionDto
                {
                    ReorderingRuleId = rule.Id,
                    CompanyId = rule.CompanyId,
                    ProductId = rule.ProductId,
                    ProductSkuId = rule.ProductSkuId,
                    ProductName = productName,
                    ProductNameEng = productNameEng,
                    SkuCode = skuCode,
                    WarehouseId = warehouse?.WarehouseId ?? warehouseId,
                    WarehouseName = warehouse?.WarehouseName,
                    WarehouseNameEng = warehouse?.WarehouseNameEng,
                    BranchId = warehouse?.BranchId ?? branchId,
                    MinimumQuantity = rule.MinimumQuantity,
                    MaximumQuantity = rule.MaximumQuantity,
                    ReorderQuantity = rule.ReorderQuantity,
                    CurrentQuantity = current,
                    ReservedQuantity = reserved,
                    AvailableQuantity = available,
                    LeadTimeDays = rule.LeadTimeDays,
                    ExpectedDate = DateTime.UtcNow.Date.AddDays(rule.LeadTimeDays),
                    CanCreatePurchaseRequest = false,
                    WarningCode = "AboveMinimum",
                    WarningMessage = "Available quantity is above the reordering minimum."
                };

            var supplier = ResolveSupplier(rule, supplierItems, vendorPricelists);
            var suggested = ResolveSuggestedQuantity(rule, available, supplier.SupplierItem?.MinimumOrderQuantity ?? 0m);
            var leadDays = rule.LeadTimeDays > 0 ? rule.LeadTimeDays : supplier.SupplierItem?.LeadTimeDays ?? 0;
            return new ReplenishmentSuggestionDto
            {
                ReorderingRuleId = rule.Id,
                CompanyId = rule.CompanyId,
                ProductId = rule.ProductId,
                ProductSkuId = rule.ProductSkuId,
                ProductName = productName,
                ProductNameEng = productNameEng,
                SkuCode = skuCode,
                WarehouseId = warehouse?.WarehouseId ?? warehouseId,
                WarehouseName = warehouse?.WarehouseName,
                WarehouseNameEng = warehouse?.WarehouseNameEng,
                BranchId = warehouse?.BranchId ?? branchId,
                SupplierId = supplier.SupplierId,
                SupplierName = supplier.SupplierName,
                CurrencyId = supplier.VendorPricelist?.CurrencyId,
                UnitCost = supplier.VendorPricelist?.UnitCost ?? 0m,
                MinimumQuantity = rule.MinimumQuantity,
                MaximumQuantity = rule.MaximumQuantity,
                ReorderQuantity = rule.ReorderQuantity,
                MinimumOrderQuantity = supplier.SupplierItem?.MinimumOrderQuantity ?? 0m,
                CurrentQuantity = current,
                ReservedQuantity = reserved,
                AvailableQuantity = available,
                SuggestedQuantity = suggested,
                LeadTimeDays = leadDays,
                ExpectedDate = DateTime.UtcNow.Date.AddDays(leadDays),
                CanCreatePurchaseRequest = supplier.SupplierId.HasValue && suggested > 0,
                WarningCode = supplier.SupplierId.HasValue ? string.Empty : "MissingSupplier",
                WarningMessage = supplier.SupplierId.HasValue ? string.Empty : "No supplier item or vendor pricelist could be matched to this SKU."
            };
        }
        catch (Exception ex) when (ex is BadRequestException or NotFoundException)
        {
            return Warning(rule, productName, productNameEng, skuCode, ex.Message);
        }
    }

    private static bool IsReplenishable(GetProductSkuInventoryContextResult context, out string warning)
    {
        if (!context.ProductIsActive || !context.SkuIsActive || !context.CategoryIsActive || !context.BrandIsActive || !context.UnitIsActive)
        {
            warning = "Catalog product, SKU, category, brand, or unit is inactive.";
            return false;
        }

        if (context.ProductType == CatalogProductType.Service)
        {
            warning = "Service products cannot be replenished into Inventory.";
            return false;
        }

        if (context.ProductType == CatalogProductType.Combo || context.ProductionType == SkuProductionType.CompositeBundle)
        {
            warning = "Combo parent SKUs are sold as kits; replenish their component SKUs instead.";
            return false;
        }

        if (!context.IsInventoryTracked)
        {
            warning = "Only inventory-tracked SKUs can be replenished.";
            return false;
        }

        warning = string.Empty;
        return true;
    }

    private static decimal ResolveSuggestedQuantity(ReorderingRule rule, decimal available, decimal minimumOrderQuantity)
    {
        var suggested = rule.ReorderQuantity > 0
            ? rule.ReorderQuantity
            : Math.Max(rule.MaximumQuantity - available, rule.MinimumQuantity - available);
        return Math.Max(suggested, minimumOrderQuantity);
    }

    private static (Guid? SupplierId, string? SupplierName, SupplierItem? SupplierItem, VendorPricelist? VendorPricelist) ResolveSupplier(
        ReorderingRule rule,
        IReadOnlyCollection<SupplierItem> supplierItems,
        IReadOnlyCollection<VendorPricelist> vendorPricelists)
    {
        var skuSupplierItems = supplierItems.Where(x => x.ProductSkuId == rule.ProductSkuId).ToList();
        var skuPricelists = vendorPricelists.Where(x => x.ProductSkuId == rule.ProductSkuId).ToList();
        SupplierItem? supplierItem = null;
        VendorPricelist? pricelist = null;

        if (rule.SupplierId.HasValue)
        {
            supplierItem = skuSupplierItems
                .OrderByDescending(x => x.IsPreferred)
                .FirstOrDefault(x => x.SupplierId == rule.SupplierId.Value);
            pricelist = skuPricelists
                .OrderByDescending(x => x.IsPreferred)
                .ThenBy(x => x.MinimumQuantity)
                .FirstOrDefault(x => x.SupplierId == rule.SupplierId.Value);
            return (rule.SupplierId, supplierItem?.SupplierName ?? pricelist?.SupplierName, supplierItem, pricelist);
        }

        supplierItem = skuSupplierItems
            .OrderByDescending(x => x.IsPreferred)
            .ThenBy(x => x.MinimumOrderQuantity)
            .FirstOrDefault();
        if (supplierItem is not null)
        {
            pricelist = skuPricelists
                .OrderByDescending(x => x.IsPreferred)
                .ThenBy(x => x.MinimumQuantity)
                .FirstOrDefault(x => x.SupplierId == supplierItem.SupplierId);
            return (supplierItem.SupplierId, supplierItem.SupplierName ?? pricelist?.SupplierName, supplierItem, pricelist);
        }

        pricelist = skuPricelists
            .OrderByDescending(x => x.IsPreferred)
            .ThenBy(x => x.MinimumQuantity)
            .FirstOrDefault();
        return (pricelist?.SupplierId, pricelist?.SupplierName, null, pricelist);
    }

    private static ReplenishmentSuggestionDto Warning(ReorderingRule rule, string productName, string productNameEng, string skuCode, string warning) =>
        new()
        {
            ReorderingRuleId = rule.Id,
            CompanyId = rule.CompanyId,
            ProductId = rule.ProductId,
            ProductSkuId = rule.ProductSkuId,
            ProductName = productName,
            ProductNameEng = productNameEng,
            SkuCode = skuCode,
            WarehouseId = rule.WarehouseId,
            MinimumQuantity = rule.MinimumQuantity,
            MaximumQuantity = rule.MaximumQuantity,
            ReorderQuantity = rule.ReorderQuantity,
            LeadTimeDays = rule.LeadTimeDays,
            ExpectedDate = DateTime.UtcNow.Date.AddDays(rule.LeadTimeDays),
            CanCreatePurchaseRequest = false,
            WarningCode = "InvalidSetup",
            WarningMessage = warning
        };
}

public class CreatePurchaseRequestFromReplenishmentHandler(ProcurementDbContext dbContext, ISender sender)
    : ICommandHandler<CreatePurchaseRequestFromReplenishmentCommand, CreateProcurementDocumentResult>
{
    public async Task<CreateProcurementDocumentResult> Handle(CreatePurchaseRequestFromReplenishmentCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, request.CompanyId, request.BranchId, cancellationToken);

        var ruleIds = command.Request.Lines.Select(x => x.ReorderingRuleId).Distinct().ToList();
        var rules = await dbContext.ReorderingRules.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && x.IsActive && ruleIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        if (rules.Count != ruleIds.Count)
            throw new BadRequestException("One or more reordering rules are inactive or do not belong to this company.");

        var document = new ProcurementDocumentDto
        {
            Kind = ProcurementDocumentKind.PurchaseRequest,
            CompanyId = request.CompanyId,
            BranchId = request.BranchId,
            SupplierId = request.SupplierId,
            CurrencyId = request.CurrencyId,
            WarehouseId = request.WarehouseId,
            DocumentDate = DateTime.UtcNow,
            SourceDocumentNumber = "Replenishment",
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? "Created from replenishment suggestions." : request.Notes
        };

        foreach (var line in request.Lines)
        {
            if (!rules.TryGetValue(line.ReorderingRuleId, out var rule) || rule.ProductSkuId != line.ProductSkuId)
                throw new BadRequestException("Selected replenishment line does not match its reordering rule.");

            var warehouseId = line.WarehouseId ?? request.WarehouseId ?? rule.WarehouseId;
            if (warehouseId.HasValue && request.BranchId.HasValue)
                await sender.Send(new EnsureWarehouseBranchScopeQuery(request.CompanyId, warehouseId.Value, request.BranchId.Value), cancellationToken);

            var suggestion = (await sender.Send(
                    new GetReplenishmentSuggestionsQuery(request.CompanyId, request.BranchId, warehouseId, line.ProductSkuId),
                    cancellationToken))
                .Items
                .FirstOrDefault(x => x.ReorderingRuleId == line.ReorderingRuleId)
                ?? throw new BadRequestException("Replenishment suggestion could not be recalculated.");

            if (!suggestion.CanCreatePurchaseRequest)
                throw new BadRequestException(string.IsNullOrWhiteSpace(suggestion.WarningMessage)
                    ? "Selected replenishment suggestion is not ready for purchase request creation."
                    : suggestion.WarningMessage);

            document.SupplierId ??= line.SupplierId ?? suggestion.SupplierId;
            document.SupplierName ??= suggestion.SupplierName;
            document.CurrencyId ??= suggestion.CurrencyId;
            document.WarehouseId ??= warehouseId;
            document.Lines.Add(new ProcurementDocumentLineDto
            {
                ProductId = suggestion.ProductId,
                ProductSkuId = suggestion.ProductSkuId,
                ProductName = suggestion.ProductName,
                ProductNameEng = suggestion.ProductNameEng,
                SkuCode = suggestion.SkuCode,
                WarehouseId = warehouseId,
                ReorderingRuleId = suggestion.ReorderingRuleId,
                Quantity = line.Quantity,
                UnitCost = suggestion.UnitCost,
                Notes = $"Replenishment rule {suggestion.ReorderingRuleId:N}"
            });
        }

        return await sender.Send(new CreateProcurementDocumentCommand(ProcurementDocumentKind.PurchaseRequest, document), cancellationToken);
    }
}
