namespace Organization.Organizations.Features.Administrations.GetAdministrations;



public record GetAdministrationsResponse(PaginatedResult<AdministrationDto> AdministrationList);
public class GetAdministrationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.AdministrationEndpoint}", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetAdministrationsQuery(request));

            return Results.Ok(result.Adapt<GetAdministrationsResponse>());
        })
            .WithName("GetAdministrations")
            .Produces<GetAdministrationsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetAdministrations")
            .WithDescription("GetAdministrations");

    }
}
