namespace Procurement.Procurement.Features;

public class ProcurementEnhancementEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/procurement/supplier-items/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { items = (await sender.Send(new GetSupplierItemsQuery(companyId))).Items }))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.View);

        app.MapPost("/api/v1/procurement/supplier-items", async (SupplierItemDto item, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertSupplierItemCommand(item))))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.Create);

        app.MapDelete("/api/v1/procurement/supplier-items/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteSupplierItemCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.PurchaseOrderPermissions.Delete);

        app.MapGet("/api/v1/procurement/vendor-pricelists/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { items = (await sender.Send(new GetVendorPricelistsQuery(companyId))).Items }))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.View);

        app.MapPost("/api/v1/procurement/vendor-pricelists", async (VendorPricelistDto item, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertVendorPricelistCommand(item))))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.Create);

        app.MapDelete("/api/v1/procurement/vendor-pricelists/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteVendorPricelistCommand(id));
            return Results.Ok("OK");
        }).RequireAuthorization(PermissionList.PurchaseOrderPermissions.Delete);

        app.MapGet("/api/v1/procurement/reordering-rules/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { items = (await sender.Send(new GetReorderingRulesQuery(companyId))).Items }))
            .RequireAuthorization(PermissionList.PurchaseRequestPermissions.View);

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

        app.MapGet("/api/v1/procurement/supplier-scorecard/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(new { rows = (await sender.Send(new GetSupplierScorecardQuery(companyId))).Rows }))
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.View);
    }
}
