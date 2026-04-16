namespace Organization.Organizations.Features.Administrations.GetAdministrationById;


public record GetAdministrationByIdResponse(AdministrationDto Administration);
public class GetAdministrationByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.AdministrationEndpoint}/" + "{id:guid}", async ([FromRoute]Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetAdministrationByIdQuery(id));
            return Results.Ok(result.Adapt<GetAdministrationByIdResponse>());
        })
            .WithName("GetAdministrationById")
            .Produces<GetAdministrationByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetAdministrationById")
            .WithDescription("GetAdministrationById");
    }
}
