namespace Organization.Organizations.Features.Companies.UpdateChildCompany;

public record UpdateChildCompanyRequest(CompanyDto Company);
public record UpdateChildCompanyResponse(bool IsSuccess);

public class UpdateChildCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/child-companies", async (UpdateChildCompanyRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateChildCompanyCommand>());
            return Results.Ok(result.Adapt<UpdateChildCompanyResponse>());
        })
            .WithName("UpdateChildCompany")
            .Produces<UpdateChildCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Update child company")
            .WithDescription("Update child company")
            .RequireAuthorization(PermissionList.CompanyPermissions.EditChild);
    }
}
