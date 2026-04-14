
namespace Organization.Organizations.Features.Companies.UpdateCompany;

public record UpdateCompanyRequest(CompanyDto Company);
public record UpdateCompanyResponse(bool IsSuccess);
public class UpdateCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}", async (UpdateCompanyRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateCompanyCommand>());
            return Results.Ok(result.Adapt<UpdateCompanyResponse>());
        })
            .WithName("UpdateCompany")
            .Produces<UpdateCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateCompany")
            .WithDescription("UpdateCompany")
            .RequireAuthorization(PermissionList.CompanyPermissions.Edit);
    }
}
