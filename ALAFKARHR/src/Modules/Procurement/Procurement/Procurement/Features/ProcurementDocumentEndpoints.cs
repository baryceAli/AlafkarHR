namespace Procurement.Procurement.Features;

public record CreateProcurementDocumentRequest(ProcurementDocumentDto Document);
public record CreateProcurementDocumentResponse(Guid Id);
public record UpdateProcurementDocumentRequest(ProcurementDocumentDto Document);

public class ProcurementDocumentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        MapDocumentRoutes(app, "purchase-requests", ProcurementDocumentKind.PurchaseRequest, ProcurementPermissionSet.ForPurchaseRequests());
        MapDocumentRoutes(app, "requests-for-quotation", ProcurementDocumentKind.RequestForQuotation, ProcurementPermissionSet.ForRequestsForQuotation());
        MapDocumentRoutes(app, "supplier-quotations", ProcurementDocumentKind.SupplierQuotation, ProcurementPermissionSet.ForSupplierQuotations());
        MapDocumentRoutes(app, "purchase-orders", ProcurementDocumentKind.PurchaseOrder, ProcurementPermissionSet.ForPurchaseOrders());
        MapDocumentRoutes(app, "goods-receipts", ProcurementDocumentKind.GoodsReceipt, ProcurementPermissionSet.ForGoodsReceipts());
        MapDocumentRoutes(app, "purchase-returns", ProcurementDocumentKind.PurchaseReturn, ProcurementPermissionSet.ForPurchaseReturns());
        MapDocumentRoutes(app, "supplier-invoices", ProcurementDocumentKind.SupplierInvoice, ProcurementPermissionSet.ForSupplierInvoices());

        app.MapGet("/api/v1/procurement/dashboard", async (Guid? companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetProcurementDashboardQuery(companyId));
            return Results.Ok(new { dashboard = result.Dashboard });
        })
            .WithName("GetProcurementDashboard")
            .Produces<ProcurementDashboardDto>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.Select);

        app.MapGet("/api/v1/procurement/smart-links/company/{companyId:guid}", async (
            Guid companyId,
            Guid? supplierId,
            Guid? productId,
            Guid? productSkuId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetProcurementSmartLinksQuery(companyId, supplierId, productId, productSkuId));
            return Results.Ok(new { partnerLinks = result.PartnerLinks, productLinks = result.ProductLinks });
        })
            .WithName("GetProcurementSmartLinks")
            .Produces<GetProcurementSmartLinksResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.View);

        app.MapPost("/api/v1/procurement/recompute-purchase-controls", async (Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new RecomputePurchaseControlsCommand(companyId));
            return Results.Ok(new { recompute = result });
        })
            .WithName("RecomputePurchaseControls")
            .Produces<ProcurementRecomputeResultDto>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.PurchaseOrderPermissions.Edit);
    }

    private static void MapDocumentRoutes(IEndpointRouteBuilder app, string route, ProcurementDocumentKind kind, ProcurementPermissionSet permissions)
    {
        var baseRoute = $"/api/v1/procurement/{route}";

        app.MapGet(baseRoute, async (
            Guid? companyId,
            int? pageIndex,
            int? pageSize,
            string? searchText,
            Guid? supplierId,
            Guid? productId,
            Guid? productSkuId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetProcurementDocumentsQuery(kind, companyId, pageIndex ?? 1, pageSize ?? 20, searchText, supplierId, productId, productSkuId));
            return Results.Ok(new { documents = result.Documents });
        })
            .WithName($"Get{kind}")
            .Produces<PaginatedResult<ProcurementDocumentDto>>(StatusCodes.Status200OK)
            .RequireAuthorization(permissions.View);

        app.MapGet($"{baseRoute}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProcurementDocumentByIdQuery(id, kind));
            return Results.Ok(new { document = result.Document });
        })
            .WithName($"Get{kind}ById")
            .Produces<ProcurementDocumentDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(permissions.View);

        app.MapPost(baseRoute, async (CreateProcurementDocumentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateProcurementDocumentCommand(kind, request.Document));
            return Results.Created($"{baseRoute}/{result.Id}", result.Adapt<CreateProcurementDocumentResponse>());
        })
            .WithName($"Create{kind}")
            .Produces<CreateProcurementDocumentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(permissions.Create);

        app.MapPut($"{baseRoute}/{{id:guid}}", async (Guid id, UpdateProcurementDocumentRequest request, ISender sender) =>
        {
            await sender.Send(new UpdateProcurementDocumentCommand(id, kind, request.Document));
            return Results.Ok("OK");
        })
            .WithName($"Update{kind}")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(permissions.Edit);

        app.MapDelete($"{baseRoute}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new RemoveProcurementDocumentCommand(id, kind));
            return Results.Ok("OK");
        })
            .WithName($"Remove{kind}")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(permissions.Delete);

        foreach (var action in permissions.WorkflowActions)
        {
            app.MapPost($"{baseRoute}/{{id:guid}}/{action.Route}", async (Guid id, ISender sender) =>
            {
                await sender.Send(new ChangeProcurementDocumentStatusCommand(id, kind, action.Name));
                return Results.Ok("OK");
            })
                .WithName($"{action.Name}{kind}")
                .Produces<string>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization(action.Permission);
        }
    }
}

