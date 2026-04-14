namespace Organization.Organizations.Features.Administrations.CreateAdministration;


public record CreateAdministrationRequest(AdministrationDto Administration);
public record CreateAdministrationResponse(AdministrationDto CreatedAdministration);
public class CreateAdministrationEdnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.ROUTE_PATTERN}/{Utils.AdministrationEndpoint}", async (CreateAdministrationRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateAdministrationCommand>());
            return Results.Created($"{Utils.ROUTE_PATTERN}/{Utils.AdministrationEndpoint}/{result.CreatedAdministration.Id}",
                result.Adapt<CreateAdministrationResponse>());
        })
            .WithName("CreateAdministration")
            .Produces<CreateAdministrationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("CreateAdministration")
            .WithDescription("CreateAdministration")
            .RequireAuthorization(PermissionList.AdministrationPermissions.Create);
    }
}
