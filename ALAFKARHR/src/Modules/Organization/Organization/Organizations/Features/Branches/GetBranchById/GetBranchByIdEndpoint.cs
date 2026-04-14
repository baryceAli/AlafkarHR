namespace Organization.Organizations.Features.Branches.GetBranchById;


public record GetBranchByIdResponse(BranchDto Branch);
public class GetBranchByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.BranchEndpoint}/" + "{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetBranchByIdQuery(id));

            return Results.Ok(result.Adapt<GetBranchByIdResponse>());
        })
            .WithName("GetBranchById")
            .Produces<GetBranchByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetBranchById")
            .WithDescription("GetBranchById");
    }
}