public record ProcurementPermissionSet(
    string View,
    string Create,
    string Edit,
    string Delete,
    IReadOnlyCollection<ProcurementWorkflowEndpointAction> WorkflowActions)
{
    public static ProcurementPermissionSet ForPurchaseRequests() =>
        new(
            PermissionList.PurchaseRequestPermissions.View,
            PermissionList.PurchaseRequestPermissions.Create,
            PermissionList.PurchaseRequestPermissions.Edit,
            PermissionList.PurchaseRequestPermissions.Delete,
            [
                new("submit", "submit", PermissionList.PurchaseRequestPermissions.Submit),
                new("approve", "approve", PermissionList.PurchaseRequestPermissions.Approve),
                new("reject", "reject", PermissionList.PurchaseRequestPermissions.Reject),
                new("cancel", "cancel", PermissionList.PurchaseRequestPermissions.Cancel)
            ]);

    public static ProcurementPermissionSet ForRequestsForQuotation() =>
        new(
            PermissionList.RequestForQuotationPermissions.View,
            PermissionList.RequestForQuotationPermissions.Create,
            PermissionList.RequestForQuotationPermissions.Edit,
            PermissionList.RequestForQuotationPermissions.Delete,
            [
                new("send", "send", PermissionList.RequestForQuotationPermissions.Submit),
                new("close", "close", PermissionList.RequestForQuotationPermissions.Close),
                new("cancel", "cancel", PermissionList.RequestForQuotationPermissions.Cancel)
            ]);

    public static ProcurementPermissionSet ForSupplierQuotations() =>
        new(
            PermissionList.SupplierQuotationPermissions.View,
            PermissionList.SupplierQuotationPermissions.Create,
            PermissionList.SupplierQuotationPermissions.Edit,
            PermissionList.SupplierQuotationPermissions.Delete,
            [
                new("accept", "accept", PermissionList.SupplierQuotationPermissions.Approve),
                new("reject", "reject", PermissionList.SupplierQuotationPermissions.Reject)
            ]);

    public static ProcurementPermissionSet ForPurchaseOrders() =>
        new(
            PermissionList.PurchaseOrderPermissions.View,
            PermissionList.PurchaseOrderPermissions.Create,
            PermissionList.PurchaseOrderPermissions.Edit,
            PermissionList.PurchaseOrderPermissions.Delete,
            [
                new("approve", "approve", PermissionList.PurchaseOrderPermissions.Approve),
                new("send", "send", PermissionList.PurchaseOrderPermissions.Submit),
                new("close", "close", PermissionList.PurchaseOrderPermissions.Close),
                new("cancel", "cancel", PermissionList.PurchaseOrderPermissions.Cancel)
            ]);

    public static ProcurementPermissionSet ForGoodsReceipts() =>
        new(
            PermissionList.GoodsReceiptPermissions.View,
            PermissionList.GoodsReceiptPermissions.Create,
            PermissionList.GoodsReceiptPermissions.Edit,
            PermissionList.GoodsReceiptPermissions.Delete,
            [
                new("post", "post", PermissionList.GoodsReceiptPermissions.Receive),
                new("cancel", "cancel", PermissionList.GoodsReceiptPermissions.Cancel)
            ]);

    public static ProcurementPermissionSet ForPurchaseReturns() =>
        new(
            PermissionList.PurchaseReturnPermissions.View,
            PermissionList.PurchaseReturnPermissions.Create,
            PermissionList.PurchaseReturnPermissions.Edit,
            PermissionList.PurchaseReturnPermissions.Delete,
            [
                new("post", "post", PermissionList.PurchaseReturnPermissions.Receive),
                new("cancel", "cancel", PermissionList.PurchaseReturnPermissions.Cancel)
            ]);

    public static ProcurementPermissionSet ForSupplierInvoices() =>
        new(
            PermissionList.SupplierInvoicePermissions.View,
            PermissionList.SupplierInvoicePermissions.Create,
            PermissionList.SupplierInvoicePermissions.Edit,
            PermissionList.SupplierInvoicePermissions.Delete,
            [
                new("match", "match", PermissionList.SupplierInvoicePermissions.Approve),
                new("post", "post", PermissionList.SupplierInvoicePermissions.Close),
                new("cancel", "cancel", PermissionList.SupplierInvoicePermissions.Cancel)
            ]);
}

public record ProcurementWorkflowEndpointAction(string Name, string Route, string Permission);
