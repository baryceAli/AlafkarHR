using SharedWithUI.Catalog.Enums;

namespace Procurement.Procurement.Features;

public record GetReplenishmentSuggestionsQuery(
    Guid CompanyId,
    Guid? BranchId,
    Guid? WarehouseId,
    Guid? ProductSkuId,
    ReplenishmentTriggerMode? TriggerMode = null,
    bool IncludeAutomatic = false,
    bool OrderToMax = false)
    : IQuery<GetReplenishmentSuggestionsResult>;

public record GetReplenishmentSuggestionsResult(IReadOnlyCollection<ReplenishmentSuggestionDto> Items);

public record CreatePurchaseRequestFromReplenishmentCommand(CreatePurchaseRequestFromReplenishmentDto Request)
    : ICommand<CreateProcurementDocumentResult>;

public record RunAutomaticReplenishmentCommand(RunAutomaticReplenishmentDto Request)
    : ICommand<RunAutomaticReplenishmentResultDto>;

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

        if (request.TriggerMode.HasValue)
            rules = rules.Where(x => ResolveTriggerMode(x) == request.TriggerMode.Value).ToList();
        else if (!request.IncludeAutomatic)
            rules = rules.Where(x => ResolveTriggerMode(x) != ReplenishmentTriggerMode.Automatic).ToList();

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
                request.OrderToMax,
                cancellationToken));
        }

        return new GetReplenishmentSuggestionsResult(suggestions
            .Where(x => x.SuggestedQuantity > 0 || (!string.Equals(x.WarningCode, "AboveMinimum", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(x.WarningCode)))
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
        bool orderToMax,
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
            var projected = await sender.Send(
                new GetProjectedStockQuery(rule.CompanyId, branchId, warehouseId, rule.ProductSkuId),
                cancellationToken);
            var projectedRows = warehouseId.HasValue
                ? projected.Rows.Where(x => x.WarehouseId == warehouseId.Value).ToList()
                : projected.Rows.ToList();

            var current = warehouse?.TotalQuantity ?? availability.TotalQuantity;
            var reserved = warehouse?.ReservedQuantity ?? availability.ReservedQuantity;
            var available = warehouse?.AvailableQuantity ?? availability.AvailableQuantity;
            var incoming = projectedRows.Sum(x => x.IncomingQuantity);
            var outgoing = projectedRows.Sum(x => x.OutgoingQuantity);
            var forecasted = projectedRows.Count == 0 ? available : projectedRows.Sum(x => x.ForecastedQuantity);
            var isBelowMinimum = forecasted < rule.MinimumQuantity;
            var isOrderToMaxEligible = forecasted < rule.MaximumQuantity;
            if (!isBelowMinimum && !orderToMax)
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
                    MinimumOrderQuantity = 0m,
                    CurrentQuantity = current,
                    ReservedQuantity = reserved,
                    AvailableQuantity = available,
                    IncomingQuantity = incoming,
                    OutgoingQuantity = outgoing,
                    ForecastedQuantity = forecasted,
                    LeadTimeDays = rule.LeadTimeDays,
                    HorizonDays = rule.HorizonDays,
                    ExpectedDate = DateTime.UtcNow.Date.AddDays(rule.LeadTimeDays + rule.HorizonDays),
                    TriggerMode = ResolveTriggerMode(rule),
                    IsBelowMinimum = false,
                    IsOrderToMaxEligible = isOrderToMaxEligible,
                    CanCreatePurchaseRequest = false,
                    WarningCode = "AboveMinimum",
                    WarningMessage = "Forecasted quantity is above the reordering minimum."
                };

            var supplier = ResolveSupplier(rule, supplierItems, vendorPricelists);
            var suggested = ResolveSuggestedQuantity(rule, forecasted, supplier.SupplierItem?.MinimumOrderQuantity ?? 0m, orderToMax);
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
                IncomingQuantity = incoming,
                OutgoingQuantity = outgoing,
                ForecastedQuantity = forecasted,
                SuggestedQuantity = suggested,
                LeadTimeDays = leadDays,
                HorizonDays = rule.HorizonDays,
                ExpectedDate = DateTime.UtcNow.Date.AddDays(leadDays + rule.HorizonDays),
                TriggerMode = ResolveTriggerMode(rule),
                IsBelowMinimum = isBelowMinimum,
                IsOrderToMaxEligible = isOrderToMaxEligible,
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

    private static decimal ResolveSuggestedQuantity(ReorderingRule rule, decimal forecasted, decimal minimumOrderQuantity, bool orderToMax)
    {
        var targetGap = Math.Max(rule.MaximumQuantity - forecasted, 0m);
        var minimumGap = Math.Max(rule.MinimumQuantity - forecasted, 0m);
        var suggested = orderToMax
            ? targetGap
            : rule.ReorderQuantity > 0 ? rule.ReorderQuantity : Math.Max(targetGap, minimumGap);
        suggested = Math.Max(suggested, minimumOrderQuantity);
        if (rule.MultipleQuantity > 0 && suggested > 0)
            suggested = Math.Ceiling(suggested / rule.MultipleQuantity) * rule.MultipleQuantity;

        return suggested;
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
            HorizonDays = rule.HorizonDays,
            LeadTimeDays = rule.LeadTimeDays,
            ExpectedDate = DateTime.UtcNow.Date.AddDays(rule.LeadTimeDays + rule.HorizonDays),
            TriggerMode = ResolveTriggerMode(rule),
            IsBelowMinimum = false,
            IsOrderToMaxEligible = false,
            CanCreatePurchaseRequest = false,
            WarningCode = "InvalidSetup",
            WarningMessage = warning
        };

    private static ReplenishmentTriggerMode ResolveTriggerMode(ReorderingRule rule) => rule.ResolveTriggerMode();
}

