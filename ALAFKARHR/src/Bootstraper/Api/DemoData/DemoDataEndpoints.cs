using Microsoft.AspNetCore.Mvc;
using SharedWithUI.GeneralSettings.Dtos;
using SharedWithUI.Permissions;

namespace Api.DemoData;

public static class DemoDataEndpoints
{
    private const string Route = "/api/v1/demo-data";

    public static IEndpointRouteBuilder MapDemoDataEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(Route, async (HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataListResponse(await service.ListAsync(cancellationToken)));
        })
            .WithName("ListDemoDataCompanies")
            .Produces<DemoDataListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.View);

        app.MapGet($"{Route}/{{companyCode}}/status", async (string companyCode, HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataStatusResponse(await service.GetStatusAsync(companyCode, cancellationToken)));
        })
            .WithName("GetDemoDataCompanyStatus")
            .Produces<DemoDataStatusResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.View);

        app.MapGet($"{Route}/status", async (HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataStatusResponse(await service.GetStatusAsync("DEMO-ERP", cancellationToken)));
        })
            .WithName("GetDemoDataStatus")
            .Produces<DemoDataStatusResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.View);

        app.MapPost($"{Route}/create", async ([FromBody] DemoDataCreateRequestDto request, HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataOperationResponse(await service.CreateAsync(request, cancellationToken)));
        })
            .WithName("CreateDemoData")
            .Produces<DemoDataOperationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.Create);

        app.MapPost($"{Route}/{{companyCode}}/reset", async (string companyCode, [FromBody] DemoDataConfirmationRequestDto request, HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataOperationResponse(await service.ResetAsync(companyCode, request, cancellationToken)));
        })
            .WithName("ResetDemoDataCompany")
            .Produces<DemoDataOperationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.Reset);

        app.MapPost($"{Route}/{{companyCode}}/admin-password/reset", async (string companyCode, [FromBody] DemoDataConfirmationRequestDto request, HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataOperationResponse(await service.ResetAdminPasswordAsync(companyCode, request, cancellationToken)));
        })
            .WithName("ResetDemoDataCompanyAdminPassword")
            .Produces<DemoDataOperationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.ResetAdminPassword);

        app.MapPost($"{Route}/reset", async ([FromBody] DemoDataConfirmationRequestDto request, HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataOperationResponse(await service.ResetAsync(request.CompanyCode, request, cancellationToken)));
        })
            .WithName("ResetDemoData")
            .Produces<DemoDataOperationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.Reset);

        app.MapDelete($"{Route}/{{companyCode}}", async (string companyCode, [FromBody] DemoDataConfirmationRequestDto request, HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataOperationResponse(await service.DeleteAsync(companyCode, request, cancellationToken)));
        })
            .WithName("DeleteDemoDataCompany")
            .Produces<DemoDataOperationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.Delete);

        app.MapDelete(Route, async ([FromBody] DemoDataConfirmationRequestDto request, HttpContext httpContext, IDemoDataManagementService service, CancellationToken cancellationToken) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            return Results.Ok(new DemoDataOperationResponse(await service.DeleteAsync(request.CompanyCode, request, cancellationToken)));
        })
            .WithName("DeleteDemoData")
            .Produces<DemoDataOperationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.DemoDataPermissions.Delete);

        return app;
    }

    private static bool IsCompanyScoped(HttpContext httpContext) =>
        httpContext.User.HasClaim(claim => claim.Type == "company_id");
}

public sealed record DemoDataStatusResponse(DemoDataStatusDto Status);
public sealed record DemoDataListResponse(IReadOnlyList<DemoDataSummaryDto> Demos);
public sealed record DemoDataOperationResponse(DemoDataOperationResultDto Result);
