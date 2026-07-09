namespace Procurement.Procurement.Features;

public class ProcurementEnhancementEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/procurement/supplier-items/company/{companyId:guid}", async (
                Guid companyId,
                Guid? supplierId,
                Guid? productId,
                Guid? productSkuId,
                ISender sender) =>
            Results.Ok(new { items = (await sender.Send(new GetSupplierItemsQuery(companyId, supplierId, productId, productSkuId))).Items }))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.View);

        app.MapPost("/api/v1/procurement/supplier-items", async (SupplierItemDto item, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertSupplierItemCommand(item))))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.Create);

        app.MapDelete("/api/v1/procurement/supplier-items/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteSupplierItemCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.PurchaseOrderPermissions.Delete);

        app.MapGet("/api/v1/procurement/vendor-pricelists/company/{companyId:guid}", async (
                Guid companyId,
                Guid? supplierId,
                Guid? productId,
                Guid? productSkuId,
                ISender sender) =>
            Results.Ok(new { items = (await sender.Send(new GetVendorPricelistsQuery(companyId, supplierId, productId, productSkuId))).Items }))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.View);

        app.MapPost("/api/v1/procurement/vendor-pricelists", async (VendorPricelistDto item, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertVendorPricelistCommand(item))))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.Create);

        app.MapDelete("/api/v1/procurement/vendor-pricelists/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteVendorPricelistCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.PurchaseOrderPermissions.Delete);

        app.MapGet("/api/v1/procurement/reordering-rules/company/{companyId:guid}", async (
                Guid companyId,
                Guid? supplierId,
                Guid? productId,
                Guid? productSkuId,
                ISender sender) =>
            Results.Ok(new { items = (await sender.Send(new GetReorderingRulesQuery(companyId, supplierId, productId, productSkuId))).Items }))
            .RequireAuthorization(PermissionList.PurchaseRequestPermissions.View);

        app.MapGet("/api/v1/procurement/replenishment/company/{companyId:guid}", async (
                Guid companyId,
                Guid? branchId,
                Guid? warehouseId,
                Guid? productSkuId,
                ReplenishmentTriggerMode? triggerMode,
                bool? includeAutomatic,
                bool? orderToMax,
                ISender sender) =>
            Results.Ok(new
            {
                items = (await sender.Send(new GetReplenishmentSuggestionsQuery(
                    companyId,
                    branchId,
                    warehouseId,
                    productSkuId,
                    triggerMode,
                    includeAutomatic ?? false,
                    orderToMax ?? false))).Items
            }))
            .RequireAuthorization(PermissionList.PurchaseRequestPermissions.View);

        app.MapPost("/api/v1/procurement/replenishment/purchase-requests", async (CreatePurchaseRequestFromReplenishmentDto request, ISender sender) =>
            Results.Ok(await sender.Send(new CreatePurchaseRequestFromReplenishmentCommand(request))))
            .RequireAuthorization(PermissionList.PurchaseRequestPermissions.Create);

        app.MapPost("/api/v1/procurement/replenishment/automatic", async (RunAutomaticReplenishmentDto request, ISender sender) =>
            Results.Ok(new { result = await sender.Send(new RunAutomaticReplenishmentCommand(request)) }))
            .RequireAuthorization(PermissionList.PurchaseRequestPermissions.Create);

        app.MapPost("/api/v1/procurement/reordering-rules", async (ReorderingRuleDto item, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertReorderingRuleCommand(item))))
            .RequireAuthorization(PermissionList.PurchaseRequestPermissions.Create);

        app.MapDelete("/api/v1/procurement/reordering-rules/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteReorderingRuleCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.PurchaseRequestPermissions.Delete);

        app.MapGet("/api/v1/procurement/tracker/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { rows = (await sender.Send(new GetProcurementTrackerQuery(companyId))).Rows }))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.View);

        app.MapGet("/api/v1/procurement/supplier-scorecard/company/{companyId:guid}", async (Guid companyId, Guid? supplierId, ISender sender) =>
            Results.Ok(new { rows = (await sender.Send(new GetSupplierScorecardQuery(companyId, supplierId))).Rows }))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.View);
    }
}