public class CreatePurchaseRequestFromReplenishmentHandler(ProcurementDbContext dbContext, ISender sender, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreatePurchaseRequestFromReplenishmentCommand, CreateProcurementDocumentResult>
{
    public async Task<CreateProcurementDocumentResult> Handle(CreatePurchaseRequestFromReplenishmentCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, request.CompanyId, request.BranchId, cancellationToken);

        var ruleIds = command.Request.Lines.Select(x => x.ReorderingRuleId).Distinct().ToList();
        var rules = await dbContext.ReorderingRules
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
                    new GetReplenishmentSuggestionsQuery(request.CompanyId, request.BranchId, warehouseId, line.ProductSkuId, IncludeAutomatic: true, OrderToMax: request.OrderToMax),
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

        var result = await sender.Send(new CreateProcurementDocumentCommand(ProcurementDocumentKind.PurchaseRequest, document), cancellationToken);
        var documentNumber = await dbContext.ProcurementDocuments.AsNoTracking()
            .Where(x => x.Id == result.Id)
            .Select(x => x.Number)
            .FirstOrDefaultAsync(cancellationToken);
        var userId = CreateProcurementDocumentHandler.GetUserId(httpContextAccessor);
        foreach (var rule in rules.Values)
            rule.MarkReplenishmentRun(result.Id, documentNumber, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }
}

public class RunAutomaticReplenishmentHandler(ProcurementDbContext dbContext, ISender sender)
    : ICommandHandler<RunAutomaticReplenishmentCommand, RunAutomaticReplenishmentResultDto>
{
    public async Task<RunAutomaticReplenishmentResultDto> Handle(RunAutomaticReplenishmentCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        await CreateProcurementDocumentHandler.EnsureCanMutateBranchAsync(sender, request.CompanyId, request.BranchId, cancellationToken);

        var today = DateTime.UtcNow.Date;
        var automaticRules = await dbContext.ReorderingRules.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId
                && x.IsActive
                && (x.TriggerMode == ReplenishmentTriggerMode.Automatic || x.AutoCreatePurchaseRequest))
            .Where(x => !request.WarehouseId.HasValue || !x.WarehouseId.HasValue || x.WarehouseId == request.WarehouseId.Value)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var suggestions = (await sender.Send(
                new GetReplenishmentSuggestionsQuery(
                    request.CompanyId,
                    request.BranchId,
                    request.WarehouseId,
                    null,
                    ReplenishmentTriggerMode.Automatic,
                    IncludeAutomatic: true),
                cancellationToken))
            .Items
            .Where(x => x.CanCreatePurchaseRequest
                && x.IsBelowMinimum
                && automaticRules.TryGetValue(x.ReorderingRuleId, out var rule)
                && (!rule.LastGeneratedAt.HasValue || rule.LastGeneratedAt.Value.Date < today))
            .ToList();

        var result = new RunAutomaticReplenishmentResultDto();
        foreach (var group in suggestions.GroupBy(x => new { x.BranchId, x.WarehouseId, x.SupplierId, x.CurrencyId }))
        {
            if (!group.Key.SupplierId.HasValue)
                continue;

            var createResult = await sender.Send(new CreatePurchaseRequestFromReplenishmentCommand(new CreatePurchaseRequestFromReplenishmentDto
            {
                CompanyId = request.CompanyId,
                BranchId = group.Key.BranchId ?? request.BranchId,
                WarehouseId = group.Key.WarehouseId ?? request.WarehouseId,
                SupplierId = group.Key.SupplierId,
                CurrencyId = group.Key.CurrencyId,
                Notes = "Created by automatic replenishment.",
                Lines = group.Select(item => new CreatePurchaseRequestFromReplenishmentLineDto
                {
                    ReorderingRuleId = item.ReorderingRuleId,
                    ProductSkuId = item.ProductSkuId,
                    WarehouseId = item.WarehouseId,
                    SupplierId = item.SupplierId,
                    Quantity = item.SuggestedQuantity
                }).ToList()
            }), cancellationToken);

            result.DocumentsCreated++;
            result.LinesCreated += group.Count();
            result.DocumentIds.Add(createResult.Id);
        }

        var skipped = suggestions.Count(x => !x.SupplierId.HasValue);
        if (skipped > 0)
            result.Warnings.Add($"{skipped} automatic replenishment line(s) were skipped because no supplier was available.");

        return result;
    }
}
