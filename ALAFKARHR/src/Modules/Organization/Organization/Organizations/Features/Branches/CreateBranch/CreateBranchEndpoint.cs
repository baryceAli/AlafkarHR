namespace Organization.Organizations.Features.Branches.CreateBranch;


public record CreateBranchRequest(BranchDto Branch);
public record CreateBranchResponse(BranchDto CreatedBranch);
public class CreateBranchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.ROUTE_PATTERN}/{Utils.BranchEndpoint}", async (CreateBranchRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateBranchCommand>());
            return Results.Created($"{Utils.ROUTE_PATTERN}/{Utils.BranchEndpoint}/{result.CreatedBranch.Id}", result.Adapt<CreateBranchResponse>());
        })
            .WithName("CreateBranch")
            .Produces<CreateBranchResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            //.ProducesProblem(StatusCodes.)
            .WithSummary("CreateBranch")
            .WithDescription("CreateBranch")
            .RequireAuthorization(PermissionList.BranchPermissions.Create);
    }
}
