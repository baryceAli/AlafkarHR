namespace Organization.Organizations.Features.Structure;

public record GetOrganizationStructureResponse(OrganizationStructureDto Structure);

public class GetOrganizationStructureEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/Structure", async (ISender sender) =>
        {
            var result = await sender.Send(new GetOrganizationStructureQuery());
            return Results.Ok(result.Adapt<GetOrganizationStructureResponse>());
        })
            .WithName("GetOrganizationStructure")
            .Produces<GetOrganizationStructureResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get organization structure")
            .WithDescription("Gets the organization hierarchy tree for the current user's company scope.")
            .RequireAuthorization(PermissionList.CompanyPermissions.Select);
    }
}
