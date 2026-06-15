namespace Organization.Organizations.Features.Companies.SetChildCompanyStatus;

public record SetChildCompanyStatusRequest(bool IsActive);
public record SetChildCompanyStatusResponse(bool IsSuccess);

public class SetChildCompanyStatusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/child-companies/{{companyId:guid}}/status", async ([FromRoute] Guid companyId, SetChildCompanyStatusRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SetChildCompanyStatusCommand(companyId, request.IsActive));
            return Results.Ok(result.Adapt<SetChildCompanyStatusResponse>());
        })
            .WithName("SetChildCompanyStatus")
            .Produces<SetChildCompanyStatusResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Set child company status")
            .WithDescription("Set child company status")
            .RequireAuthorization(PermissionList.CompanyPermissions.DisableChild);
    }
}
