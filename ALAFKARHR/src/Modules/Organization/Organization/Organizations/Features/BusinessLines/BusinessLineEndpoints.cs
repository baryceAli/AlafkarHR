namespace Organization.Organizations.Features.BusinessLines;

public record GetBusinessLinesResponse(List<BusinessLineDto> BusinessLines);
public record CreateBusinessLineRequest(BusinessLineDto BusinessLine);
public record CreateBusinessLineResponse(BusinessLineDto BusinessLine);
public record UpdateBusinessLineRequest(BusinessLineDto BusinessLine);
public record SetBusinessLineStatusRequest(bool IsActive);

public class BusinessLineEndpoints : ICarterModule
{
    private const string Route = "/api/v1/organization/business-lines";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(Route, async (bool? includeInactive, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            var result = await sender.Send(new GetBusinessLinesQuery(includeInactive == true));
            return Results.Ok(result.Adapt<GetBusinessLinesResponse>());
        })
            .WithName("GetBusinessLines")
            .Produces<GetBusinessLinesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.View);

        app.MapPost(Route, async (CreateBusinessLineRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            var result = await sender.Send(new CreateBusinessLineCommand(request.BusinessLine));
            return Results.Created($"{Route}/{result.BusinessLine.Id}", result.Adapt<CreateBusinessLineResponse>());
        })
            .WithName("CreateBusinessLine")
            .Produces<CreateBusinessLineResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ManageLicense);

        app.MapPut(Route, async (UpdateBusinessLineRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            var result = await sender.Send(new UpdateBusinessLineCommand(request.BusinessLine));
            return Results.Ok(result);
        })
            .WithName("UpdateBusinessLine")
            .Produces<UpdateBusinessLineResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ManageLicense);

        app.MapPatch($"{Route}/{{id:guid}}/status", async (Guid id, SetBusinessLineStatusRequest request, HttpContext httpContext, ISender sender) =>
        {
            if (IsCompanyScoped(httpContext))
                return Results.Forbid();

            var result = await sender.Send(new SetBusinessLineStatusCommand(id, request.IsActive));
            return Results.Ok(result);
        })
            .WithName("SetBusinessLineStatus")
            .Produces<SetBusinessLineStatusResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.ManageLicense);
    }

    private static bool IsCompanyScoped(HttpContext httpContext) =>
        httpContext.User.HasClaim(claim => claim.Type == "company_id");
}
