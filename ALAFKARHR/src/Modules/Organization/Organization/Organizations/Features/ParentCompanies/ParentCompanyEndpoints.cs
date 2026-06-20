namespace Organization.Organizations.Features.ParentCompanies;

public record GetParentCompaniesResponse(PaginatedResult<ParentCompanyDto> CompanyList);
public record GetParentCompanyByIdResponse(ParentCompanyDto Company);
public record CreateParentCompanyRequest(ParentCompanyDto Company);
public record CreateParentCompanyResponse(ParentCompanyDto CreatedCompany);
public record UpdateParentCompanyRequest(ParentCompanyDto Company);
public record UpdateParentCompanyLicenseRequest(CompanyLicenseDto License);
public record SetParentCompanyStatusRequest(bool IsActive);
public record ResetParentCompanyAdminPasswordRequest(string TemporaryPassword);

public class ParentCompanyEndpoints : ICarterModule
{
    private const string ParentCompanyRoute = "/api/v1/organization/parent-companies";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ParentCompanyRoute, async ([AsParameters] PaginationRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new GetParentCompaniesQuery(request));
            return Results.Ok(result.Adapt<GetParentCompaniesResponse>());
        })
            .WithName("GetParentCompanies")
            .Produces<GetParentCompaniesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.View);

        app.MapGet($"{ParentCompanyRoute}/{{id:guid}}", async (Guid id, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new GetParentCompanyByIdQuery(id));
            return Results.Ok(result.Adapt<GetParentCompanyByIdResponse>());
        })
            .WithName("GetParentCompanyById")
            .Produces<GetParentCompanyByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.View);

        app.MapPost(ParentCompanyRoute, async (CreateParentCompanyRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new CreateParentCompanyCommand(request.Company));
            return Results.Created($"{ParentCompanyRoute}/{result.CreatedCompany.Id}", result.Adapt<CreateParentCompanyResponse>());
        })
            .WithName("CreateParentCompany")
            .Produces<CreateParentCompanyResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.Create);

        app.MapPut(ParentCompanyRoute, async (UpdateParentCompanyRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UpdateParentCompanyCommand(request.Company));
            return Results.Ok(result);
        })
            .WithName("UpdateParentCompany")
            .Produces<UpdateParentCompanyResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.Edit);

        app.MapPut($"{ParentCompanyRoute}/{{id:guid}}/license", async (Guid id, UpdateParentCompanyLicenseRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UpdateParentCompanyLicenseCommand(id, request.License));
            return Results.Ok(result);
        })
            .WithName("UpdateParentCompanyLicense")
            .Produces<UpdateParentCompanyLicenseResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ManageLicense);

        app.MapPatch($"{ParentCompanyRoute}/{{id:guid}}/status", async (Guid id, SetParentCompanyStatusRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new SetParentCompanyStatusCommand(id, request.IsActive));
            return Results.Ok(result);
        })
            .WithName("SetParentCompanyStatus")
            .Produces<SetParentCompanyStatusResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.Suspend);

        app.MapPost($"{ParentCompanyRoute}/{{id:guid}}/admin/reset-password", async (Guid id, ResetParentCompanyAdminPasswordRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new ResetParentCompanyAdminPasswordCommand(id, request.TemporaryPassword));
            return Results.Ok(result);
        })
            .WithName("ResetParentCompanyAdminPassword")
            .Produces<ResetParentCompanyAdminPasswordResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ResetAdminPassword);

        app.MapDelete($"{ParentCompanyRoute}/{{id:guid}}", async (Guid id, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new DeleteParentCompanyCommand(id));
            return Results.Ok(result);
        })
            .WithName("DeleteParentCompany")
            .Produces<DeleteParentCompanyResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.Delete);
    }

    private static bool IsCompanyScoped(HttpContext httpContext) =>
        httpContext.User.HasClaim(claim => claim.Type == "company_id");
}
