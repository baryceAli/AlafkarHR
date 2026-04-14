namespace Organization.Organizations.Features.Administrations.UpdateAdministration;


public record UpdateAdministrationRequest(AdministrationDto Administration);
public record UpdateAdministrationResponse(bool IsSuccess);
public class UpdateAdministrationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.ROUTE_PATTERN}/{Utils.AdministrationEndpoint}", async (UpdateAdministrationRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateAdministrationCommand>());
            return Results.Ok(result.Adapt<UpdateAdministrationResponse>());
        })
            .WithName("UpdateAdministration")
            .Produces<UpdateAdministrationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateAdministration")
            .WithDescription("UpdateAdministration")
            .RequireAuthorization(PermissionList.AdministrationPermissions.Edit);
    }
}
