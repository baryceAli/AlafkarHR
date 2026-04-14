namespace Organization.Organizations.Features.Administrations.DeleteAdministration;


public record DeleteAdministrationResponse(bool IsSuccess);
public class DeleteAdministrationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete($"{Utils.ROUTE_PATTERN}/{Utils.AdministrationEndpoint}/" + "{id:guid}", async ([FromRoute] Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteAdministrationCommand(id));
            return Results.Ok(result.Adapt<DeleteAdministrationResponse>());
        })
            .WithName("DeleteAdministration")
            .Produces<DeleteAdministrationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeleteAdministration")
            .WithDescription("DeleteAdministration")
            .RequireAuthorization(PermissionList.AdministrationPermissions.Delete);

    }
}
