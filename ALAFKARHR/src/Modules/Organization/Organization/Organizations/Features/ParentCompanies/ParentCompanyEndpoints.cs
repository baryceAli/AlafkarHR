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
        app.MapGet(ParentCompanyRoute, async ([AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetParentCompaniesQuery(request));
            return Results.Ok(result.Adapt<GetParentCompaniesResponse>());
        })
            .WithName("GetParentCompanies")
            .Produces<GetParentCompaniesResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.View);

        app.MapGet($"{ParentCompanyRoute}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetParentCompanyByIdQuery(id));
            return Results.Ok(result.Adapt<GetParentCompanyByIdResponse>());
        })
            .WithName("GetParentCompanyById")
            .Produces<GetParentCompanyByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.View);

        app.MapPost(ParentCompanyRoute, async (CreateParentCompanyRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateParentCompanyCommand(request.Company));
            return Results.Created($"{ParentCompanyRoute}/{result.CreatedCompany.Id}", result.Adapt<CreateParentCompanyResponse>());
        })
            .WithName("CreateParentCompany")
            .Produces<CreateParentCompanyResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.Create);

        app.MapPut(ParentCompanyRoute, async (UpdateParentCompanyRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateParentCompanyCommand(request.Company));
            return Results.Ok(result);
        })
            .WithName("UpdateParentCompany")
            .Produces<UpdateParentCompanyResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.Edit);

        app.MapPut($"{ParentCompanyRoute}/{{id:guid}}/license", async (Guid id, UpdateParentCompanyLicenseRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateParentCompanyLicenseCommand(id, request.License));
            return Results.Ok(result);
        })
            .WithName("UpdateParentCompanyLicense")
            .Produces<UpdateParentCompanyLicenseResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ManageLicense);

        app.MapPatch($"{ParentCompanyRoute}/{{id:guid}}/status", async (Guid id, SetParentCompanyStatusRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SetParentCompanyStatusCommand(id, request.IsActive));
            return Results.Ok(result);
        })
            .WithName("SetParentCompanyStatus")
            .Produces<SetParentCompanyStatusResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.Suspend);

        app.MapPost($"{ParentCompanyRoute}/{{id:guid}}/admin/reset-password", async (Guid id, ResetParentCompanyAdminPasswordRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ResetParentCompanyAdminPasswordCommand(id, request.TemporaryPassword));
            return Results.Ok(result);
        })
            .WithName("ResetParentCompanyAdminPassword")
            .Produces<ResetParentCompanyAdminPasswordResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ResetAdminPassword);

        app.MapDelete($"{ParentCompanyRoute}/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteParentCompanyCommand(id));
            return Results.Ok(result);
        })
            .WithName("DeleteParentCompany")
            .Produces<DeleteParentCompanyResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.Delete);
    }
}
