
namespace Organization.Organizations.Features.Companies.DeleteCompany;

public record DeleteCompanyResponse(bool IsSuccess);
public class DeleteCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}"+"{id}", async ([FromRoute] Guid id, ISender sender) =>
        {
            var command = new DeleteCompanyCommand(id);
            var result = await sender.Send(command);
            return Results.Ok(result.Adapt<DeleteCompanyResponse>());
        })
            .WithName("DeleteCompany")
            .Produces<DeleteCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("DeleteCompany")
            .WithDescription("DeleteCompany")
            .RequireAuthorization(PermissionList.CompanyPermissions.Delete);
    }
}
