namespace Organization.Organizations.Features.Branches.GetByCompanyId;


public record GetByCompanyIdResponse(List<BranchDto> BranchList);
public class GetByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.BranchEndpoint}/GetByCompanyId/" + "{companyId}", async (Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetByCompanyIdQuery(companyId));
            return Results.Ok(result.Adapt<GetByCompanyIdResponse>());
        })
            .WithName("GetByCompanyId")
            .Produces<GetByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetByCompanyId")
            .WithDescription("GetByCompanyId");

    }
}
