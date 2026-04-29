namespace Organization.Organizations.Features.Branches.GetByCompanyId;

public record GetByCompanyIdRequest(Guid CompanyId, PaginationRequest PaginationRequest);
public record GetByCompanyIdResponse(PaginatedResult<BranchDto> BranchList);
public class GetByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.BranchEndpoint}/GetByCompanyId/" + "{companyId}", async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetByCompanyIdQuery(companyId, request));
            return Results.Ok(result.Adapt<GetByCompanyIdResponse>());
        })
            .WithName("GetByCompanyId")
            .Produces<GetByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetByCompanyId")
            .WithDescription("GetByCompanyId");
            //.RequireAuthorization(PermissionList.BranchPermissions.View);

    }
}
