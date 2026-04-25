using Organization.Organizations.Features.Administrations.GetAdministrations;

namespace Organization.Organizations.Features.Administrations.GetAdministrationsByBranchId;

public record GetAdministrationsByBranchIdResponse(PaginatedResult<AdministrationDto> AdministrationList);
public class GetAdministrationsByBranchIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.AdministrationEndpoint}"+"/BranchId/{branchId}", async ([FromRoute]Guid branchId,[AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetAdministrationsByBranchIdQuery(branchId,request));

            return Results.Ok(result.Adapt<GetAdministrationsByBranchIdResponse>());
        })
            .WithName("GetAdministrationsByBranchId")
            .Produces<GetAdministrationsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetAdministrationsByBranchId")
            .WithDescription("GetAdministrationsByBranchId")
            .RequireAuthorization(PermissionList.AdministrationPermissions.View);

    }
}
