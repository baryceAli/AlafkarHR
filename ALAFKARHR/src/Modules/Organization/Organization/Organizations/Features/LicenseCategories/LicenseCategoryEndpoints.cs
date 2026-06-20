namespace Organization.Organizations.Features.LicenseCategories;

public record GetLicenseCategoriesResponse(List<LicenseCategoryDto> Categories);
public record CreateLicenseCategoryRequest(LicenseCategoryDto Category);
public record CreateLicenseCategoryResponse(LicenseCategoryDto Category);
public record UpdateLicenseCategoryRequest(LicenseCategoryDto Category);
public record SetLicenseCategoryStatusRequest(bool IsActive);

public class LicenseCategoryEndpoints : ICarterModule
{
    private const string Route = "/api/v1/organization/license-categories";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(Route, async ([FromQuery] bool includeInactive, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            var result = await sender.Send(new GetLicenseCategoriesQuery(includeInactive));
            return Results.Ok(result.Adapt<GetLicenseCategoriesResponse>());
        })
            .WithName("GetLicenseCategories")
            .Produces<GetLicenseCategoriesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.View);

        app.MapPost(Route, async (CreateLicenseCategoryRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            var result = await sender.Send(new CreateLicenseCategoryCommand(request.Category));
            return Results.Created($"{Route}/{result.Category.Id}", result.Adapt<CreateLicenseCategoryResponse>());
        })
            .WithName("CreateLicenseCategory")
            .Produces<CreateLicenseCategoryResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ManageLicense);

        app.MapPut(Route, async (UpdateLicenseCategoryRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            var result = await sender.Send(new UpdateLicenseCategoryCommand(request.Category));
            return Results.Ok(result);
        })
            .WithName("UpdateLicenseCategory")
            .Produces<UpdateLicenseCategoryResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ManageLicense);

        app.MapPatch($"{Route}/{{id:guid}}/status", async (Guid id, SetLicenseCategoryStatusRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            var result = await sender.Send(new SetLicenseCategoryStatusCommand(id, request.IsActive));
            return Results.Ok(result);
        })
            .WithName("SetLicenseCategoryStatus")
            .Produces<SetLicenseCategoryStatusResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ManageLicense);
    }

    private static bool IsCompanyScoped(HttpContext httpContext) =>
        httpContext.User.HasClaim(claim => claim.Type == "company_id");
}
